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
        
        /// <summary>
        /// Updates only the BlindBoxItemId field of an order detail
        /// </summary>
        /// <param name="orderDetailId">The ID of the order detail to update</param>
        /// <param name="blindBoxItemId">The new BlindBoxItemId value</param>
        /// <returns>A Result containing a boolean indicating success</returns>
        Task<Result<bool>> UpdateOrderDetailBlindBoxItemAsync(Guid orderDetailId, Guid blindBoxItemId);
    }
}
