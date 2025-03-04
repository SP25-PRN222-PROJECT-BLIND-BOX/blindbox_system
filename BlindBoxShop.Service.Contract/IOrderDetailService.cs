using BlindBoxShop.Shared.DataTransferObject.OrderDetail;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IOrderDetailService : IDisposable
    {
        Task<Result<IEnumerable<OrderDetailDto>>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges);
    }
}
