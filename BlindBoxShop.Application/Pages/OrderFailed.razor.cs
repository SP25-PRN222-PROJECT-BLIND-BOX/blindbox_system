using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages
{
    public partial class OrderFailed : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "id")]
        public string QueryId { get; set; }

        public string ErrorMessage { get; set; } = "There was a problem processing your payment. Your order could not be completed.";
        public List<string> PossibleCauses { get; set; } = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // If Id is not set from route but is available in query, use that
                if (string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(QueryId))
                {
                    Id = QueryId;
                }

                // Get error message from localStorage if available
                var errorFromStorage = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "payment_error");
                if (!string.IsNullOrEmpty(errorFromStorage))
                {
                    ErrorMessage = errorFromStorage;
                    await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "payment_error");
                }

                // Set possible causes based on the error message
                SetPossibleCauses();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing OrderFailed page: {ex.Message}");
            }
        }

        private void SetPossibleCauses()
        {
            PossibleCauses = new List<string>
            {
                "Thông tin thanh toán không chính xác hoặc không đầy đủ",
                "Không thể kết nối đến cổng thanh toán",
                "Tài khoản hoặc thẻ của bạn không đủ số dư",
                "Giao dịch bị từ chối bởi ngân hàng hoặc nhà cung cấp dịch vụ thanh toán"
            };

            // Add specific causes based on error message
            if (ErrorMessage.Contains("VNPay", StringComparison.OrdinalIgnoreCase))
            {
                PossibleCauses.Add("Kết nối với cổng thanh toán VNPay không thành công");
            }
            else if (ErrorMessage.Contains("timeout", StringComparison.OrdinalIgnoreCase))
            {
                PossibleCauses.Add("Giao dịch bị hết thời gian chờ");
            }
        }
    }
} 