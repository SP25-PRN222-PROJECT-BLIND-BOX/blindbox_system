using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.Enum;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.JSInterop;

namespace BlindBoxShop.Application.Pages.Pages
{
    public partial class BlindBoxDetail : ComponentBase
    {
        [Parameter]
        public string id { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private IServiceManager ServiceManager { get; set; } = default!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;
        
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private BlindBoxDto BlindBox { get; set; }
        private List<BlindBoxDto> _relatedProducts = new();
        private List<string> _images = new();
        private Dictionary<Guid, string> _relatedProductImages = new();
        private int _quantity = 1;
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadBlindBoxAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(id))
            {
                await LoadBlindBoxAsync();
            }
        }

        private async Task LoadBlindBoxAsync()
        {
            try
            {
                _isLoading = true;
                
                // Clear images list before populating
                _images.Clear();
                
                var response = await ServiceManager.BlindBoxService.GetBlindBoxByIdAsync(Guid.Parse(id), false);
                if (response.IsSuccess && response.Value != null)
                {
                    BlindBox = response.Value;
                    
                    // Fetch blind box images from database
                    using var blindBoxImageService = ServiceManager.BlindBoxImageService;
                    var blindBoxImages = await blindBoxImageService.GetBlindBoxImagesByBlindBoxIdAsync(BlindBox.Id);
                    
                    // Add all images to carousel
                    if (blindBoxImages?.Value?.Any() == true)
                    {
                        foreach (var image in blindBoxImages.Value)
                        {
                            if (!string.IsNullOrWhiteSpace(image.ImageUrl))
                            {
                                // Ensure the URL is properly formatted
                                var imageUrl = image.ImageUrl.Trim();
                                if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
                                {
                                    imageUrl = "/" + imageUrl;
                                }
                                _images.Add(imageUrl);
                            }
                        }
                    }
                    
                    // If no images found in BlindBoxImages, try using MainImageUrl as fallback
                    if (!_images.Any() && !string.IsNullOrWhiteSpace(BlindBox.MainImageUrl))
                    {
                        var imageUrl = BlindBox.MainImageUrl.Trim();
                        if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
                        {
                            imageUrl = "/" + imageUrl;
                        }
                        _images.Add(imageUrl);
                    }
                    
                    // Add fallback images if still no images were added
                    if (!_images.Any())
                    {
                        // Add category-specific placeholder based on category name
                        string placeholder = !string.IsNullOrEmpty(BlindBox.CategoryName) && BlindBox.CategoryName.ToLower().Contains("physical") 
                            ? "/images/physical-box-placeholder.jpg" 
                            : "/images/box-placeholder.jpg";
                        _images.Add(placeholder);
                        
                        // Add a second image for the carousel
                        _images.Add("/images/box-placeholder-alt.jpg");
                    }
                    
                    // Load related products
                    await LoadRelatedProductsAsync();
                }
                else
                {
                    Snackbar.Add("Không thể tải thông tin BlindBox.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task LoadRelatedProductsAsync()
        {
            try
            {
                if (BlindBox == null) return;
                
                var parameter = new BlindBoxParameter
                {
                    PageSize = 4,
                    OrderBy = "CreatedAt desc"
                };
                
                // Try to get related by category first
                if (BlindBox.BlindBoxCategoryId.HasValue)
                {
                    parameter.CategoryId = BlindBox.BlindBoxCategoryId;
                }
                
                using var blindBoxService = ServiceManager.BlindBoxService;
                var result = await blindBoxService.GetBlindBoxesAsync(parameter, false);
                
                if (result.IsSuccess && result.Value?.Any() == true)
                {
                    // Filter out current product and take up to 4
                    _relatedProducts = result.Value
                        .Where(b => b.Id != BlindBox.Id)
                        .Take(4)
                        .ToList();
                    
                    // If we don't have enough, we can try to get more by rarity
                    if (_relatedProducts.Count < 4)
                    {
                        var rarityParameter = new BlindBoxParameter
                        {
                            PageSize = 8,
                            Rarity = (int)BlindBox.Rarity
                        };
                        
                        var rarityResult = await blindBoxService.GetBlindBoxesAsync(rarityParameter, false);
                        
                        if (rarityResult.IsSuccess && rarityResult.Value?.Any() == true)
                        {
                            var additionalProducts = rarityResult.Value
                                .Where(b => b.Id != BlindBox.Id && !_relatedProducts.Any(rp => rp.Id == b.Id))
                                .Take(4 - _relatedProducts.Count)
                                .ToList();
                            
                            _relatedProducts.AddRange(additionalProducts);
                        }
                    }
                    
                    // Load images for related products
                    await LoadRelatedProductImagesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading related products: {ex.Message}");
                // Don't show error to user for this non-critical feature
            }
        }

        private async Task LoadRelatedProductImagesAsync()
        {
            try
            {
                _relatedProductImages.Clear();

                using var blindBoxImageService = ServiceManager.BlindBoxImageService;
                
                foreach (var product in _relatedProducts)
                {
                    // Get first image from BlindBoxImages table
                    var images = await blindBoxImageService.GetBlindBoxImagesByBlindBoxIdAsync(product.Id);
                    
                    if (images.IsSuccess && images.Value?.Any() == true)
                    {
                        var firstImage = images.Value.First().ImageUrl;
                        
                        // Ensure URL formatting
                        if (!string.IsNullOrWhiteSpace(firstImage))
                        {
                            if (!firstImage.StartsWith("http://") && !firstImage.StartsWith("https://") && !firstImage.StartsWith("/"))
                            {
                                firstImage = "/" + firstImage;
                            }
                            
                            _relatedProductImages[product.Id] = firstImage;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(product.MainImageUrl))
                    {
                        // Fallback to MainImageUrl if no images in BlindBoxImages
                        var imageUrl = product.MainImageUrl;
                        if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
                        {
                            imageUrl = "/" + imageUrl;
                        }
                        
                        _relatedProductImages[product.Id] = imageUrl;
                    }
                    else
                    {
                        // Default placeholder
                        _relatedProductImages[product.Id] = "/images/box-placeholder.jpg";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading related product images: {ex.Message}");
                // Don't show errors for non-critical features
            }
        }

        private void NavigateToProductDetail(Guid productId)
        {
            NavigationManager.NavigateTo($"/blindbox/{productId}");
        }

        private async Task AddToCart()
        {
            if (BlindBox == null || BlindBox.Status != BlindBoxStatus.Available) return;
            
            try
            {
                // Kiểm tra nếu là BlindBox có thể mở online (probability > 0)
                if (BlindBox.Probability > 0)
                {
                    Snackbar.Add("Sản phẩm này chỉ có thể mở trực tuyến, không thể thêm vào giỏ hàng", Severity.Warning);
                    return;
                }
                
                // Regular blind box - add to cart
                // Get existing cart from localStorage
                var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                List<BlindBoxShop.Application.Models.CartItem> cartItems = new();
                
                if (!string.IsNullOrEmpty(cartJson))
                {
                    cartItems = System.Text.Json.JsonSerializer.Deserialize<List<BlindBoxShop.Application.Models.CartItem>>(cartJson) ?? new List<BlindBoxShop.Application.Models.CartItem>();
                }
                
                // Check if item already in cart by BlindBoxId
                var existingItem = cartItems.FirstOrDefault(i => i.BlindBoxId == BlindBox.Id);
                
                if (existingItem != null)
                {
                    // Increase quantity by the selected amount
                    existingItem.Quantity += _quantity;
                }
                else
                {
                    // Add new item
                    cartItems.Add(new BlindBoxShop.Application.Models.CartItem
                    {
                        Id = BlindBox.Id.GetHashCode(),
                        BlindBoxId = BlindBox.Id,
                        ProductName = BlindBox.Name,
                        Description = BlindBox.Description?.Substring(0, Math.Min(50, BlindBox.Description.Length)) + "...",
                        ImageUrl = !string.IsNullOrEmpty(BlindBox.MainImageUrl) ? BlindBox.MainImageUrl : 
                            (_images.Any() ? _images.First() : "/images/box-placeholder.jpg"),
                        Price = BlindBox.CurrentPrice,
                        Quantity = _quantity
                    });
                }
                
                // Save to localStorage
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "blindbox_cart", System.Text.Json.JsonSerializer.Serialize(cartItems));
                
                Snackbar.Add($"Đã thêm {_quantity} sản phẩm vào giỏ hàng", Severity.Success);
                
                // Optionally navigate to cart
                // NavigationManager.NavigateTo("/cart");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi khi thêm vào giỏ hàng: {ex.Message}", Severity.Error);
            }
        }
        
        private void BuyAndOpenOnline()
        {
            if (BlindBox == null || BlindBox.Status != BlindBoxStatus.Available) return;
            
            try
            {
                // Redirect to checkout with special parameter for direct purchase
                NavigationManager.NavigateTo($"/checkout/direct/{BlindBox.Id}?quantity={_quantity}");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi khi mua trực tiếp: {ex.Message}", Severity.Error);
            }
        }

        private string FormatPrice(decimal price)
        {
            return string.Format("{0:n0}", price) + " ₫";
        }

        private Color GetRarityColor(BlindBoxRarity rarity)
        {
            return rarity switch
            {
                BlindBoxRarity.Common => Color.Default,
                BlindBoxRarity.Uncommon => Color.Info,
                BlindBoxRarity.Rare => Color.Primary,
                _ => Color.Default
            };
        }

        public bool IsNewProduct(DateTime createdAt)
        {
            // Consider a product new if it was created within the last 14 days
            return (DateTime.Now - createdAt).TotalDays <= 14;
        }
    }
} 