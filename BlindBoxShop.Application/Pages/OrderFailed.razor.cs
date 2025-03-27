using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages
{
    public partial class OrderFailed : ComponentBase
    {
        [Parameter]
        [SupplyParameterFromQuery(Name = "error")]
        public string ErrorCode { get; set; }
        
        private string ErrorMessage { get; set; } = "Your order could not be processed. Please try again or contact our support team for assistance.";
        private List<string> PossibleCauses { get; set; } = new List<string>();
        
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        
        protected override void OnInitialized()
        {
            // Set default causes
            PossibleCauses = new List<string>
            {
                "Network connection issue",
                "Invalid payment information",
                "Products out of stock",
                "System error"
            };
            
            // Determine error message based on error code
            switch (ErrorCode)
            {
                case "payment":
                    ErrorMessage = "Payment failed. Please check your payment information and try again.";
                    PossibleCauses = new List<string>
                    {
                        "Invalid card details",
                        "Insufficient funds",
                        "Payment gateway error",
                        "Card expired"
                    };
                    break;
                    
                case "stock":
                    ErrorMessage = "One or more products in your cart are currently not available.";
                    PossibleCauses = new List<string>
                    {
                        "Product sold out",
                        "Product temporarily unavailable",
                        "Limited edition item no longer available",
                        "Product removed from the store"
                    };
                    break;
                    
                case "address":
                    ErrorMessage = "Invalid address information. Please check your shipping details.";
                    PossibleCauses = new List<string>
                    {
                        "Incomplete address",
                        "Invalid postal code",
                        "We don't ship to your location",
                        "Address verification failed"
                    };
                    break;
                    
                case "network":
                    ErrorMessage = "Network connection error. Please check your internet connection and try again.";
                    PossibleCauses = new List<string>
                    {
                        "Unstable internet connection",
                        "Request timeout",
                        "Server connection error",
                        "API service unavailable"
                    };
                    break;
                    
                case "timeout":
                    ErrorMessage = "Request has timed out. Please try again later.";
                    PossibleCauses = new List<string>
                    {
                        "Server busy",
                        "Network latency",
                        "Process taking too long",
                        "Payment processing timeout"
                    };
                    break;
                    
                default:
                    // Keep default message and causes
                    break;
            }
        }
    }
} 