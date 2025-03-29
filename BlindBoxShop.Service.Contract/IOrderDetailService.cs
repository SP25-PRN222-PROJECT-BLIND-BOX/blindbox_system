using BlindBoxShop.Shared.DataTransferObject.OrderDetail;
using BlindBoxShop.Shared.ResultModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Service.Contract
{
    public interface IOrderDetailService : IDisposable
    {
        Task<Result<OrderDetailDto>> CreateOrderDetailAsync(OrderDetailForCreationDto orderDetailForCreation);
        Task<Result<IEnumerable<OrderDetailDto>>> CreateBatchOrderDetailsAsync(IEnumerable<OrderDetailForCreationDto> orderDetailsForCreation);
        Task<Result<IEnumerable<OrderDetailDto>>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges);
        Task<Result<OrderDetailDto>> GetOrderDetailByIdAsync(Guid id, bool trackChanges);
    }
}
