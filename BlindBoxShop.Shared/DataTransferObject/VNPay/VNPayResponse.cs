namespace BlindBoxShop.Shared.DataTransferObject.VNPay
{
    public class VNPayResponse
    {
        public bool IsSuccessful { get; set; }
        public string RedirectUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        
        // Additional data that might be useful
        public string OrderId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }
} 