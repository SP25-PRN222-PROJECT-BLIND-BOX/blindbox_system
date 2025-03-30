using BlindBoxShop.Entities.Models;

namespace BlindBoxShop.Repository.Contract
{
    public interface IOrderDetailRepository : IRepositoryBase<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges);
        Task<OrderDetail> GetOrderDetailWithImagesAsync(Guid id, bool trackChanges);
        Task<Dictionary<Guid, string>> GetImageUrlsForOrderDetailsAsync(List<Guid> orderDetailIds);
    }
}
