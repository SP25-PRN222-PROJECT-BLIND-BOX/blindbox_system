using BlindBoxShop.Entities.Models;

namespace BlindBoxShop.Repository.Contract
{
    public interface IOrderDetailRepository : IRepositoryBase<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges);
    }
}
