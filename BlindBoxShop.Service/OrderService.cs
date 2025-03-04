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
        public OrderService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _orderRepository = repositoryManager.Order;
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
                var orderEntity = _mapper.Map<Order>(orderForCreateDto);
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
            var orders = await _orderRepository.GetOrdersAsync(orderParameter, trackChanges);
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return (ordersDto, orders.MetaData);
        }

        // Get Order by Id with Order Details
        //public async Task<Result<OrderWithDetailsDto>> GetOrderWithDetailsByIdAsync(Guid id, bool trackChanges)
        //{
        //    var orderResult = await GetAndCheckIfOrderExistByIdAsync(id, trackChanges);
        //    if (!orderResult.IsSuccess)
        //        return Result<OrderWithDetailsDto>.Failure(orderResult.Errors!);

        //    var orderEntity = orderResult.GetValue<Order>();

        //    // Get Order Details using the OrderDetailService
        //    var orderDetailsResult = await _orderDetailService.GetOrderDetailsByOrderIdAsync(id, trackChanges);
        //    if (!orderDetailsResult.IsSuccess)
        //        return Result<OrderWithDetailsDto>.Failure(orderDetailsResult.Errors!);

        //    var orderDetailsDto = orderDetailsResult.GetValue<IEnumerable<OrderDetailDto>>();

        //    // Map the Order entity to OrderDto
        //    var orderDto = _mapper.Map<OrderDto>(orderEntity);

        //    // Combine Order and Order Details into a single DTO
        //    var orderWithDetailsDto = new OrderWithDetailsDto
        //    {
        //        Order = orderDto,
        //        OrderDetails = orderDetailsDto
        //    };

        //    return Result<OrderWithDetailsDto>.Success(orderWithDetailsDto);
        //}

        public async Task<Result> CancelOrderAsync(Guid id)
        {
            var checkIfExistResult = await GetAndCheckIfOrderExistByIdAsync(id, trackChanges: true);
            if (!checkIfExistResult.IsSuccess)
                return checkIfExistResult.Errors!;

            var orderEntity = checkIfExistResult.GetValue<Order>();

            if (orderEntity.Status == "Cancelled")
            {
                return Result.Failure(new ErrorResult
                {
                    Code = "OrderAlreadyCancelled",
                    Description = "The order has already been cancelled."
                });
            }

            orderEntity.Status = "Cancelled";

            orderEntity.UpdatedAt = DateTime.UtcNow;

            // Lưu thay đổi vào cơ sở dữ liệu
            _orderRepository.Update(orderEntity);
            await _orderRepository.SaveAsync();

            return Result.Success();
        }


        public async Task<Result<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(Guid userId, OrderParameter orderParameter, bool trackChanges)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId, orderParameter, trackChanges);
            if (!orders.Any())
            {
                return Result<IEnumerable<OrderDto>>.Failure(OrderErrors.GetNoOrdersFoundForUserError(userId));
            }

            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return (ordersDto, orders.MetaData);
        }

        public void Dispose()
        {
            _orderRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
