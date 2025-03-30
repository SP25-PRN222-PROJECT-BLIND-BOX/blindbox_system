using AutoMapper;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.OrderDetail;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBoxShop.Service
{
    public class OrderDetailService : BaseService, IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBlindBoxPriceHistoryRepository _blindBoxPriceHistoryRepository;

        public OrderDetailService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _orderDetailRepository = repositoryManager.OrderDetail;
            _blindBoxPriceHistoryRepository = repositoryManager.BlindBoxPriceHistory;
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

        private string GetFirstImageUrl(BlindBox blindBox)
        {
            if (blindBox?.BlindBoxImages == null || !blindBox.BlindBoxImages.Any()) 
                return "/images/box-placeholder.jpg";
                
            var firstImage = blindBox.BlindBoxImages.FirstOrDefault();
            return EnsureCorrectImageUrl(firstImage?.ImageUrl ?? "");
        }

        public async Task<Result<OrderDetailDto>> CreateOrderDetailAsync(OrderDetailForCreationDto orderDetailForCreation)
        {
            try
            {
                Console.WriteLine($"Creating order detail for BlindBox ID: {orderDetailForCreation.BlindBoxId}, Order ID: {orderDetailForCreation.OrderId}");
                
                // Lấy BlindBoxPriceHistory mới nhất cho BlindBox này
                var priceHistory = await _blindBoxPriceHistoryRepository.GetLatestPriceHistoryByBlindBoxIdAsync(
                    orderDetailForCreation.BlindBoxId, false);

                if (priceHistory == null)
                {
                    Console.WriteLine($"No price history found for BlindBox ID: {orderDetailForCreation.BlindBoxId}. Creating new price history.");
                    
                    // Lấy BlindBox từ repository
                    var blindBox = await _repositoryManager.BlindBox.FindById(orderDetailForCreation.BlindBoxId, false);
                    
                    if (blindBox == null)
                    {
                        Console.WriteLine($"ERROR: BlindBox with ID {orderDetailForCreation.BlindBoxId} not found.");
                        return Result<OrderDetailDto>.Failure(new ErrorResult
                        {
                            Code = "BlindBoxNotFound",
                            Description = $"BlindBox with ID {orderDetailForCreation.BlindBoxId} not found"
                        });
                    }
                    
                    // Tạo mới price history
                    priceHistory = new BlindBoxPriceHistory
                    {
                        BlindBoxId = orderDetailForCreation.BlindBoxId,
                        DefaultPrice = orderDetailForCreation.Price,
                        Price = orderDetailForCreation.Price
                    };
                    
                    // Lưu price history mới
                    _repositoryManager.BlindBoxPriceHistory.Create(priceHistory);
                    await _repositoryManager.BlindBoxPriceHistory.SaveAsync();
                    
                    Console.WriteLine($"Created new price history with ID: {priceHistory.Id}");
                    
                    // Lấy lại priceHistory có kèm BlindBox để có thông tin đầy đủ
                    priceHistory = await _blindBoxPriceHistoryRepository.GetLatestPriceHistoryByBlindBoxIdAsync(
                        orderDetailForCreation.BlindBoxId, false);
                    
                    if (priceHistory == null)
                    {
                        Console.WriteLine("ERROR: Failed to retrieve the newly created price history.");
                        return Result<OrderDetailDto>.Failure(new ErrorResult
                        {
                            Code = "PriceHistoryCreationFailed",
                            Description = "Failed to create and retrieve price history"
                        });
                    }
                }
                
                Console.WriteLine($"Using price history ID: {priceHistory.Id}");

                // Tạo OrderDetail với BlindBoxPriceHistoryId chính xác
                var orderDetailEntity = new OrderDetail
                {
                    OrderId = orderDetailForCreation.OrderId,
                    BlindBoxPriceHistoryId = priceHistory.Id,
                    Quantity = orderDetailForCreation.Quantity,
                    BlindBoxItemId = orderDetailForCreation.BlindBoxItemId
                };
                
                _orderDetailRepository.Create(orderDetailEntity);
                await _orderDetailRepository.SaveAsync();
                
                Console.WriteLine($"Created order detail with ID: {orderDetailEntity.Id}");

                // Lấy hình ảnh đầu tiên của BlindBox
                string imageUrl = GetFirstImageUrl(priceHistory.BlindBox);

                // Tạo và trả về DTO
                var orderDetailToReturn = new OrderDetailDto
                {
                    Id = orderDetailEntity.Id,
                    Quantity = orderDetailEntity.Quantity,
                    BlindBoxName = priceHistory.BlindBox?.Name ?? "",
                    Price = priceHistory.Price,
                    ImageUrl = imageUrl
                };
                
                return Result<OrderDetailDto>.Success(orderDetailToReturn);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in CreateOrderDetailAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return Result<OrderDetailDto>.Failure(new ErrorResult 
                { 
                    Code = "CreateOrderDetailError",
                    Description = $"Error creating order detail: {ex.Message}" 
                });
            }
        }

        public async Task<Result<IEnumerable<OrderDetailDto>>> CreateBatchOrderDetailsAsync(IEnumerable<OrderDetailForCreationDto> orderDetailsForCreation)
        {
            try
            {
                Console.WriteLine($"Creating batch order details, count: {orderDetailsForCreation.Count()}");
                
                var orderDetailEntities = new List<OrderDetail>();
                var orderDetailDtos = new List<OrderDetailDto>();
                
                foreach (var dto in orderDetailsForCreation)
                {
                    Console.WriteLine($"Processing order detail for BlindBox ID: {dto.BlindBoxId}, Order ID: {dto.OrderId}");
                    
                    // Lấy BlindBoxPriceHistory mới nhất cho BlindBox này
                    var priceHistory = await _blindBoxPriceHistoryRepository.GetLatestPriceHistoryByBlindBoxIdAsync(
                        dto.BlindBoxId, false);

                    if (priceHistory == null)
                    {
                        Console.WriteLine($"No price history found for BlindBox ID: {dto.BlindBoxId}. Creating new price history.");
                        
                        // Lấy BlindBox từ repository
                        var blindBox = await _repositoryManager.BlindBox.FindById(dto.BlindBoxId, false);
                        
                        if (blindBox == null)
                        {
                            Console.WriteLine($"ERROR: BlindBox with ID {dto.BlindBoxId} not found.");
                            return Result<IEnumerable<OrderDetailDto>>.Failure(new ErrorResult
                            {
                                Code = "BlindBoxNotFound",
                                Description = $"BlindBox with ID {dto.BlindBoxId} not found"
                            });
                        }
                        
                        // Tạo mới price history
                        priceHistory = new BlindBoxPriceHistory
                        {
                            BlindBoxId = dto.BlindBoxId,
                            DefaultPrice = dto.Price,
                            Price = dto.Price
                        };
                        
                        // Lưu price history mới
                        _repositoryManager.BlindBoxPriceHistory.Create(priceHistory);
                        await _repositoryManager.BlindBoxPriceHistory.SaveAsync();
                        
                        Console.WriteLine($"Created new price history with ID: {priceHistory.Id}");
                        
                        // Lấy lại priceHistory có kèm BlindBox để có thông tin đầy đủ
                        priceHistory = await _blindBoxPriceHistoryRepository.GetLatestPriceHistoryByBlindBoxIdAsync(
                            dto.BlindBoxId, false);
                        
                        if (priceHistory == null)
                        {
                            Console.WriteLine("ERROR: Failed to retrieve the newly created price history.");
                            return Result<IEnumerable<OrderDetailDto>>.Failure(new ErrorResult
                            {
                                Code = "PriceHistoryCreationFailed",
                                Description = "Failed to create and retrieve price history"
                            });
                        }
                    }
                    
                    Console.WriteLine($"Using price history ID: {priceHistory.Id}");

                    // Lấy hình ảnh đầu tiên của BlindBox
                    string imageUrl = GetFirstImageUrl(priceHistory.BlindBox);

                    // Tạo OrderDetail với BlindBoxPriceHistoryId chính xác
                    var entity = new OrderDetail
                    {
                        OrderId = dto.OrderId,
                        BlindBoxPriceHistoryId = priceHistory.Id,
                        Quantity = dto.Quantity
                    };
                    
                    _orderDetailRepository.Create(entity);
                    orderDetailEntities.Add(entity);
                    
                    Console.WriteLine($"Added order detail entity to batch");
                    
                    // Tạo DTO để trả về - đây chỉ là dự kiến ID, sẽ không chính xác cho đến khi SaveAsync
                    orderDetailDtos.Add(new OrderDetailDto
                    {
                        Id = entity.Id, // Entity ID sẽ là Guid.Empty cho đến khi SaveAsync
                        Quantity = entity.Quantity,
                        BlindBoxName = priceHistory.BlindBox?.Name ?? "",
                        Price = priceHistory.Price,
                        ImageUrl = imageUrl
                    });
                }
                
                Console.WriteLine("Saving all order details to database...");
                await _orderDetailRepository.SaveAsync();
                Console.WriteLine("Successfully saved all order details!");
                
                // Cập nhật ID thực tế sau khi lưu
                for (int i = 0; i < orderDetailEntities.Count; i++)
                {
                    orderDetailDtos[i].Id = orderDetailEntities[i].Id;
                }
                
                return Result<IEnumerable<OrderDetailDto>>.Success(orderDetailDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in CreateBatchOrderDetailsAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine($"Inner Inner Exception: {ex.InnerException.InnerException.Message}");
                    }
                }
                
                return Result<IEnumerable<OrderDetailDto>>.Failure(new ErrorResult 
                { 
                    Code = "CreateBatchOrderDetailsError",
                    Description = $"Error creating order details: {ex.Message}" 
                });
            }
        }

        public async Task<Result<OrderDetailDto>> GetOrderDetailByIdAsync(Guid id, bool trackChanges)
        {
            try
            {
                var orderDetail = await _orderDetailRepository.GetOrderDetailWithImagesAsync(id, trackChanges);
                
                if (orderDetail == null)
                    return Result<OrderDetailDto>.Failure(new ErrorResult 
                    { 
                        Code = "OrderDetailNotFound",
                        Description = "Order detail not found" 
                    });

                // Lấy hình ảnh đầu tiên của BlindBox
                string imageUrl = GetFirstImageUrl(orderDetail.BlindBoxPriceHistory?.BlindBox);

                var orderDetailDto = new OrderDetailDto
                {
                    Id = orderDetail.Id,
                    Quantity = orderDetail.Quantity,
                    BlindBoxName = orderDetail.BlindBoxPriceHistory?.BlindBox?.Name ?? "Unknown",
                    Price = orderDetail.BlindBoxPriceHistory?.Price ?? 0,
                    ImageUrl = imageUrl
                };
                
                return Result<OrderDetailDto>.Success(orderDetailDto);
            }
            catch (Exception ex)
            {
                return Result<OrderDetailDto>.Failure(new ErrorResult 
                { 
                    Code = "GetOrderDetailError",
                    Description = $"Error retrieving order detail: {ex.Message}" 
                });
            }
        }

        public async Task<Result<IEnumerable<OrderDetailDto>>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges)
        {
            try
            {
                Console.WriteLine($"Getting order details for Order ID: {orderId}");
                var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId, trackChanges);
                var orderDetailsList = orderDetails.ToList();
                
                // Nếu không có order details, trả về danh sách rỗng
                if (!orderDetailsList.Any())
                {
                    Console.WriteLine("No order details found");
                    return Result<IEnumerable<OrderDetailDto>>.Success(new List<OrderDetailDto>());
                }
                
                // Lấy danh sách order detail IDs để query hình ảnh
                var orderDetailIds = orderDetailsList.Select(od => od.Id).ToList();
                
                // Lấy hình ảnh cho tất cả order details trong một query duy nhất
                var imageUrls = await _orderDetailRepository.GetImageUrlsForOrderDetailsAsync(orderDetailIds);
                
                var orderDetailsDto = orderDetailsList.Select(detail => 
                {
                    // Lấy hình ảnh từ dictionary, nếu không có thì lấy từ BlindBox hoặc sử dụng placeholder
                    string imageUrl;
                    
                    if (imageUrls.TryGetValue(detail.Id, out var url))
                    {
                        imageUrl = url;
                        Console.WriteLine($"Found image URL in dictionary for OrderDetail ID: {detail.Id}: '{imageUrl}'");
                    }
                    else
                    {
                        // Fallback to old method
                        imageUrl = GetFirstImageUrl(detail.BlindBoxPriceHistory?.BlindBox);
                        Console.WriteLine($"Using fallback image URL for OrderDetail ID: {detail.Id}: '{imageUrl}'");
                    }
                    
                    // Đảm bảo URL hình ảnh được chuẩn hóa
                    imageUrl = EnsureCorrectImageUrl(imageUrl);
                    
                    Console.WriteLine($"Final image URL for OrderDetail ID: {detail.Id}, Product: {detail.BlindBoxPriceHistory?.BlindBox?.Name}: '{imageUrl}'");
                    
                    // Trả về DTO với hình ảnh chuẩn hóa
                    return new OrderDetailDto
                    {
                        Id = detail.Id,
                        Quantity = detail.Quantity,
                        BlindBoxName = detail.BlindBoxPriceHistory?.BlindBox?.Name ?? "Unknown",
                        Price = detail.BlindBoxPriceHistory?.Price ?? 0,
                        ImageUrl = imageUrl,
                        BlindBoxId = detail.BlindBoxPriceHistory?.BlindBoxId ?? Guid.Empty
                    };
                }).ToList();
                
                return Result<IEnumerable<OrderDetailDto>>.Success(orderDetailsDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetOrderDetailsByOrderIdAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return Result<IEnumerable<OrderDetailDto>>.Failure(new ErrorResult 
                { 
                    Code = "GetOrderDetailsError",
                    Description = $"Error retrieving order details: {ex.Message}" 
                });
            }
        }

        public async Task<Result<bool>> UpdateOrderDetailBlindBoxItemAsync(Guid orderDetailId, Guid blindBoxItemId)
        {
            try
            {
                Console.WriteLine($"Updating order detail {orderDetailId} with BlindBoxItemId: {blindBoxItemId}");
                
                // Find the order detail by ID
                var orderDetail = await _orderDetailRepository.FindById(orderDetailId, true);
                
                if (orderDetail == null)
                {
                    return Result<bool>.Failure(new ErrorResult
                    {
                        Code = "OrderDetailNotFound",
                        Description = $"Order detail with ID {orderDetailId} not found"
                    });
                }
                
                // Update only the BlindBoxItemId field
                orderDetail.BlindBoxItemId = blindBoxItemId;
                
                // Save the changes
                _orderDetailRepository.Update(orderDetail);
                await _orderDetailRepository.SaveAsync();
                
                Console.WriteLine($"Successfully updated order detail {orderDetailId} with BlindBoxItemId: {blindBoxItemId}");
                
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in UpdateOrderDetailBlindBoxItemAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                return Result<bool>.Failure(new ErrorResult
                {
                    Code = "UpdateOrderDetailError",
                    Description = $"Error updating order detail: {ex.Message}"
                });
            }
        }

        public void Dispose()
        {
            _orderDetailRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
