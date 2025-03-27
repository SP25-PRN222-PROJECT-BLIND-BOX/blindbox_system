using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using BlindBoxShop.Application.Components.Layout;

namespace BlindBoxShop.Application.Pages
{
    public partial class OrderSuccess : ComponentBase
    {
        [Parameter]
        public string OrderId { get; set; }
        
        private string PaymentMethod { get; set; } = "Cash on Delivery";
        private List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        private decimal Subtotal { get; set; }
        private decimal ShippingFee { get; set; } = 30000;
        private decimal Total => Subtotal + ShippingFee;
        
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        
        [Inject]
        private ISnackbar Snackbar { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            // Redirect if accessed directly without order ID
            if (string.IsNullOrEmpty(OrderId) || OrderId == "null")
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            
            // Retrieve cart items before clearing
            try
            {
                var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                if (!string.IsNullOrEmpty(cartJson))
                {
                    var cartItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                    if (cartItems != null)
                    {
                        // Convert cart items to order items
                        OrderItems = cartItems.Select(item => new OrderItem
                        {
                            Id = item.Id,
                            ProductName = item.ProductName,
                            ImageUrl = item.ImageUrl,
                            Price = item.Price,
                            Quantity = item.Quantity
                        }).ToList();
                        
                        // Calculate subtotal
                        Subtotal = OrderItems.Sum(item => item.Price * item.Quantity);
                    }
                }
                
                // Clear cart
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "blindbox_cart");
                
                // Get payment method from localStorage if available
                var checkoutInfoJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_checkout_info");
                if (!string.IsNullOrEmpty(checkoutInfoJson))
                {
                    // Could parse for payment method if needed
                }
            }
            catch (Exception)
            {
                // Fallback to empty order for display
                OrderItems = new List<OrderItem>();
            }
        }
        
        // Format price with Vietnamese currency
        private string FormatPrice(decimal price)
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0}", price) + " ₫";
        }
    }
    
    public class OrderItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
} 