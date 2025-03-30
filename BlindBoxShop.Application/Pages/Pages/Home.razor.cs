using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages.Pages
{
    // Custom DTO classes for the UI
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class CollectionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class PackageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsFeatured { get; set; } = true;
    }

    public class TestimonialDto
    {
        public string Name { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public partial class Home : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private IServiceManager ServiceManager { get; set; } = default!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;
        
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private List<CategoryDto> _featuredCategories = new();
        private List<BlindBoxDto> _newArrivals = new();
        private List<CollectionDto> _popularCollections = new();
        private List<PackageDto> _featuredPackages = new();
        private List<TestimonialDto> _testimonials = new();
        private Dictionary<Guid, string> _blindBoxImages = new();
        private DateTime _lastUpdateTime = DateTime.Now;
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _isLoading = true;
            try
            {
                await Task.WhenAll(
                    LoadCategoriesAsync(),
                    LoadNewArrivalsAsync(),
                    LoadPopularCollectionsAsync(),
                    LoadFeaturedPackagesAsync()
                );
                
                // If there's a ReviewService available, we could load real testimonials here
                await LoadTestimonialsAsync();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading data: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            var categoryParameter = new BlindBoxCategoryParameter
            {
                PageSize = 6,
                OrderBy = "CreatedAt desc"
            };
            
            using var categoryService = ServiceManager.BlindBoxCategoryService;
            var result = await categoryService.GetBlindBoxCategoriesAsync(categoryParameter, false);
            
            if (result.IsSuccess && result.Value != null)
            {
                var categories = result.Value.ToList();
                
                if (categories.Count > 0)
                {
                    _featuredCategories = categories.Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name ?? string.Empty,
                        Description = c.Description ?? string.Empty,
                        ImageUrl = GetCategoryImageUrl(c.Name ?? string.Empty)
                    }).ToList();
                }
            }
            else if (result.Errors != null && result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    Snackbar.Add($"Category error: {error.Description}", Severity.Warning);
                }
            }
        }

        private async Task LoadNewArrivalsAsync()
        {
            var parameter = new BlindBoxParameter
            {
                PageSize = 8,
                OrderBy = "CreatedAt desc"
            };
            
            using var blindBoxService = ServiceManager.BlindBoxService;
            var result = await blindBoxService.GetBlindBoxesAsync(parameter, false);
            
            if (result.IsSuccess && result.Value != null)
            {
                var boxes = result.Value.ToList();
                
                if (boxes.Count > 0)
                {
                    _newArrivals = boxes
                        .OrderByDescending(box => box.CreatedAt)
                        .Take(8)
                        .ToList();
                    
                    // Load images for the new arrivals
                    await LoadBlindBoxImagesAsync(_newArrivals);
                    
                    _lastUpdateTime = DateTime.Now;
                }
            }
            else if (result.Errors != null && result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    Snackbar.Add($"Product error: {error.Description}", Severity.Warning);
                }
            }
        }

        private async Task LoadBlindBoxImagesAsync(List<BlindBoxDto> blindBoxes)
        {
            try
            {
                _blindBoxImages.Clear();
                
                using var blindBoxImageService = ServiceManager.BlindBoxImageService;
                
                foreach (var box in blindBoxes)
                {
                    // Get first image from BlindBoxImages table
                    var images = await blindBoxImageService.GetBlindBoxImagesByBlindBoxIdAsync(box.Id);
                    
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
                            
                            _blindBoxImages[box.Id] = firstImage;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(box.MainImageUrl))
                    {
                        // Fallback to MainImageUrl if no images in BlindBoxImages
                        var imageUrl = box.MainImageUrl;
                        if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
                        {
                            imageUrl = "/" + imageUrl;
                        }
                        
                        _blindBoxImages[box.Id] = imageUrl;
                    }
                    else
                    {
                        // Default placeholder
                        _blindBoxImages[box.Id] = "/images/box-placeholder.jpg";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading blind box images: {ex.Message}");
            }
        }

        private async Task LoadPopularCollectionsAsync()
        {
            using var packageService = ServiceManager.PackageService;
            var result = await packageService.GetAllPackagesAsync(false);
            
            if (result.IsSuccess && result.Value != null)
            {
                var packages = result.Value.ToList();

                if (packages.Count > 0)
                {
                    // Create a dictionary to categorize packages by their likely type based on name
                    var packagesByType = new Dictionary<string, List<PackageDto>>();
                    
                    foreach (var package in packages)
                    {
                        var type = GetPackageTypeFromName(package.Name);
                        if (!packagesByType.ContainsKey(type))
                            packagesByType[type] = new List<PackageDto>();
                        
                        packagesByType[type].Add(new PackageDto
                        {
                            Id = package.Id,
                            Name = package.Name ?? "Package",
                            Description = package.Description ?? string.Empty,
                            ImageUrl = package.ImageUrl ?? GetPackageImageUrl(package.Name ?? "Package"),
                            ItemCount = await GetPackageItemCount(package.Id),
                            Type = type,
                            Price = package.Price
                        });
                    }
                    
                    // Select one representative package from each type
                    _popularCollections = packagesByType
                        .SelectMany(group => group.Value.Take(1)
                            .Select(p => new CollectionDto
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Type = group.Key,
                                Description = p.Description,
                                ImageUrl = p.ImageUrl,
                                ItemCount = p.ItemCount
                            }))
                        .Take(4)
                        .ToList();
                }
            }
            else if (result.Errors != null && result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    Snackbar.Add($"Collection error: {error.Description}", Severity.Warning);
                }
            }
        }

        private async Task LoadFeaturedPackagesAsync()
        {
            using var packageService = ServiceManager.PackageService;
            var result = await packageService.GetAllPackagesAsync(false);
            
            if (result.IsSuccess && result.Value != null)
            {
                var packages = result.Value.ToList();
                
                if (packages.Count > 0)
                {
                    _featuredPackages = new List<PackageDto>();
                    
                    // Get a mix of different types
                    foreach (var package in packages.Take(6))
                    {
                        var type = GetPackageTypeFromName(package.Name);
                        _featuredPackages.Add(new PackageDto
                        {
                            Id = package.Id,
                            Name = package.Name ?? "Package",
                            Description = package.Description ?? string.Empty,
                            ImageUrl = package.ImageUrl ?? GetPackageImageUrl(package.Name ?? "Package"),
                            ItemCount = await GetPackageItemCount(package.Id),
                            Type = type,
                            Price = package.Price
                        });
                    }
                }
            }
            else if (result.Errors != null && result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    Snackbar.Add($"Package error: {error.Description}", Severity.Warning);
                }
            }
        }

        private async Task<int> GetPackageItemCount(Guid packageId)
        {
            // Get count of blind boxes in this package
            var parameter = new BlindBoxParameter
            {
                PageSize = 100,
                PageNumber = 1
            };
            
            using var blindBoxService = ServiceManager.BlindBoxService;
            var result = await blindBoxService.GetBlindBoxesAsync(parameter, false);
            
            if (result.IsSuccess && result.Value != null)
            {
                // Count the blind boxes that belong to this package
                return result.Value.Count(box => box.PackageId == packageId);
            }
            
            return 0;
        }

        private async Task LoadTestimonialsAsync()
        {
            _testimonials = new List<TestimonialDto>();
            
            try
            {
                // Since ReviewService is not available in IServiceManager, we'll use a fallback approach
                // In a real implementation, you would fetch from the actual review service if available
                
                // Fallback to some default testimonials since we can't get real ones yet
                _testimonials = new List<TestimonialDto>
                {
                    new TestimonialDto
                    {
                        Name = "Minh Anh",
                        Comment = "I've been collecting BlindBoxes for years, but this store has the most diverse and rare items I've ever seen. Very happy with my legendary dragon model!",
                        Rating = 5,
                        AvatarUrl = "/images/avatar1.jpg",
                        Date = DateTime.Now.AddDays(-5)
                    },
                    new TestimonialDto
                    {
                        Name = "Thanh Hà",
                        Comment = "Beautiful packaging and the element of surprise makes unboxing exciting. I received a rare item on my first purchase!",
                        Rating = 4,
                        AvatarUrl = "/images/avatar2.jpg",
                        Date = DateTime.Now.AddDays(-12)
                    },
                    new TestimonialDto
                    {
                        Name = "Quang Huy",
                        Comment = "Fast shipping and excellent customer service. Product quality exceeded my expectations. Will definitely buy again!",
                        Rating = 5,
                        AvatarUrl = "/images/avatar3.jpg",
                        Date = DateTime.Now.AddDays(-20)
                    }
                };
            }
            catch
            {
                // Fallback in case of errors
                _testimonials = new List<TestimonialDto>
                {
                    new TestimonialDto
                    {
                        Name = "Customer",
                        Comment = "Great products and excellent service!",
                        Rating = 5,
                        AvatarUrl = "/images/avatar1.jpg",
                        Date = DateTime.Now.AddDays(-3)
                    }
                };
            }
            
            await Task.CompletedTask; // Make the method truly async
        }

        // Check if a BlindBox is new (created within last 7 days)
        public bool IsNewProduct(DateTime createdAt)
        {
            return (DateTime.Now - createdAt).TotalDays <= 7;
        }

        // Helper methods
        private string FormatPrice(decimal price)
        {
            return $"{price.ToString("N0")} ₫";
        }

        private Color GetRarityColor(BlindBoxRarity rarity)
        {
            return rarity switch
            {
                BlindBoxRarity.Common => Color.Default,
                BlindBoxRarity.Uncommon => Color.Success,
                BlindBoxRarity.Rare => Color.Warning,
                _ => Color.Default
            };
        }

        private void NavigateToProductDetail(Guid productId)
        {
            NavigationManager.NavigateTo($"/blindbox/{productId}");
        }

        private async Task AddToCart(BlindBoxDto blindBox)
        {
            try
            {
                // Kiểm tra xem BlindBox có phải loại physical không (probability = 0)
                if (blindBox.Probability > 0)
                {
                    Snackbar.Add("Sản phẩm này chỉ có thể mở trực tuyến, không thể thêm vào giỏ hàng", Severity.Warning);
                    return;
                }
                
                // Lấy thông tin image URL
                string imageUrl = _blindBoxImages.ContainsKey(blindBox.Id) 
                    ? _blindBoxImages[blindBox.Id] 
                    : (!string.IsNullOrEmpty(blindBox.MainImageUrl) ? blindBox.MainImageUrl : "/images/box-placeholder.jpg");
                
                // Lấy giỏ hàng hiện tại từ localStorage
                var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                List<Dictionary<string, object>> cartItems = new List<Dictionary<string, object>>();
                
                if (!string.IsNullOrEmpty(cartJson))
                {
                    // Deserialize giỏ hàng
                    cartItems = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(cartJson) ?? new List<Dictionary<string, object>>();
                }
                
                // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
                bool itemExists = false;
                foreach (var item in cartItems)
                {
                    if (item.TryGetValue("BlindBoxId", out var blindBoxIdObj) && 
                        blindBoxIdObj.ToString() == blindBox.Id.ToString())
                    {
                        // Nếu đã tồn tại, tăng số lượng lên 1
                        if (item.TryGetValue("Quantity", out var quantityObj) && 
                            int.TryParse(quantityObj.ToString(), out int currentQuantity))
                        {
                            item["Quantity"] = currentQuantity + 1;
                            itemExists = true;
                            break;
                        }
                    }
                }
                
                // Nếu sản phẩm chưa tồn tại, thêm mới vào giỏ hàng
                if (!itemExists)
                {
                    var newItem = new Dictionary<string, object>
                    {
                        ["Id"] = new Random().Next(10000, 99999),
                        ["BlindBoxId"] = blindBox.Id,
                        ["ProductName"] = blindBox.Name,
                        ["Description"] = blindBox.Description,
                        ["ImageUrl"] = imageUrl,
                        ["Price"] = blindBox.CurrentPrice,
                        ["Quantity"] = 1
                    };
                    
                    cartItems.Add(newItem);
                }
                
                // Lưu giỏ hàng vào localStorage
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "blindbox_cart", 
                    JsonSerializer.Serialize(cartItems));
                
                // Hiển thị thông báo
                Snackbar.Add($"Đã thêm {blindBox.Name} vào giỏ hàng", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi: {ex.Message}", Severity.Error);
            }
        }

        // Helper method to get a placeholder image URL based on category name
        private string GetCategoryImageUrl(string categoryName)
        {
            var name = categoryName.ToLower();
            
            if (name.Contains("anime"))
                return "https://th.bing.com/th/id/OIP.0RO7lxBvGsBVZGYbLXC9CAHaEK?rs=1&pid=ImgDetMain";
            
            if (name.Contains("game"))
                return "https://vending-cdn.kootoro.com/torov-cms/upload/image/1723016271998-L%C3%BD%20gi%E1%BA%A3i%20xu%20h%C6%B0%E1%BB%9Bng%20%C4%91%E1%BB%93%20ch%C6%A1i%20blind%20box%20%C4%91%C6%B0%E1%BB%A3c%20gi%E1%BB%9Bi%20tr%E1%BA%BB%20s%C4%83n%20%C4%91%C3%B3n.jpg";

            if (name.Contains("phim") || name.Contains("movie"))
                return "https://th.bing.com/th/id/OIP.g34yQKGm8B6UxnQyjfJKwAHaE8?rs=1&pid=ImgDetMain";

            if (name.Contains("sport") || name.Contains("thể thao"))
                return "https://th.bing.com/th/id/OIP.JbqKu93pqBO4EBDrGgABjwHaFU?rs=1&pid=ImgDetMain";
            
            if (name.Contains("hoạt hình") || name.Contains("cartoon"))
                return "https://th.bing.com/th/id/OIP.p8S23BaFnpn23GFgxQCJ9gHaEK?rs=1&pid=ImgDetMain";
            
            // Default image for other categories
            return "https://vending-cdn.kootoro.com/torov-cms/upload/image/1723016271998-L%C3%BD%20gi%E1%BA%A3i%20xu%20h%C6%B0%E1%BB%9Bng%20%C4%91%E1%BB%93%20ch%C6%A1i%20blind%20box%20%C4%91%C6%B0%E1%BB%A3c%20gi%E1%BB%9Bi%20tr%E1%BA%BB%20s%C4%83n%20%C4%91%C3%B3n.jpg";
        }

        // Helper method to get a placeholder image URL based on package name
        private string GetPackageImageUrl(string packageName)
        {
            var name = packageName.ToLower();
            
            if (name.Contains("anime"))
                return "https://th.bing.com/th/id/OIP.0RO7lxBvGsBVZGYbLXC9CAHaEK?rs=1&pid=ImgDetMain";
            
            if (name.Contains("game"))
                return "https://th.bing.com/th/id/OIP.i0KMhRFQAMnZYtRQFvEMqQHaEK?rs=1&pid=ImgDetMain";

            if (name.Contains("movie") || name.Contains("phim"))
                return "https://th.bing.com/th/id/OIP.wVQV2DcbEXZk-hEwdutB0gHaFK?rs=1&pid=ImgDetMain";

            if (name.Contains("limited") || name.Contains("giới hạn"))
                return "https://th.bing.com/th/id/OIP.VRZmTy-P25nLzl0yqgEVjAHaFj?rs=1&pid=ImgDetMain";
                
            if (name.Contains("sport") || name.Contains("thể thao"))
                return "https://th.bing.com/th/id/OIP.JbqKu93pqBO4EBDrGgABjwHaFU?rs=1&pid=ImgDetMain";
            
            // Default image for other packages
            return "https://vending-cdn.kootoro.com/torov-cms/upload/image/1723016271998-L%C3%BD%20gi%E1%BA%A3i%20xu%20h%C6%B0%E1%BB%9Bng%20%C4%91%E1%BB%93%20ch%C6%A1i%20blind%20box%20%C4%91%C6%B0%E1%BB%A3c%20gi%E1%BB%9Bi%20tr%E1%BA%BB%20s%C4%83n%20%C4%91%C3%B3n.jpg";
        }

        // Determine package type from name for better categorization
        private string GetPackageTypeFromName(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return "Standard";
                
            var nameLower = packageName.ToLower();
            
            if (nameLower.Contains("anime")) return "Anime";
            if (nameLower.Contains("game")) return "Game";
            if (nameLower.Contains("movie")) return "Movie";
            if (nameLower.Contains("standard")) return "Standard";
            if (nameLower.Contains("premium")) return "Premium";
            if (nameLower.Contains("online")) return "Opened";
            if (nameLower.Contains("limited")) return "Limited Edition";
            if (nameLower.Contains("vol")) return "Collection";
            
            return "Standard";
        }
    }
}