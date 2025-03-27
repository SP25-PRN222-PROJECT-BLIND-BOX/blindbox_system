using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages.Checkout
{
    public partial class Checkout : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        private List<CartItem> _cartItems = new List<CartItem>();
        private CheckoutInfo _checkoutInfo = new CheckoutInfo();
        private decimal Subtotal => _cartItems.Sum(item => item.Price * item.Quantity);
        private decimal ShippingCost => Subtotal > 0 ? 20000 : 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadCartFromLocalStorage();
            await LoadCheckoutInfoFromLocalStorage();
        }

        private async Task LoadCartFromLocalStorage()
        {
            try
            {
                var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                
                if (!string.IsNullOrEmpty(cartJson))
                {
                    _cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
                }
                
                // If no cart in localStorage or it's empty, load sample data for preview
                if (_cartItems.Count == 0 && NavigationManager.Uri.Contains("?sample=true"))
                {
                    LoadSampleData();
                }
            }
            catch (Exception)
            {
                Snackbar.Add("Failed to load cart data", Severity.Error);
                LoadSampleData(); // Load sample data on error for preview
            }
        }

        private async Task LoadCheckoutInfoFromLocalStorage()
        {
            try
            {
                var infoJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_checkout_info");
                
                if (!string.IsNullOrEmpty(infoJson))
                {
                    _checkoutInfo = JsonSerializer.Deserialize<CheckoutInfo>(infoJson) ?? new CheckoutInfo();
                }
                else
                {
                    // Default payment method
                    _checkoutInfo.PaymentMethod = "Cash on Delivery";
                }
            }
            catch (Exception)
            {
                Snackbar.Add("Failed to load checkout info", Severity.Error);
            }
        }

        private void LoadSampleData()
        {
            _cartItems = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    ProductName = "LUCKY EMMA - Emma Secret Forest Masked Ball Blind Box Series",
                    ImageUrl = "/images/shop/emma-forest.jpg",
                    Price = 360000,
                    Quantity = 1
                },
                new CartItem
                {
                    Id = 2,
                    ProductName = "LUCKY EMMA - Alice Fairy Tale Blind Box Series",
                    ImageUrl = "/images/shop/emma-alice.jpg",
                    Price = 240000,
                    Quantity = 1
                }
            };
        }

        private async Task SaveCheckoutInfoToLocalStorage()
        {
            try
            {
                var infoJson = JsonSerializer.Serialize(_checkoutInfo);
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "blindbox_checkout_info", infoJson);
            }
            catch (Exception)
            {
                Snackbar.Add("Failed to save checkout info", Severity.Error);
            }
        }

        private bool IsFormValid()
        {
            return !string.IsNullOrWhiteSpace(_checkoutInfo.FirstName) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.LastName) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.Address) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.Phone) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.Email) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.PaymentMethod);
        }

        private async Task PlaceOrder()
        {
            if (!IsFormValid())
            {
                Snackbar.Add("Vui lòng điền đầy đủ thông tin yêu cầu", Severity.Warning);
                return;
            }

            try
            {
                await SaveCheckoutInfoToLocalStorage();
                
                // Giả lập xử lý đơn hàng
                Snackbar.Add("Đang xử lý đơn hàng...", Severity.Info);
                
                // Tạo đối tượng đơn hàng
                var order = new
                {
                    CustomerName = $"{_checkoutInfo.FirstName} {_checkoutInfo.LastName}",
                    Address = _checkoutInfo.Address,
                    Phone = _checkoutInfo.Phone,
                    Email = _checkoutInfo.Email,
                    PaymentMethod = _checkoutInfo.PaymentMethod,
                    OrderDate = DateTime.Now,
                    Subtotal = Subtotal,
                    ShippingCost = ShippingCost,
                    Total = Subtotal + ShippingCost,
                    Items = _cartItems
                };
                
                // Trong ứng dụng thực tế, bạn sẽ gửi đơn hàng đến API
                // Giả lập đơn hàng thành công và xóa giỏ hàng
                
                // Tạo random OrderId (trong thực tế sẽ nhận từ server)
                var orderId = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
                
                // Xóa giỏ hàng (Không xóa ở đây vì sẽ được xử lý trong trang thành công)
                // await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "blindbox_cart");
                
                // Chuyển hướng đến trang xác nhận đơn hàng
                Snackbar.Add("Đặt hàng thành công!", Severity.Success);
                NavigationManager.NavigateTo($"/order-success/{orderId}");
                
                // Giả lập một số trường hợp lỗi (chỉ để demo)
                // Hiện tại luôn thành công, nhưng trong thực tế có thể thất bại
                // Nếu thất bại: NavigationManager.NavigateTo("/order-failed?error=payment");
            }
            catch (Exception)
            {
                Snackbar.Add("Đặt hàng thất bại. Vui lòng thử lại.", Severity.Error);
                NavigationManager.NavigateTo("/order-failed");
            }
        }

        private string FormatPrice(decimal price)
        {
            return $"{price:N0}₫";
        }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class CheckoutInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; }
    }
} 