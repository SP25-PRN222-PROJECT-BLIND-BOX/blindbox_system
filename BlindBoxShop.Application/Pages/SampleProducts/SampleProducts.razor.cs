using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using BlindBoxShop.Application.Models;

namespace BlindBoxShop.Application.Pages.SampleProducts
{
    public partial class SampleProducts : ComponentBase, IDisposable
    {
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        private List<ProductItem> _sampleProducts = new List<ProductItem>();
        private bool _showAlert = false;
        private System.Timers.Timer _alertTimer;

        protected override void OnInitialized()
        {
            _alertTimer = new System.Timers.Timer(3000);
            _alertTimer.Elapsed += HideAlert;
            _alertTimer.AutoReset = false;

            LoadSampleProducts();
        }

        private void LoadSampleProducts()
        {
            _sampleProducts = new List<ProductItem>
            {
                new ProductItem
                {
                    Id = 1,
                    Name = "LUCKY EMMA - Emma Secret Forest Masked Ball",
                    Description = "Limited Edition Blind Box Series",
                    ImageUrl = "https://lzd-img-global.slatic.net/g/p/e91f4b5bea5b4c3398b78d4c25f98d7e.jpg_720x720q80.jpg",
                    Price = 360000
                },
                new ProductItem
                {
                    Id = 2,
                    Name = "LUCKY EMMA - Alice Fairy Tale",
                    Description = "Fairy Tale Collection Blind Box Series",
                    ImageUrl = "https://lzd-img-global.slatic.net/g/p/e5f5ef74e0e0c8e8da59992eaa8ef1cf.png_720x720q80.png",
                    Price = 240000
                },
                new ProductItem
                {
                    Id = 3,
                    Name = "DIMOO Secret Garden",
                    Description = "World Series Blind Box Collection",
                    ImageUrl = "https://m.media-amazon.com/images/I/51GUT9AWDPL._AC_UF1000,1000_QL80_.jpg",
                    Price = 320000
                },
                new ProductItem
                {
                    Id = 4,
                    Name = "Tiny Bunny Space Adventure",
                    Description = "Cosmic Collection Limited Edition",
                    ImageUrl = "https://www.westfield.co.nz/on/demandware.static/-/Sites/en_NZ/dw4a626073/shop/miniso/miniso-product-2.jpg",
                    Price = 280000
                },
                new ProductItem
                {
                    Id = 5,
                    Name = "SKULLPANDA Urban Series",
                    Description = "Street Fashion Blind Box Collection",
                    ImageUrl = "https://cf.shopee.com.my/file/e4b0eac12859d018da8e04ec96af1a1c",
                    Price = 390000
                },
                new ProductItem
                {
                    Id = 6,
                    Name = "PUCKY Ocean Babies",
                    Description = "Sea Creatures Blind Box Series",
                    ImageUrl = "https://cdn.shopify.com/s/files/1/0416/0993/6333/files/Ocean_Babies_Series_Blind_Box_-_Preorder_10_1024x1024.jpg",
                    Price = 260000
                },
                new ProductItem
                {
                    Id = 7,
                    Name = "MOLLY Zodiac Collection",
                    Description = "12 Zodiac Signs Blind Box Series",
                    ImageUrl = "https://cdn-amz.woka.io/images/I/61SqElbLgQL.jpg",
                    Price = 350000
                },
                new ProductItem
                {
                    Id = 8,
                    Name = "LABUBU Winter Dreams",
                    Description = "Limited Winter Edition Blind Box",
                    ImageUrl = "https://cdn.shopify.com/s/files/1/0554/5047/2579/shop/HTB1DmuhX5frK1RjSspbq6A4pFXaf_41da44d4-1a77-40e5-b0ba-c270ca2951d0.jpg",
                    Price = 420000
                }
            };
        }

        private async Task AddToCart(ProductItem product)
        {
            try
            {
                var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                List<CartItem> cartItems = new List<CartItem>();

                if (!string.IsNullOrEmpty(cartJson))
                {
                    cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
                }

                // Convert product Id to Guid to use as BlindBoxId (for sample products)
                Guid productGuid = Guid.NewGuid(); // Generate a unique ID for sample products
                
                // First check by BlindBoxId for consistency with other components
                var existingItem = cartItems.Find(item => item.BlindBoxId == productGuid);
                
                // If not found, check by Id as fallback (for backward compatibility)
                if (existingItem == null)
                {
                    existingItem = cartItems.Find(item => item.Id == product.Id);
                }

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cartItems.Add(new CartItem
                    {
                        Id = product.Id,
                        BlindBoxId = productGuid, // Set the BlindBoxId for consistency
                        ProductName = product.Name,
                        Description = product.Description,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = 1
                    });
                }

                var updatedCartJson = JsonSerializer.Serialize(cartItems);
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "blindbox_cart", updatedCartJson);

                ShowSuccessAlert();
                Snackbar.Add($"Đã thêm {product.Name} vào giỏ hàng!", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi khi thêm vào giỏ hàng: {ex.Message}", Severity.Error);
            }
        }

        private void ShowSuccessAlert()
        {
            _showAlert = true;
            _alertTimer.Stop();
            _alertTimer.Start();
            StateHasChanged();
        }

        private void HideAlert(object sender, ElapsedEventArgs e)
        {
            _showAlert = false;
            InvokeAsync(StateHasChanged);
        }

        private string FormatPrice(decimal price)
        {
            return $"{price:N0}₫";
        }

        public void Dispose()
        {
            _alertTimer?.Dispose();
        }
    }

    public class ProductItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public Guid BlindBoxId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
} 