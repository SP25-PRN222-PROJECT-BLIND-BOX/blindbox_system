using BlindBoxShop.Shared.DataTransferObject.VNPay;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IVNPayService
    {
        // Creates a payment URL for VNPay
        Task<Result<string>> GetPaymentUrlAsync(Guid orderId, Guid userId, string returnUrl);
        
        // Processes the callback from VNPay after payment
        Task<Result<VNPayResponse>> ProcessPaymentCallbackAsync(VNPayRequest request);
    }
} 