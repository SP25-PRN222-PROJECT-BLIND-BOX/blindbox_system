using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.ResultModel;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using System.ComponentModel.DataAnnotations;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.DataTransferObject.OrderDetail;

namespace BlindBoxShop.Service
{
        public class OrderService : BaseService, IOrderService
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IOrderDetailRepository _orderDetailRepository;

            public OrderService(IRepositoryManager repositoryManager, IMapper mapper, IOrderDetailRepository orderDetailRepository) : base(repositoryManager, mapper)
            {
                _orderRepository = repositoryManager.Order;
                _orderDetailRepository = orderDetailRepository;
            }

        private async Task<Result<Order>> GetAndCheckIfOrderExistByIdAsync(Guid id, bool trackChanges)
        {
            var order = await _orderRepository.FindById(id, trackChanges);
            if (order is null)
                return Result<Order>.Failure(OrderErrors.GetOrderNotFoundError(id));

            return Result<Order>.Success(order);
        }

        // Create Order
        public async Task<Result<OrderDto>> CreateOrderAsync(OrderForCreationDto orderForCreateDto)
        {
            try
            {
                // Đảm bảo tính phí vận chuyển cố định
                const decimal ShippingFee = 20000;
                
                // Cập nhật tổng giá trị đơn hàng bao gồm phí vận chuyển
                if (orderForCreateDto.Total == orderForCreateDto.SubTotal)
                {
                    // Nếu Total đang bằng SubTotal, nghĩa là chưa có phí vận chuyển
                    orderForCreateDto.Total = orderForCreateDto.SubTotal + ShippingFee;
                    Console.WriteLine($"Updated order total to include shipping fee: {orderForCreateDto.Total}");
                }
                
                var orderEntity = _mapper.Map<Order>(orderForCreateDto);
                
                // Set shipping fee property if it exists
                var shippingFeeProperty = orderEntity.GetType().GetProperty("ShippingFee");
                if (shippingFeeProperty != null)
                {
                    shippingFeeProperty.SetValue(orderEntity, ShippingFee);
                }
                
                await _orderRepository.CreateAsync(orderEntity);
                await _orderRepository.SaveAsync();

                var orderDto = _mapper.Map<OrderDto>(orderEntity);
                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateOrderAsync: {ex.Message}");
                return Result<OrderDto>.Failure(new ErrorResult
                {
                    Code = "InternalError",
                    Description = "An unexpected error occurred."
                });
            }
        }

        // Get All Orders
        public async Task<Result<IEnumerable<OrderDto>>> GetOrdersAsync(OrderParameter orderParameter, bool trackChanges)
        {
            try
            {
                Console.WriteLine("Getting all orders with manual mapping");
                var orders = await _orderRepository.GetOrdersAsync(orderParameter, trackChanges);
                
                // Map thủ công thay vì dùng AutoMapper
                var ordersDto = new List<OrderDto>();
                
                foreach (var order in orders)
                {
                    var orderDto = new OrderDto
                    {
                        Id = order.Id,
                        CustomerName = order.User?.UserName ?? "Unknown",
                        Status = order.Status,
                        PaymentMethod = order.PaymentMethod,
                        Province = order.Province,
                        Wards = order.Wards,
                        Address = order.Address,
                        VoucherId = order.VoucherId,
                        SubTotal = order.SubTotal,
                        Total = order.Total,
                        CreatedAt = order.CreatedAt
                    };
                    
                    // Thêm OrderDetails nếu có
                    if (order.OrderDetails != null && order.OrderDetails.Any())
                    {
                        // Lấy OrderDetails từ repository để đảm bảo có đủ thông tin
                        var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(order.Id, trackChanges);
                        
                        var orderDetailsDto = orderDetails.Select(detail => new OrderDetailDto
                        {
                            Id = detail.Id,
                            Quantity = detail.Quantity,
                            BlindBoxName = detail.BlindBoxPriceHistory?.BlindBox?.Name ?? "Unknown",
                            Price = detail.BlindBoxPriceHistory?.Price ?? 0,
                            ImageUrl = GetFirstImageUrl(detail.BlindBoxPriceHistory?.BlindBox),
                            BlindBoxId = detail.BlindBoxPriceHistory?.BlindBoxId ?? Guid.Empty
                        }).ToList();
                        
                        // Kiểm tra nếu OrderDto có thuộc tính OrderDetails, thì mới set
                        var orderDetailsPropertyInfo = typeof(OrderDto).GetProperty("OrderDetails");
                        if (orderDetailsPropertyInfo != null) 
                        {
                            orderDetailsPropertyInfo.SetValue(orderDto, orderDetailsDto);
                        }
                    }
                    
                    ordersDto.Add(orderDto);
                }
                
                return (ordersDto, orders.MetaData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrdersAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return Result<IEnumerable<OrderDto>>.Failure(new ErrorResult
                {
                    Code = "GetOrdersError",
                    Description = $"Error retrieving orders: {ex.Message}"
                });
            }
        }

        // Helper method để lấy URL hình ảnh đầu tiên từ BlindBox
        private string GetFirstImageUrl(BlindBox blindBox)
        {
            if (blindBox?.BlindBoxImages == null || !blindBox.BlindBoxImages.Any()) 
                return "/images/box-placeholder.jpg";
                
            var firstImage = blindBox.BlindBoxImages.FirstOrDefault();
            return EnsureCorrectImageUrl(firstImage?.ImageUrl ?? "");
        }
        
        private string EnsureCorrectImageUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return "/images/box-placeholder.jpg";

            // Kiểm tra các trường hợp URL chuẩn
            if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                return imageUrl;

            // Thêm dấu '/' ở đầu nếu cần
            if (!imageUrl.StartsWith("/"))
                imageUrl = "/" + imageUrl;

            return imageUrl;
        }

