using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.JSInterop;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BlindBoxShop.Application.Pages.Pages
{
    public partial class BlindBoxDetail : ComponentBase, IDisposable
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

        [Inject]
        private IDialogService DialogService { get; set; } = default!;

        private BlindBoxDto BlindBox { get; set; }
        private List<BlindBoxDto> _relatedProducts = new();
        private List<string> _images = new();
        private List<string> _itemImages = new(); // Images of items inside the BlindBox
        private List<string> _animationImages = new(); // Images for animation
        private Dictionary<Guid, string> _relatedProductImages = new();
        private int _quantity = 1;
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private bool _gachaDialogVisible = false;
        private bool _gachaResultDialogVisible = false;
        private bool _isGachaAnimationComplete = false;
        private bool _acceptTerms = false;
        private int _currentAnimationIndex = 0;

        // User info fields for order creation
        private string _firstName = "";
        private string _lastName = "";
        private string _address = "";
        private string _province = "";
        private string _ward = "";
        private string _phone = "";

        // Animation timer
        private Timer _animationTimer;

        // Fixed shipping price
        private readonly decimal _shippingPrice = 20000;

        // Getting total price including shipping
        private decimal TotalPrice => (BlindBox?.CurrentPrice ?? 0) + _shippingPrice;

        // Mock result item data
        private class MockResultItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public int Rarity { get; set; }
            public bool IsSecret { get; set; }
        }

        private MockResultItem _resultItem;

        // Dialog options
        private DialogOptions _dialogOptions = new()
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            Position = DialogPosition.Center
        };

        // Result dialog options (no close button)
        private DialogOptions _resultDialogOptions = new()
        {
            CloseOnEscapeKey = false,
            CloseButton = false,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            Position = DialogPosition.Center
        };

        // Item preview variables
        private bool _itemPreviewVisible;
        private BlindBoxItemDto _selectedItem;

        // Image preview variables
        private bool _imagePreviewVisible;
        private string _selectedImage;
        private bool _isZoomed;

        protected override async Task OnInitializedAsync()
        {
            await LoadBlindBoxAsync();

            // Load user info if available
            await LoadUserInfoAsync();
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
                _itemImages.Clear();

                var response = await ServiceManager.BlindBoxService.GetBlindBoxByIdAsync(Guid.Parse(id), false);
                if (response.IsSuccess && response.Value != null)
                {
                    BlindBox = response.Value;

                    // Debug info for items
                    if (BlindBox.Items != null)
                    {
                        Console.WriteLine($"Loaded {BlindBox.Items.Count} items for BlindBox {BlindBox.Id}");

                        foreach (var item in BlindBox.Items)
                        {
                            Console.WriteLine($"Item {item.Id} - Name: {item.Name} - ImageUrl before: {item.ImageUrl}");

                            if (!string.IsNullOrWhiteSpace(item.ImageUrl))
                            {
                                // Ensure the URL is properly formatted
                                var imageUrl = item.ImageUrl.Trim();
                                if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
                                {
                                    item.ImageUrl = "/" + imageUrl;
                                }
                            }
                            else
                            {
                                // Set a default placeholder for items without images
                                item.ImageUrl = "/images/placeholder-item.jpg";
                            }

                            Console.WriteLine($"Item {item.Id} - ImageUrl after: {item.ImageUrl}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No items found for BlindBox {BlindBox.Id}");
                    }

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

                    // For online BlindBoxes, load the items
                    if (BlindBox.Probability > 0)
                    {
                        await LoadItemImagesAsync();
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

        private async Task LoadItemImagesAsync()
        {
            try
            {
                // Ensure we clear the list before populating it
                _itemImages.Clear();

                // Try to load actual BlindBoxItems from service
                // This is the actual API call to get the BlindBoxItems
                using var blindBoxItemService = ServiceManager.BlindBoxItemService;
                var itemsResult = await blindBoxItemService.GetItemsByBlindBoxIdAsync(BlindBox.Id, false);

                if (itemsResult.IsSuccess && itemsResult.Value?.Any() == true)
                {
                    // Add all item images from the API response
                    foreach (var item in itemsResult.Value)
                    {
                        if (!string.IsNullOrWhiteSpace(item.ImageUrl))
                        {
                            // Ensure the URL is properly formatted
                            var imageUrl = item.ImageUrl.Trim();
                            if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
                            {
                                imageUrl = "/" + imageUrl;
                            }
                            _itemImages.Add(imageUrl);
                        }
                    }

                    Console.WriteLine($"Loaded {_itemImages.Count} BlindBoxItems from service for BlindBox {BlindBox.Id}");
                }
                else
                {
                    Console.WriteLine($"No BlindBoxItems found in service, using fallbacks for BlindBox {BlindBox.Id}");
                }

                // If we couldn't get actual items or there were none, use fallbacks
                if (!_itemImages.Any())
                {
                    // Use BlindBox's own images as items if available
                    if (_images.Any())
                    {
                        // First add all images of the BlindBox itself
                        _itemImages.AddRange(_images);
                    }

                    // Add images from related products if available and needed
                    if (_relatedProductImages.Any() && _itemImages.Count < 6)
                    {
                        foreach (var img in _relatedProductImages.Values)
                        {
                            if (!_itemImages.Contains(img))
                            {
                                _itemImages.Add(img);
                                if (_itemImages.Count >= 10) break; // Limit to 10 images
                            }
                        }
                    }
                }

                // Always ensure we have at least 5 items for display
                // If we still don't have enough images, use standard placeholders
                if (_itemImages.Count < 5)
                {
                    string[] placeholderImages =
                    {
                        "/images/box-placeholder.jpg",
                        "/images/box-placeholder-alt.jpg",
                        "/images/online-box-placeholder.jpg",
                        "/images/physical-box-placeholder.jpg",
                        "/images/placeholder-item.jpg"
                    };

                    // Add placeholder images if needed
                    foreach (var img in placeholderImages)
                    {
                        if (!_itemImages.Contains(img))
                        {
                            _itemImages.Add(img);
                            if (_itemImages.Count >= 10) break; // Limit to 10 images
                        }
                    }
                }

                // Log the image count for debugging
                Console.WriteLine($"Final count: {_itemImages.Count} item images for BlindBox {BlindBox.Id}");

                // Build animation images - for gacha animation
                _animationImages.Clear();
                _animationImages.AddRange(_itemImages);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading BlindBoxItems: {ex.Message}");

                // Fallback to ensure we always have at least one image
                if (!_itemImages.Any())
                {
                    _itemImages.Add("/images/box-placeholder.jpg");
                    _animationImages.Add("/images/box-placeholder.jpg");
                }
            }
        }

        private async Task LoadUserInfoAsync()
        {
            try
            {
                // Try to get user info from localStorage as fallback
                var firstName = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_firstName");
                var lastName = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_lastName");
                var address = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_address");
                var province = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_province");
                var ward = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_ward");
                var phone = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_phone");

                if (!string.IsNullOrEmpty(firstName)) _firstName = firstName;
                if (!string.IsNullOrEmpty(lastName)) _lastName = lastName;
                if (!string.IsNullOrEmpty(address)) _address = address;
                if (!string.IsNullOrEmpty(province)) _province = province;
                if (!string.IsNullOrEmpty(ward)) _ward = ward;
                if (!string.IsNullOrEmpty(phone)) _phone = phone;

                Console.WriteLine("Loaded user info from localStorage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user info: {ex.Message}");
                // Don't show an error to the user
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
                            Rarity = BlindBox.Rarity
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

        // Open the gacha dialog
        private void OpenGachaDialog()
        {
            try
            {
                if (BlindBox == null || BlindBox.Status != BlindBoxStatus.Available) return;

                // Điều hướng đến trang BlindBoxGacha
                NavigationManager.NavigateTo($"/blindbox-gacha/{BlindBox.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to gacha page: {ex.Message}");
                Snackbar.Add($"Lỗi khi mở trang quay gacha: {ex.Message}", Severity.Error);
            }
        }

        // Close the gacha dialog
        private void CloseGachaDialog()
        {
            _gachaDialogVisible = false;
        }

        // Process the gacha draw
        private async Task ProcessGacha()
        {
            if (BlindBox == null || !_acceptTerms) return;

            try
            {
                _gachaDialogVisible = false;
                await Task.Delay(500); // Wait for dialog to close

                // Check if we have enough item images for a good animation experience
                if (_itemImages.Count < 3)
                {
                    // Reload item images if needed
                    await LoadItemImagesAsync();

                    // If still not enough, generate some placeholder images
                    if (_itemImages.Count < 3)
                    {
                        for (int i = _itemImages.Count; i < 5; i++)
                        {
                            _itemImages.Add($"/images/box-placeholder.jpg");
                        }
                        _animationImages.Clear();
                        _animationImages.AddRange(_itemImages);
                    }
                }

                // Shuffle animation images for better animation effect
                _animationImages = _animationImages.OrderBy(x => Guid.NewGuid()).ToList();

                // Show the animation dialog
                _gachaResultDialogVisible = true;
                _isGachaAnimationComplete = false;

                // Start animation
                _animationTimer = new Timer(100); // Animation speed
                _animationTimer.Elapsed += AnimationTimerElapsed;
                _animationTimer.AutoReset = true;
                _animationTimer.Start();

                // Simulate drawing process (3 seconds)
                await Task.Delay(3000);

                // Get result
                _resultItem = await DrawMockGachaItemAsync();

                // Stop animation
                _animationTimer.Stop();
                _isGachaAnimationComplete = true;

                // Create mock order
                await SimulateCreateOrderAsync();

                // Force UI update
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
                _gachaResultDialogVisible = false;
            }
        }

        private async Task<MockResultItem> DrawMockGachaItemAsync()
        {
            // Simulate drawing an item based on rarity probabilities
            // Rarity distribution: Common (50%), Uncommon (30%), Rare (15%), Epic (4%), Legendary (1%)
            Random random = new Random();
            int roll = random.Next(1, 101);

            int rarity;
            bool isSecret = false;

            if (roll <= 50) // 50%
            {
                rarity = 0; // Common
            }
            else if (roll <= 80) // 30%
            {
                rarity = 1; // Uncommon
            }
            else if (roll <= 95) // 15%
            {
                rarity = 2; // Rare
            }
            else if (roll <= 99) // 4%
            {
                rarity = 3; // Epic
            }
            else // 1%
            {
                rarity = 4; // Legendary
                isSecret = true;
            }

            // Use the probability to increase chance of secret items
            if (BlindBox.Probability >= 10 && random.Next(1, 101) <= BlindBox.Probability)
            {
                rarity = Math.Max(rarity, 3); // At least Epic

                if (random.Next(1, 101) <= BlindBox.Probability / 2)
                {
                    rarity = 4; // Legendary
                    isSecret = true;
                }
            }

            // Build item name and description based on rarity
            string[] rarityNames = { "Common", "Uncommon", "Rare", "Epic", "Legendary" };
            string[] itemTypes = { "Figurine", "Keychain", "Pin", "Plushie", "Collectible" };

            string name = $"{rarityNames[rarity]} {BlindBox.Name} {itemTypes[random.Next(itemTypes.Length)]}";
            string description = isSecret
                ? $"A secret {rarityNames[rarity].ToLower()} item from the {BlindBox.Name} collection. This item is extremely rare and sought after by collectors!"
                : $"A {rarityNames[rarity].ToLower()} item from the {BlindBox.Name} collection. {(rarity >= 2 ? "Quite a lucky find!" : "A nice addition to your collection!")}";

            // Select the appropriate image for the item based on rarity
            // Group images by likely rarity (assuming first items are common, later ones are rare)
            string imageUrl;

            if (_itemImages.Count > 0)
            {
                // Distribute the images across rarity levels
                int imageCount = _itemImages.Count;
                int imagesPerRarity = Math.Max(1, imageCount / 5); // At least 1 image per rarity

                // Calculate starting index for the selected rarity
                int startIndex = rarity * imagesPerRarity;
                int endIndex = Math.Min((rarity + 1) * imagesPerRarity - 1, imageCount - 1);

                // Select a random image within the rarity range
                if (startIndex <= endIndex)
                {
                    int selectedIndex = random.Next(startIndex, endIndex + 1);
                    imageUrl = _itemImages[selectedIndex];
                }
                else
                {
                    // Fallback if indexes are invalid
                    imageUrl = _itemImages[random.Next(imageCount)];
                }
            }
            else
            {
                // Fallback placeholder
                imageUrl = "/images/box-placeholder.jpg";
            }

            return new MockResultItem
            {
                Name = name,
                Description = description,
                ImageUrl = imageUrl,
                Rarity = rarity,
                IsSecret = isSecret
            };
        }

        private async Task SimulateCreateOrderAsync()
        {
            // In a real application, this would create an actual order in the database
            // For now, we'll just simulate it by showing a snackbar

            await Task.Delay(1000); // Simulate API call

            // Build message
            string message = $"Order created successfully! Your {_resultItem.Name} will be shipped to {_address}.";
            Snackbar.Add(message, Severity.Success);
        }

        private void AnimationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Use Invoke to ensure UI updates on the UI thread
            InvokeAsync(() =>
            {
                // Cycle through animation images
                _currentAnimationIndex = (_currentAnimationIndex + 1) % _animationImages.Count;
                StateHasChanged();
            });
        }

        private async Task CloseResultDialog()
        {
            _gachaResultDialogVisible = false;

            // Reset state
            _isGachaAnimationComplete = false;
            _resultItem = null;

            // Clear temporary form data
            _firstName = "";
            _lastName = "";
            _address = "";
            _province = "";
            _ward = "";
            _phone = "";
            _acceptTerms = false;

            // Allow a moment for the dialog to close before navigating
            await Task.Delay(300);

            // Optional: Navigate to a thank you or order confirmation page
            // NavigationManager.NavigateTo("/order-confirmation");
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

        private Color GetItemRarityColor(int rarity)
        {
            return rarity switch
            {
                0 => Color.Success,  // Common
                1 => Color.Info,     // Uncommon
                2 => Color.Warning,  // Rare
                3 => Color.Error,    // Epic
                _ => Color.Default
            };
        }

        private string GetRarityText(int rarity)
        {
            return rarity switch
            {
                0 => "Phổ biến",
                1 => "Không phổ biến",
                2 => "Hiếm",
                3 => "Siêu hiếm",
                4 => "Huyền thoại",
                _ => "Không xác định"
            };
        }

        public bool IsNewProduct(DateTime createdAt)
        {
            // Consider a product new if it was created within the last 14 days
            return (DateTime.Now - createdAt).TotalDays <= 14;
        }

        public void Dispose()
        {
            _animationTimer?.Dispose();
        }

        // Process the gacha draw with VNPay
        private async Task ProcessGachaWithVNPay()
        {
            if (BlindBox == null || !_acceptTerms) return;

            try
            {
                _isProcessing = true;

                // Validate shipping information
                if (string.IsNullOrWhiteSpace(_firstName) ||
                    string.IsNullOrWhiteSpace(_lastName) ||
                    string.IsNullOrWhiteSpace(_address) ||
                    string.IsNullOrWhiteSpace(_province) ||
                    string.IsNullOrWhiteSpace(_phone))
                {
                    Snackbar.Add("Vui lòng điền đầy đủ thông tin giao hàng", Severity.Warning);
                    _isProcessing = false;
                    return;
                }

                // Create a simplified order data object
                var orderData = new
                {
                    Status = BlindBoxShop.Shared.Enum.OrderStatus.AwaitingPayment,
                    PaymentMethod = BlindBoxShop.Shared.Enum.PaymentMethod.VnPay,
                    CustomerName = $"{_firstName} {_lastName}",
                    Phone = _phone,
                    Address = _address,
                    Province = _province,
                    Wards = _ward,
                    ShippingFee = _shippingPrice,
                    SubTotal = BlindBox.CurrentPrice,
                    Total = TotalPrice,
                    BlindBoxId = BlindBox.Id,
                    Quantity = 1
                };

                // Store order data in localStorage for reference
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "gacha_order_data",
                    System.Text.Json.JsonSerializer.Serialize(orderData));

                // Create a mock order ID for testing
                var orderId = Guid.NewGuid();
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "gacha_order_id", orderId.ToString());
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "gacha_blindbox_id", BlindBox.Id.ToString());

                // Close the confirmation dialog
                _gachaDialogVisible = false;

                // Get base URL for the VNPay redirect
                var baseUrl = NavigationManager.BaseUri.TrimEnd('/');

                // For demonstration, navigate to a mock VNPay URL
                Snackbar.Add("Đang chuyển đến cổng thanh toán VNPay...", Severity.Info);

                // Navigate to the mock VNPay URL
                var mockVnpayUrl = $"{baseUrl}/payment/vnpay?orderId={orderId}&amount={orderData.Total * 100}";
                NavigationManager.NavigateTo(mockVnpayUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing VNPay payment: {ex.Message}");
                Snackbar.Add($"Lỗi xử lý thanh toán: {ex.Message}", Severity.Error);
                _isProcessing = false;
            }
        }

        // Item preview methods
        private void OpenItemPreview(BlindBoxItemDto item)
        {
            _selectedItem = item;
            _itemPreviewVisible = true;
        }

        private void CloseItemPreview()
        {
            _itemPreviewVisible = false;
        }

        // Image preview methods
        private void OpenImagePreview(string imageUrl)
        {
            Console.WriteLine($"Opening image preview for URL: {imageUrl}");

            try
            {
                _selectedImage = imageUrl;
                _imagePreviewVisible = true;

                // Force UI update
                StateHasChanged();

                Console.WriteLine("Image preview dialog should be visible now");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OpenImagePreview: {ex.Message}");
            }
        }

        private void CloseImagePreview()
        {
            Console.WriteLine("Closing image preview");
            _imagePreviewVisible = false;
            _isZoomed = false;

            // Force UI update
            StateHasChanged();
        }

        private void NavigatePreviewImage(int direction)
        {
            if (_images.Count <= 1) return;

            var currentIndex = _images.IndexOf(_selectedImage);
            if (currentIndex < 0) currentIndex = 0;

            var newIndex = (currentIndex + direction) % _images.Count;
            if (newIndex < 0) newIndex = _images.Count - 1;

            _selectedImage = _images[newIndex];
            _isZoomed = false;
        }

        private void ToggleZoom()
        {
            _isZoomed = !_isZoomed;
        }

        private void HandleKeyDown(KeyboardEventArgs e)
        {
            if (!_imagePreviewVisible) return;

            switch (e.Key)
            {
                case "ArrowLeft":
                    NavigatePreviewImage(-1);
                    break;
                case "ArrowRight":
                    NavigatePreviewImage(1);
                    break;
                case "Escape":
                    CloseImagePreview();
                    break;
            }
        }
    }
}