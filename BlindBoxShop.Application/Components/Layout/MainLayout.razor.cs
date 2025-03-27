using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Timers;
using System.Linq;

namespace BlindBoxShop.Application.Components.Layout
{
    public partial class MainLayout
    {
        private bool _drawerOpen = false;
        private string _selectedCategory = "All";
        private List<CartItem> _cartItems = new List<CartItem>();
        private int _cartItemCount = 0;
        private System.Timers.Timer _cartCheckTimer;
        
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        protected override void OnInitialized()
        {
            _cartCheckTimer = new System.Timers.Timer(2000); // Kiểm tra mỗi 2 giây
            _cartCheckTimer.Elapsed += CheckCartItemsCount;
            _cartCheckTimer.AutoReset = true;
            _cartCheckTimer.Start();

            // Gọi hàm kiểm tra ngay lập tức
            InvokeAsync(UpdateCartItemCount);
        }

        private async void CheckCartItemsCount(object sender, ElapsedEventArgs e)
        {
            await InvokeAsync(UpdateCartItemCount);
        }

        private async Task UpdateCartItemCount()
        {
            try
            {
                var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                
                if (!string.IsNullOrEmpty(cartJson))
                {
                    var newCartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                    
                    if (newCartItems != null)
                    {
                        _cartItems = newCartItems;
                        var totalCount = _cartItems.Sum(item => item.Quantity);
                        
                        // Chỉ cập nhật UI nếu số lượng sản phẩm thay đổi
                        if (_cartItemCount != totalCount)
                        {
                            _cartItemCount = totalCount;
                            StateHasChanged();
                        }
                    }
                    else
                    {
                        _cartItems = new List<CartItem>();
                        _cartItemCount = 0;
                    }
                }
                else
                {
                    _cartItems = new List<CartItem>();
                    _cartItemCount = 0;
                }
            }
            catch (Exception)
            {
                _cartItems = new List<CartItem>();
                _cartItemCount = 0;
            }
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
        
        private async Task HandleLogout()
        {
            await JSRuntime.InvokeVoidAsync("eval", "document.forms[0].submit();");
        }

        public void Dispose()
        {
            _cartCheckTimer?.Stop();
            _cartCheckTimer?.Dispose();
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
}