using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Constant.ErrorConstant;
using BlindBoxShop.Shared.DataTransferObject.VNPay;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlindBoxShop.Service
{
    public class VNPayService : BaseService, IVNPayService
    {
        private readonly ILogger<VNPayService> _logger;
        private readonly IOrderRepository _orderRepository;
        
        // VNPay configuration
        private readonly string _vnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly string _vnpReturnUrl = "/payment/vnpay-callback";
        private readonly string _vnpTmnCode = "99EBBM2U"; // Sandbox TMN Code
        private readonly string _vnpHashSecret = "HMYDH7PAL07DLX77WG37DY5I0VIJEJIB"; // Sandbox Hash Secret
        private readonly string _vnpVersion = "2.1.0";
        private readonly string _vnpCommand = "pay";
        private readonly string _orderType = "other";

        public VNPayService(
            IRepositoryManager repositoryManager, 
            IMapper mapper,
            ILogger<VNPayService> logger) : base(repositoryManager, mapper)
        {
            _logger = logger;
            _orderRepository = repositoryManager.Order;
        }

        public async Task<Result<string>> GetPaymentUrlAsync(Guid orderId, Guid userId, string returnUrl)
        {
            try
            {
                // Get order information
                var order = await _repositoryManager.Order.FindByCondition(
                    o => o.Id == orderId && o.UserId == userId, 
                    false
                ).FirstOrDefaultAsync();

                if (order == null)
                {
                    return Result<string>.Failure(OrderErrors.GetOrderNotFoundError(orderId));
                }

                if (order.Status == OrderStatus.Processing)
                {
                    return Result<string>.Failure(new ErrorResult 
                    { 
                        Code = "OrderAlreadyPaid",
                        Description = "Order is already paid."
                    });
                }

                // Generate a random transaction reference
                string vnpTxnRef = GetRandomNumber(8);
                
                // Amount needs to be multiplied by 100 per VNPay requirements
                int amount = (int)(order.Total * 100);
                string totalPrice = amount.ToString();

                // Tạo 2 URL callback khác nhau cho trường hợp thành công và thất bại
                var successReturnUrl = $"{returnUrl}/order-success/{order.Id}";
                var failureReturnUrl = $"{returnUrl}/order-failed?id={order.Id}";
                
                // Tạo URL callback dựa trên cấu hình VNPay (sẽ được sử dụng trong VNP_ReturnUrl)
                var baseReturnUrl = $"{returnUrl}{_vnpReturnUrl}";

                // Create dictionary for all VNPay parameters
                var vnpParams = new Dictionary<string, string>
                {
                    {"vnp_Version", _vnpVersion},
                    {"vnp_Command", _vnpCommand},
                    {"vnp_TmnCode", _vnpTmnCode},
                    {"vnp_Amount", totalPrice},
                    {"vnp_CurrCode", "VND"},
                    {"vnp_TxnRef", vnpTxnRef},
                    {"vnp_OrderInfo", order.Id.ToString()},
                    {"vnp_OrderType", _orderType},
                    {"vnp_Locale", "vn"},
                    {"vnp_ReturnUrl", baseReturnUrl},
                    {"vnp_IpAddr", "127.0.0.1"}  // Should ideally be client IP
                };

                // Add time parameters
                var now = DateTime.UtcNow.AddHours(7); // Vietnam timezone
                var createDate = now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                vnpParams.Add("vnp_CreateDate", createDate);

                var expireDate = now.AddMinutes(15).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                vnpParams.Add("vnp_ExpireDate", expireDate);

                // Sort the parameters to ensure correct hash generation
                var fieldNames = vnpParams.Keys.ToList();
                fieldNames.Sort();

                // Build hash data and query
                var hashData = new StringBuilder();
                var query = new StringBuilder();

                foreach (var fieldName in fieldNames)
                {
                    var fieldValue = vnpParams[fieldName];
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        hashData.Append(Uri.EscapeDataString(fieldName) + "=" + Uri.EscapeDataString(fieldValue));
                        query.Append(Uri.EscapeDataString(fieldName) + "=" + Uri.EscapeDataString(fieldValue));
                        
                        if (fieldNames.IndexOf(fieldName) < fieldNames.Count - 1)
                        {
                            hashData.Append("&");
                            query.Append("&");
                        }
                    }
                }

                // Generate secure hash
                var secureHash = HmacSHA512(_vnpHashSecret, hashData.ToString());
                var queryUrl = query.ToString() + "&vnp_SecureHash=" + secureHash;

                // Update order status to awaiting payment
                order.Status = OrderStatus.AwaitingPayment;
                _orderRepository.Update(order);
                await _orderRepository.SaveAsync();

                // Log created payment URL for debugging
                var fullPaymentUrl = _vnpUrl + "?" + queryUrl;
                _logger.LogInformation($"[VNPay] Generated payment URL: {fullPaymentUrl}");
                _logger.LogInformation($"[VNPay] Return URL: {baseReturnUrl}");
                _logger.LogInformation($"[VNPay] Success URL: {successReturnUrl}");
                _logger.LogInformation($"[VNPay] Failure URL: {failureReturnUrl}");
                _logger.LogInformation($"[VNPay] Order ID: {order.Id}, User ID: {userId}");

                // Return the full payment URL
                return Result<string>.Success(fullPaymentUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating VNPay payment URL");
                return Result<string>.Failure(new ErrorResult 
                { 
                    Code = "VNPayError",
                    Description = $"Error generating payment URL: {ex.Message}"
                });
            }
        }

        public async Task<Result<VNPayResponse>> ProcessPaymentCallbackAsync(VNPayRequest request)
        {
            try
            {
                _logger.LogInformation($"[VNPay] Processing payment callback for order: {request.VnpOrderInfo}");
                _logger.LogInformation($"[VNPay] Response code: {request.VnpResponseCode}, Transaction status: {request.VnpTransactionStatus}");
                
                var response = new VNPayResponse
                {
                    IsSuccessful = false,
                    OrderId = request.VnpOrderInfo
                };

                // Validate the request parameters
                if (string.IsNullOrEmpty(request.VnpOrderInfo) || !Guid.TryParse(request.VnpOrderInfo, out var orderId))
                {
                    response.Message = "Invalid order information";
                    response.RedirectUrl = "/order-failed";
                    return Result<VNPayResponse>.Success(response);
                }

                if (string.IsNullOrEmpty(request.VnpAmount) || !double.TryParse(request.VnpAmount, out _))
                {
                    response.Message = "Invalid payment amount";
                    response.RedirectUrl = "/order-failed";
                    return Result<VNPayResponse>.Success(response);
                }

                // Get order information
                var order = await _repositoryManager.Order.FindByCondition(
                    o => o.Id == orderId, 
                    true
                ).FirstOrDefaultAsync();

                if (order == null)
                {
                    response.Message = "Order not found";
                    response.RedirectUrl = "/order-failed";
                    return Result<VNPayResponse>.Success(response);
                }

                // Build security validation parameters
                var validationFields = new Dictionary<string, string>
                {
                    {"vnp_Amount", request.VnpAmount ?? string.Empty},
                    {"vnp_BankCode", request.VnpBankCode ?? string.Empty},
                    {"vnp_BankTranNo", request.VnpBankTranNo ?? string.Empty},
                    {"vnp_CardType", request.VnpCardType ?? string.Empty},
                    {"vnp_OrderInfo", request.VnpOrderInfo ?? string.Empty},
                    {"vnp_PayDate", request.VnpPayDate ?? string.Empty},
                    {"vnp_ResponseCode", request.VnpResponseCode ?? string.Empty},
                    {"vnp_TmnCode", request.VnpTmnCode ?? string.Empty},
                    {"vnp_TransactionNo", request.VnpTransactionNo ?? string.Empty},
                    {"vnp_TransactionStatus", request.VnpTransactionStatus ?? string.Empty},
                    {"vnp_TxnRef", request.VnpTxnRef ?? string.Empty}
                };

                // Generate security hash for validation
                var calculatedHash = HashAllFields(validationFields);
                _logger.LogInformation($"[VNPay] Calculated hash: {calculatedHash}");
                _logger.LogInformation($"[VNPay] Received hash: {request.VnpSecureHash}");
                
                // Validate the hash
                if (calculatedHash.Equals(request.VnpSecureHash, StringComparison.OrdinalIgnoreCase))
                {
                    // Success transaction
                    if (request.VnpResponseCode == "00" && request.VnpTransactionStatus == "00")
                    {
                        // Update order status to processing/paid
                        order.Status = OrderStatus.Processing;
                        _orderRepository.Update(order);
                        await _orderRepository.SaveAsync();

                        response.IsSuccessful = true;
                        response.Message = "Payment successful";
                        response.RedirectUrl = $"/order-success/{order.Id}";
                        response.PaymentStatus = "Successful";
                        response.TransactionId = request.VnpTransactionNo;
                    }
                    else
                    {
                        // Failed transaction - keep status as AwaitingPayment
                        order.Status = OrderStatus.AwaitingPayment;
                        _orderRepository.Update(order);
                        await _orderRepository.SaveAsync();

                        response.Message = "Payment failed";
                        response.RedirectUrl = $"/order-failed?id={order.Id}";
                        response.PaymentStatus = "Failed";
                    }
                }
                else
                {
                    // Invalid security hash
                    _logger.LogWarning($"[VNPay] Invalid hash for order {request.VnpOrderInfo}. Calculated: {calculatedHash}, Received: {request.VnpSecureHash}");
                    
                    order.Status = OrderStatus.AwaitingPayment;
                    _orderRepository.Update(order);
                    await _orderRepository.SaveAsync();

                    response.Message = "Invalid payment verification";
                    response.RedirectUrl = $"/order-failed?id={order.Id}";
                    response.PaymentStatus = "Failed";
                }

                return Result<VNPayResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[VNPay] Error processing payment callback: {ex.Message}");
                var response = new VNPayResponse
                {
                    IsSuccessful = false,
                    Message = $"Error processing payment: {ex.Message}",
                    RedirectUrl = "/order-failed",
                    PaymentStatus = "Error"
                };
                return Result<VNPayResponse>.Success(response);
            }
        }

        // Calculate HMAC-SHA512 hash
        private string HmacSHA512(string key, string data)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                    return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        // Calculate hash for all fields for security validation
        private string HashAllFields(Dictionary<string, string> fields)
        {
            // Sort field names alphabetically
            var sortedFieldNames = fields.Keys.OrderBy(k => k).ToList();
            var sb = new StringBuilder();

            foreach (var fieldName in sortedFieldNames)
            {
                if (fields.TryGetValue(fieldName, out var fieldValue) && !string.IsNullOrEmpty(fieldValue))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append("&");
                    }
                    sb.Append(fieldName);
                    sb.Append("=");
                    sb.Append(Uri.EscapeDataString(fieldValue));
                }
            }

            return HmacSHA512(_vnpHashSecret, sb.ToString());
        }

        // Generate random number for transaction reference
        private string GetRandomNumber(int length)
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
} 