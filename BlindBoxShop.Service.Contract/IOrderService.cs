using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IOrderService : IDisposable
    {
        Task<Result<IEnumerable<OrderDto>>> GetOrdersAsync(OrderParameter orderParameter, bool trackChanges);
        Task<Result<OrderDto>> CreateOrderAsync(OrderForCreationDto orderForCreateDto);
        Task<Result> CancelOrderAsync(Guid id);
        //Task<Result<OrderWithDetailsDto>> GetOrderWithDetailsByIdAsync(Guid id, bool trackChanges);
        Task<Result<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(Guid userId, OrderParameter orderParameter, bool trackChanges);
    }
}
