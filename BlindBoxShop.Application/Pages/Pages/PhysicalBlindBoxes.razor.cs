using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BlindBoxShop.Application.Models;

namespace BlindBoxShop.Application.Pages.Pages
{
    public partial class PhysicalBlindBoxes : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private IServiceManager ServiceManager { get; set; } = default!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private List<BlindBoxCategoryDto> _categories = new();
        private List<BlindBoxDto> _allBlindBoxes = new();
        private List<BlindBoxDto> _filteredBlindBoxes = new();
        private List<BlindBoxDto> _pagedBlindBoxes = new();
        private List<BlindBoxShop.Shared.DataTransferObject.Package.PackageDto> _packages = new();
        private Dictionary<Guid, string> _blindBoxImages = new();

        private bool _isLoading = true;
        private bool _isGridView = true;
        private string _searchTerm = string.Empty;
        private string _sortOrder = "created_desc";
        private Guid? _selectedCategory;
        private Guid? _selectedPackage;
        private int _pageSize = 12;
        private int _currentPage = 1;
        private int _totalPages => (_filteredBlindBoxes.Count + _pageSize - 1) / _pageSize;

        private Dictionary<Guid, string> _categoryDict = new();
        private Dictionary<Guid, string> _packageDict = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadCategoriesAsync();
            await LoadPackagesAsync();
            await LoadBlindBoxesAsync();
            _isLoading = false;
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var parameter = new BlindBoxCategoryParameter
                {
                    PageSize = 100
                };
                
                var result = await ServiceManager.BlindBoxCategoryService.GetBlindBoxCategoriesAsync(parameter, false);
                if (result.IsSuccess && result.Value != null)
                {
                    _categories = result.Value.ToList();
                    
                    // Build dictionary for faster lookups
                    foreach (var category in _categories)
                    {
                        _categoryDict[category.Id] = category.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading categories: {ex.Message}", Severity.Error);
            }
        }