        // Get Order by Id with Order Details
        public async Task<Result<OrderWithDetailsDto>> GetOrderWithDetailsByIdAsync(Guid id, bool trackChanges)
        {
            try 
            {
                Console.WriteLine($"Getting order with details for ID: {id}");
                
                // Check if the order exists
                var orderEntity = await _orderRepository.GetOrderByIdAsync(id, trackChanges);
                if (orderEntity == null)
                    return Result<OrderWithDetailsDto>.Failure(OrderErrors.GetOrderNotFoundError(id));

                // Fetch order details
                var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(id, trackChanges);

                // Map order và order details thủ công
                var orderDto = new OrderDto
                {
                    Id = orderEntity.Id,
                    CustomerName = orderEntity.User?.UserName ?? "Unknown",
                    Status = orderEntity.Status,
                    PaymentMethod = orderEntity.PaymentMethod,
                    Province = orderEntity.Province,
                    Wards = orderEntity.Wards,
                    Address = orderEntity.Address,
                    VoucherId = orderEntity.VoucherId,
                    SubTotal = orderEntity.SubTotal,
                    Total = orderEntity.Total,
                    CreatedAt = orderEntity.CreatedAt
                };

                // Map OrderDetails thủ công để đảm bảo có BlindBoxId và hình ảnh
                var orderDetailsDto = orderDetails.Select(detail => new OrderDetailDto
                {
                    Id = detail.Id,
                    Quantity = detail.Quantity,
                    BlindBoxItemId = detail.BlindBoxItemId,
                    BlindBoxName = detail.BlindBoxPriceHistory?.BlindBox?.Name ?? "Unknown",
                    Price = detail.BlindBoxPriceHistory?.Price ?? 0,
                    ImageUrl = GetFirstImageUrl(detail.BlindBoxPriceHistory?.BlindBox),
                    BlindBoxId = detail.BlindBoxPriceHistory?.BlindBoxId ?? Guid.Empty
                }).ToList();
                
                // Debug info
                foreach (var detailDto in orderDetailsDto)
                {
                    Console.WriteLine($"OrderDetail: {detailDto.BlindBoxName}, BlindBoxId: {detailDto.BlindBoxId}, ImageUrl: {detailDto.ImageUrl}");
                }

                // Combine into OrderWithDetailsDto
                var orderWithDetailsDto = new OrderWithDetailsDto
                {
                    Order = orderDto,
                    OrderDetails = orderDetailsDto
                };

                return Result<OrderWithDetailsDto>.Success(orderWithDetailsDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetOrderWithDetailsByIdAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return Result<OrderWithDetailsDto>.Failure(new ErrorResult 
                { 
                    Code = "GetOrderDetailsError",
                    Description = $"Error retrieving order details: {ex.Message}" 
                });
            }
        }

        public async Task<Result> CancelOrderAsync(Guid id)
        {
            var checkIfExistResult = await GetAndCheckIfOrderExistByIdAsync(id, trackChanges: true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var orderEntity = checkIfExistResult.GetValue<Order>();

            if (orderEntity.Status == OrderStatus.Cancelled)
            {
                return Result.Failure(new ErrorResult
                {
                    Code = "OrderAlreadyCancelled",
                    Description = "The order has already been cancelled."
                });
            }

            orderEntity.Status = OrderStatus.Cancelled;

            orderEntity.UpdatedAt = DateTime.UtcNow;

            // Lưu thay đổi vào cơ sở dữ liệu
            _orderRepository.Update(orderEntity);
            await _orderRepository.SaveAsync();

            return Result.Success();
        }


        public async Task<Result<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(Guid userId, OrderParameter orderParameter, bool trackChanges)
        {
            try
            {
                Console.WriteLine($"Getting orders for user ID: {userId} with manual mapping");
                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId, orderParameter, trackChanges);
                
                if (orders == null || orders.Count == 0)
                {
                    return Result<IEnumerable<OrderDto>>.Success(new List<OrderDto>());
                }
                
                // Map thủ công thay vì dùng AutoMapper
                var ordersDto = new List<OrderDto>();
                
                foreach (var order in orders)
                {
                    var orderDto = new OrderDto
                    {
                        Id = order.Id,
                        CustomerName = order.User?.UserName ?? "Unknown",
                        Status = order.Status,
                        PaymentMethod = order.PaymentMethod,
                        Province = order.Province,
                        Wards = order.Wards,
                        Address = order.Address,
                        VoucherId = order.VoucherId,
                        SubTotal = order.SubTotal,
                        Total = order.Total,
                        CreatedAt = order.CreatedAt
                    };
                    
                    // Thêm OrderDetails nếu có
                    if (order.OrderDetails != null && order.OrderDetails.Any())
                    {
                        // Lấy OrderDetails từ repository để đảm bảo có đủ thông tin
                        var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(order.Id, trackChanges);
                        
                        var orderDetailsDto = orderDetails.Select(detail => new OrderDetailDto
                        {
                            Id = detail.Id,
                            Quantity = detail.Quantity,
                            BlindBoxName = detail.BlindBoxPriceHistory?.BlindBox?.Name ?? "Unknown",
                            Price = detail.BlindBoxPriceHistory?.Price ?? 0,
                            ImageUrl = GetFirstImageUrl(detail.BlindBoxPriceHistory?.BlindBox),
                            BlindBoxId = detail.BlindBoxPriceHistory?.BlindBoxId ?? Guid.Empty
                        }).ToList();
                        
                        // Kiểm tra nếu OrderDto có thuộc tính OrderDetails, thì mới set
                        var orderDetailsPropertyInfo = typeof(OrderDto).GetProperty("OrderDetails");
                        if (orderDetailsPropertyInfo != null) 
                        {
                            orderDetailsPropertyInfo.SetValue(orderDto, orderDetailsDto);
                        }
                    }
                    
                    ordersDto.Add(orderDto);
                }
                
                return Result<IEnumerable<OrderDto>>.Success(ordersDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrdersByUserIdAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return Result<IEnumerable<OrderDto>>.Failure(new ErrorResult
                {
                    Code = "InternalError",
                    Description = "An error occurred while retrieving orders."
                });
            }
        }

        public void Dispose()
        {
            _orderRepository.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<Result<bool>> ChangePaymentMethodAsync(Guid orderId, PaymentMethod newPaymentMethod)
        {
            try
            {
                var checkIfExistResult = await GetAndCheckIfOrderExistByIdAsync(orderId, trackChanges: true);
                if (!checkIfExistResult.IsSuccess)
                    return Result<bool>.Failure(checkIfExistResult.Errors);

                var orderEntity = checkIfExistResult.GetValue<Order>();

                // Only allow changing payment method for awaiting payment orders
                if (orderEntity.Status != OrderStatus.AwaitingPayment)
                {
                    return Result<bool>.Failure(new ErrorResult 
                    { 
                        Code = "InvalidOrderStatus",
                        Description = "Payment method can only be changed for orders awaiting payment."
                    });
                }
                
                // Change payment method
                orderEntity.PaymentMethod = newPaymentMethod;
                
                // If changing to COD, update status to Processing
                if (newPaymentMethod == PaymentMethod.Cash)
                {
                    orderEntity.Status = OrderStatus.Processing;
                }
                
                _orderRepository.Update(orderEntity);
                await _orderRepository.SaveAsync();
                
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing payment method for order {orderId}: {ex.Message}");
                return Result<bool>.Failure(new ErrorResult 
                { 
                    Code = "ChangePaymentMethodError",
                    Description = $"Failed to change payment method: {ex.Message}"
                });
            }
        }

        public async Task<Result<bool>> UpdateOrderStatusAsync(Guid orderId, string status, string notes = "")
        {
            try
            {
                var checkIfExistResult = await GetAndCheckIfOrderExistByIdAsync(orderId, trackChanges: true);
                if (!checkIfExistResult.IsSuccess)
                    return Result<bool>.Failure(checkIfExistResult.Errors);

                var orderEntity = checkIfExistResult.GetValue<Order>();
                
                // Parse the status string to OrderStatus enum
                if (!Enum.TryParse<OrderStatus>(status, out var orderStatus))
                {
                    return Result<bool>.Failure(new ErrorResult 
                    { 
                        Code = "InvalidOrderStatus",
                        Description = $"Invalid order status: {status}"
                    });
                }
                
                // Update the status
                orderEntity.Status = orderStatus;
                orderEntity.UpdatedAt = DateTime.UtcNow;
                
                // Save changes
                _orderRepository.Update(orderEntity);
                await _orderRepository.SaveAsync();
                
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status for order {orderId}: {ex.Message}");
                return Result<bool>.Failure(new ErrorResult 
                { 
                    Code = "UpdateOrderStatusError",
                    Description = $"Failed to update order status: {ex.Message}"
                });
            }
        }
    }
}
