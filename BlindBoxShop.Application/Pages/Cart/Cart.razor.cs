using BlindBoxShop.Entities.Models;
using Microsoft.AspNetCore.Components;
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
        private decimal ShippingCost => Subtotal > 0 ? 20000 : 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadCartFromLocalStorage();
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

        private void LoadSampleData()
        {
            _cartItems = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    ProductName = "LUCKY EMMA - Emma Secret Forest Masked Ball Blind Box Series",
                    Description = "Blind Box Series",
                    ImageUrl = "/images/shop/emma-forest.jpg",
                    Price = 360000,
                    Quantity = 1
                },
                new CartItem
                {
                    Id = 2,
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
                Snackbar.Add("Failed to update cart", Severity.Error);
            }
        }

        private async Task IncreaseQuantity(int itemId)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.Quantity++;
                await UpdateLocalStorage();
                Snackbar.Add("Quantity updated", Severity.Success);
            }
        }

        private async Task DecreaseQuantity(int itemId)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
                await UpdateLocalStorage();
                Snackbar.Add("Quantity updated", Severity.Success);
            }
        }

        private async Task RemoveItem(int itemId)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _cartItems.Remove(item);
                await UpdateLocalStorage();
                Snackbar.Add("Item removed from cart", Severity.Success);
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
} 