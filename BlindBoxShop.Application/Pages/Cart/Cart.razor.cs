using BlindBoxShop.Application.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages.Cart
{
    public partial class Cart : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        private List<CartItem> _cartItems = new List<CartItem>();
        private decimal Subtotal => _cartItems.Sum(item => item.Price * item.Quantity);
        private decimal ShippingCost => Subtotal > 500000 ? 0 : 20000;
        private int _uniqueItemCount => _cartItems.Select(item => item.BlindBoxId).Distinct().Count();

        protected override async Task OnInitializedAsync()
        {
            await LoadCartFromLocalStorage();
            
            // Xử lý các tham số truy vấn URL
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("action", out var action))
            {
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var idValue) 
                    && int.TryParse(idValue, out var id))
                {
                    switch (action)
                    {
                        case "increase":
                            await IncreaseQuantity(id);
                            break;
                        case "decrease":
                            await DecreaseQuantity(id);
                            break;
                        case "remove":
                            await RemoveItem(id);
                            break;
                    }
                    
                    // Chuyển hướng đến trang không có tham số để tránh hành động lặp lại khi làm mới trang
                    NavigationManager.NavigateTo("/cart", false);
                }
            }
        }
        
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return Task.CompletedTask;
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
                Snackbar.Add("Không thể tải giỏ hàng, vui lòng thử lại", Severity.Error);
                LoadSampleData(); // Load sample data on error for preview
            }
        }

        private void LoadSampleData()
        {
            _cartItems = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    BlindBoxId = Guid.NewGuid(), // Generate a random Guid for sample data
                    ProductName = "LUCKY EMMA - Emma Secret Forest Masked Ball Blind Box Series",
                    Description = "Blind Box Series",
                    ImageUrl = "/images/shop/emma-forest.jpg",
                    Price = 360000,
                    Quantity = 1
                },
                new CartItem
                {
                    Id = 2,
                    BlindBoxId = Guid.NewGuid(), // Generate a random Guid for sample data
                    ProductName = "LUCKY EMMA - Alice Fairy Tale Blind Box Series",
                    Description = "Blind Box Series",
                    ImageUrl = "/images/shop/emma-alice.jpg",
                    Price = 240000,
                    Quantity = 1
                }
            };
        }

        private async Task UpdateLocalStorage()
        {
            try
            {
                var cartJson = JsonSerializer.Serialize(_cartItems);
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "blindbox_cart", cartJson);
            }
            catch (Exception)
            {
                Snackbar.Add("Không thể cập nhật giỏ hàng", Severity.Error);
            }
        }

        private async Task IncreaseQuantity(int itemId)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.Quantity++;
                await UpdateLocalStorage();
                Snackbar.Add("Đã cập nhật số lượng", Severity.Success);
                StateHasChanged();
            }
        }

        private async Task DecreaseQuantity(int itemId)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
                await UpdateLocalStorage();
                Snackbar.Add("Đã cập nhật số lượng", Severity.Success);
                StateHasChanged();
            }
        }

        private async Task RemoveItem(int itemId)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _cartItems.Remove(item);
                await UpdateLocalStorage();
                Snackbar.Add("Đã xóa sản phẩm khỏi giỏ hàng", Severity.Success);
                StateHasChanged();
            }
        }

        private string FormatPrice(decimal price)
        {
            return $"{price:N0}₫";
        }
        
        private async Task GoToCheckout()
        {
            try
            {
                // Trực tiếp chuyển hướng đến trang checkout không dùng JavaScript phức tạp
                await JSRuntime.InvokeVoidAsync("blazorExtensions.navigateTo", "/checkout");
            }
            catch
            {
                // Nếu cách trên không hoạt động, sử dụng NavigationManager
                NavigationManager.NavigateTo("/checkout", true);
            }
        }
    }
} 