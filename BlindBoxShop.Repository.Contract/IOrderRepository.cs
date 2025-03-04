using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Repository.Contract
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        Task<PagedList<Order>> GetOrdersAsync(OrderParameter orderParameter, bool trackChanges);
        Task<PagedList<Order>> GetOrdersByUserIdAsync(Guid userId, OrderParameter orderParameter, bool trackChanges);
        Task<PagedList<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, OrderParameter orderParameter, bool trackChanges);
    }
}