        private async Task LoadPackagesAsync()
        {
            try
            {
                var result = await ServiceManager.PackageService.GetAllPackagesAsync(false);
                if (result.IsSuccess && result.Value != null)
                {
                    // Chỉ lấy các package loại Standard (vật lý tiêu chuẩn)
                    _packages = result.Value
                        .Where(p => p.Type == PackageType.Standard)
                        .ToList();
                    
                    // Build dictionary for faster lookups
                    foreach (var package in _packages)
                    {
                        _packageDict[package.Id] = package.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading packages: {ex.Message}", Severity.Error);
            }
        }

        private async Task LoadBlindBoxesAsync()
        {
            try
            {
                // Thiết lập tham số giống VoucherTable
                var parameter = new BlindBoxParameter 
                { 
                    PageSize = 100,
                    PageNumber = 1
                };
                
                // Thêm sắp xếp theo kiểu của VoucherTable
                parameter.OrderBy = "CreatedAt desc";
                
                // Sử dụng using statement giống VoucherTable
                using var blindBoxService = ServiceManager!.BlindBoxService;
                
                // Gọi API
                var result = await blindBoxService.GetBlindBoxesAsync(parameter, false);
                
                // Xử lý kết quả giống VoucherTable
                if (result.IsSuccess)
                {
                    // Lấy giá trị từ Result
                    _allBlindBoxes = result.Value?.ToList() ?? new List<BlindBoxDto>();
                    
                    // Lọc rõ ràng các BlindBox thuộc loại Standard packages
                    var standardPackageIds = _packages
                        .Where(p => p.Type == PackageType.Standard)
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => p.Id)
                        .ToList();
                        
                    // Lọc blind boxes thuộc các package standard
                    _allBlindBoxes = _allBlindBoxes
                        .Where(b => standardPackageIds.Contains(b.PackageId))
                        .ToList();
                    
                    // Làm giàu dữ liệu với thông tin category và package
                    foreach (var box in _allBlindBoxes)
                    {
                        if (box.BlindBoxCategoryId.HasValue && _categoryDict.ContainsKey(box.BlindBoxCategoryId.Value))
                        {
                            box.CategoryName = _categoryDict[box.BlindBoxCategoryId.Value];
                        }
                        
                        if (_packageDict.ContainsKey(box.PackageId))
                        {
                            box.PackageName = _packageDict[box.PackageId];
                        }
                    }
                    
                    // Load images for all blind boxes
                    await LoadBlindBoxImagesAsync(_allBlindBoxes);
                    
                    ApplyFilters();
                }
                else
                {
                    // Handle API errors
                    if (result.Errors != null)
                    {
                        foreach (var error in result.Errors)
                        {
                            Snackbar.Add($"Lỗi: {error.Description}", Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add("Không thể tải dữ liệu BlindBox", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading BlindBoxes: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
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

        private void ApplyFilters()
        {
            _filteredBlindBoxes = _allBlindBoxes;
            
            // Lọc theo danh mục
            if (_selectedCategory.HasValue)
            {
                _filteredBlindBoxes = _filteredBlindBoxes
                    .Where(b => b.BlindBoxCategoryId == _selectedCategory.Value)
                    .ToList();
            }

            // Lọc theo gói
            if (_selectedPackage.HasValue)
            {
                _filteredBlindBoxes = _filteredBlindBoxes
                    .Where(b => b.PackageId == _selectedPackage.Value)
                    .ToList();
            }

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                var searchTermLower = _searchTerm.ToLower();
                _filteredBlindBoxes = _filteredBlindBoxes
                    .Where(b => b.Name.ToLower().Contains(searchTermLower) || 
                                b.Description.ToLower().Contains(searchTermLower))
                    .ToList();
            }

            // Sắp xếp
            _filteredBlindBoxes = SortBlindBoxes(_filteredBlindBoxes);
            
            // Phân trang
            UpdatePagedBlindBoxes();
        }

        private List<BlindBoxDto> SortBlindBoxes(List<BlindBoxDto> blindBoxes)
        {
            return _sortOrder switch
            {
                "price_asc" => blindBoxes.OrderBy(b => b.CurrentPrice).ToList(),
                "price_desc" => blindBoxes.OrderByDescending(b => b.CurrentPrice).ToList(),
                "name_asc" => blindBoxes.OrderBy(b => b.Name).ToList(),
                "name_desc" => blindBoxes.OrderByDescending(b => b.Name).ToList(),
                "created_desc" => blindBoxes.OrderByDescending(b => b.CreatedAt).ToList(),
                "created_asc" => blindBoxes.OrderBy(b => b.CreatedAt).ToList(),
                _ => blindBoxes.OrderByDescending(b => b.CreatedAt).ToList()
            };
        }

        private void UpdatePagedBlindBoxes()
        {
            _pagedBlindBoxes = _filteredBlindBoxes
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();
        }

        private void OnCategoryChanged(Guid? categoryId)
        {
            _selectedCategory = categoryId;
            _currentPage = 1;
            ApplyFilters();
        }

        private void OnPackageChanged(Guid? packageId)
        {
            _selectedPackage = packageId;
            _currentPage = 1;
            ApplyFilters();
        }

        private void OnSortOrderChanged(string sortOrder)
        {
            _sortOrder = sortOrder;
            ApplyFilters();
        }

        private void OnClearSearch()
        {
            _searchTerm = string.Empty;
            ApplyFilters();
        }

        private void OnSearchKeyDown(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                ApplyFilters();
            }
        }

        private void OnPageChanged(int page)
        {
            _currentPage = page;
            UpdatePagedBlindBoxes();
        }

        private void ResetFilters()
        {
            _selectedCategory = null;
            _selectedPackage = null;
            _searchTerm = string.Empty;
            _sortOrder = "created_desc";
            _currentPage = 1;
            ApplyFilters();
        }

        private void FilterByCategory(string categoryName)
        {
            var category = _categories.FirstOrDefault(c => c.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase));
            if (category != null)
            {
                _selectedCategory = category.Id;
                _currentPage = 1;
                ApplyFilters();
            }
        }

        private void NavigateToProductDetail(Guid blindBoxId)
        {
            NavigationManager.NavigateTo($"/blindbox/{blindBoxId}");
        }

        private async Task AddToCart(BlindBoxDto blindBox)
        {
            try
            {
                // Get existing cart from localStorage
                var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                List<CartItem> cartItems = new();
                
                if (!string.IsNullOrEmpty(cartJson))
                {
                    cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
                }
                
                // Check if item already in cart using BlindBoxId for consistency
                var existingItem = cartItems.FirstOrDefault(i => i.BlindBoxId == blindBox.Id);
                
                if (existingItem != null)
                {
                    // Increase quantity
                    existingItem.Quantity += 1;
                }
                else
                {
                    // Add new item
                    cartItems.Add(new CartItem
                    {
                        Id = blindBox.Id.GetHashCode(),
                        BlindBoxId = blindBox.Id,
                        ProductName = blindBox.Name,
                        Description = blindBox.Description?.Substring(0, Math.Min(50, blindBox.Description?.Length ?? 0)) + "...",
                        ImageUrl = _blindBoxImages.ContainsKey(blindBox.Id) ? _blindBoxImages[blindBox.Id] : "/images/box-placeholder.jpg",
                        Price = blindBox.CurrentPrice,
                        Quantity = 1
                    });
                }
                
                // Save to localStorage
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "blindbox_cart", JsonSerializer.Serialize(cartItems));
                
                Snackbar.Add($"Đã thêm \"{blindBox.Name}\" vào giỏ hàng!", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi: {ex.Message}", Severity.Error);
            }
        }

        private async Task AddToCartWithoutPropagation(BlindBoxDto blindBox, MouseEventArgs e)
        {
            // Handle the event separately to prevent navigation
            await AddToCart(blindBox);
        }

        private string FormatPrice(decimal price)
        {
            return string.Format("{0:#,##0} ₫", price);
        }

        private bool IsNewProduct(DateTime createdAt)
        {
            return (DateTime.Now - createdAt).TotalDays <= 7;
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

        private string GetRarityText(BlindBoxRarity rarity)
        {
            return rarity switch
            {
                BlindBoxRarity.Common => "Phổ Thông",
                BlindBoxRarity.Uncommon => "Không Phổ Biến",
                BlindBoxRarity.Rare => "Hiếm",
                _ => "Không Xác Định"
            };
        }

        private string GetCategoryImageUrl(string categoryName)
        {
            var name = categoryName.ToLower();
            
            if (name.Contains("anime"))
                return "https://th.bing.com/th/id/OIP.0RO7lxBvGsBVZGYbLXC9CAHaEK?rs=1&pid=ImgDetMain";
            
            if (name.Contains("game"))
                return "https://th.bing.com/th/id/OIP.i0KMhRFQAMnZYtRQFvEMqQHaEK?rs=1&pid=ImgDetMain";

            if (name.Contains("phim"))
                return "https://th.bing.com/th/id/OIP.g34yQKGm8B6UxnQyjfJKwAHaE8?rs=1&pid=ImgDetMain";

            if (name.Contains("sport") || name.Contains("thể thao"))
                return "https://th.bing.com/th/id/OIP.JbqKu93pqBO4EBDrGgABjwHaFU?rs=1&pid=ImgDetMain";
            
            if (name.Contains("hoạt hình") || name.Contains("cartoon"))
                return "https://th.bing.com/th/id/OIP.p8S23BaFnpn23GFgxQCJ9gHaEK?rs=1&pid=ImgDetMain";
            
            // Default image for other categories
            return "https://vending-cdn.kootoro.com/torov-cms/upload/image/1723016271998-L%C3%BD%20gi%E1%BA%A3i%20xu%20h%C6%B0%E1%BB%9Bng%20%C4%91%E1%BB%93%20ch%C6%A1i%20blind%20box%20%C4%91%C6%B0%E1%BB%A3c%20gi%E1%BB%9Bi%20tr%E1%BA%BB%20s%C4%83n%20%C4%91%C3%B3n.jpg";
        }
    }
} 