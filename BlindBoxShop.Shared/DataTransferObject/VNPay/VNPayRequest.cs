using System;
using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.VNPay
{
    public class VNPayRequest
    {
        // Request data
        public string VnpTxnRef { get; set; } = string.Empty;
        public string VnpOrderInfo { get; set; } = string.Empty;
        public string VnpResponseCode { get; set; } = string.Empty;
        public string VnpTransactionNo { get; set; } = string.Empty;
        public string VnpBankCode { get; set; } = string.Empty;
        public string VnpAmount { get; set; } = string.Empty;
        public string VnpCardType { get; set; } = string.Empty;
        public string VnpPayDate { get; set; } = string.Empty;
        public string VnpTransactionStatus { get; set; } = string.Empty;
        public string VnpBankTranNo { get; set; } = string.Empty;
        public string VnpSecureHash { get; set; } = string.Empty;
        public string VnpTmnCode { get; set; } = string.Empty;
    }
} 