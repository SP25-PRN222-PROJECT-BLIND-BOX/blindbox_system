using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages.Pages
{
    public partial class OpenableBlindBoxes : ComponentBase
    {
        [Inject] 
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject] 
        private IServiceManager ServiceManager { get; set; } = default!;

        [Inject] 
        private ISnackbar Snackbar { get; set; } = default!;

        private bool _isLoading = true;
        private List<BlindBoxDto> _allBlindBoxes = new();
        private List<BlindBoxDto> _filteredBlindBoxes = new();
        private List<BlindBoxDto> _pagedBlindBoxes = new();
        private List<BlindBoxShop.Shared.DataTransferObject.Package.PackageDto> _packages = new();
        private Dictionary<Guid, string> _blindBoxImages = new();
        
        private string _searchTerm = string.Empty;
        private Guid? _selectedPackage;
        private BlindBoxRarity? _selectedRarity;
        private string _sortOrder = "price_asc";
        
        private int _pageSize = 12;
        private int _currentPage = 1;
        private int _totalPages => (_filteredBlindBoxes.Count + _pageSize - 1) / _pageSize;

        protected override async Task OnInitializedAsync()
        {
            await LoadPackagesAsync();
            await LoadBlindBoxesAsync();
            
            _isLoading = false;
        }

        private async Task LoadPackagesAsync()
        {
            try
            {
                var result = await ServiceManager.PackageService.GetAllPackagesAsync(false);
                if (result.IsSuccess && result.Value != null)
                {
                    // Filter packages with type Opened (openable packages)
                    _packages = result.Value
                        .Where(p => p.Type == PackageType.Opened)
                        .ToList();
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
                    
                    // Lọc rõ ràng các BlindBox thuộc loại Opened packages
                    var openablePackageIds = _packages
                        .Where(p => p.Type == PackageType.Opened)
                        .Select(p => p.Id)
                        .ToList();
                        
                    // Lọc blind boxes thuộc các package openable
                    _allBlindBoxes = _allBlindBoxes
                        .Where(b => openablePackageIds.Contains(b.PackageId))
                        .ToList();
                        
                    // Load images for all blind boxes
                    await LoadBlindBoxImagesAsync(_allBlindBoxes);
                        
                    ApplyFilters();
                }
                else
                {
                    // Xử lý lỗi từ API
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

            // Apply package filter
            if (_selectedPackage.HasValue)
            {
                _filteredBlindBoxes = _filteredBlindBoxes
                    .Where(b => b.PackageId == _selectedPackage.Value)
                    .ToList();
            }

            // Apply rarity filter
            if (_selectedRarity.HasValue)
            {
                _filteredBlindBoxes = _filteredBlindBoxes
                    .Where(b => b.Rarity == _selectedRarity.Value)
                    .ToList();
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                var searchTermLower = _searchTerm.ToLower();
                _filteredBlindBoxes = _filteredBlindBoxes
                    .Where(b => b.Name.ToLower().Contains(searchTermLower) || 
                                b.Description.ToLower().Contains(searchTermLower))
                    .ToList();
            }

            // Apply sorting
            _filteredBlindBoxes = SortBlindBoxes(_filteredBlindBoxes);

            // Apply pagination
            UpdatePagedBlindBoxes();
        }

        private List<BlindBoxDto> SortBlindBoxes(List<BlindBoxDto> blindBoxes)
        {
            return _sortOrder switch
            {
                "price_asc" => blindBoxes.OrderBy(b => b.CurrentPrice).ToList(),
                "price_desc" => blindBoxes.OrderByDescending(b => b.CurrentPrice).ToList(),
                "probability_asc" => blindBoxes.OrderBy(b => b.Probability).ToList(),
                "probability_desc" => blindBoxes.OrderByDescending(b => b.Probability).ToList(),
                _ => blindBoxes
            };
        }

        private void UpdatePagedBlindBoxes()
        {
            _pagedBlindBoxes = _filteredBlindBoxes
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();
        }

        private void OnPackageChanged(Guid? packageId)
        {
            _selectedPackage = packageId;
            _currentPage = 1;
            ApplyFilters();
        }

        private void OnRarityChanged(BlindBoxRarity? rarity)
        {
            _selectedRarity = rarity;
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
            _selectedPackage = null;
            _selectedRarity = null;
            _searchTerm = string.Empty;
            _sortOrder = "price_asc";
            _currentPage = 1;
            ApplyFilters();
        }

        private void NavigateToProductDetail(Guid blindBoxId)
        {
            NavigationManager.NavigateTo($"/blindbox/{blindBoxId}");
        }

        private async Task AddToCart(BlindBoxDto blindBox)
        {
            try
            {
                // Openable blindboxes can't be added to cart - show appropriate message
                Snackbar.Add("Sản phẩm này chỉ có thể mở trực tuyến, không thể thêm vào giỏ hàng", Severity.Warning);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi: {ex.Message}", Severity.Error);
            }
        }

        private async Task AddToCartWithoutPropagation(BlindBoxDto blindBox, MouseEventArgs e)
        {
            // Just call the original method - we don't need to prevent default
            // since we're handling the event separately with stopPropagation
            await AddToCart(blindBox);
        }

        private string GetRarityText(BlindBoxRarity rarity)
        {
            return rarity switch
            {
                BlindBoxRarity.Common => "Phổ Thông",
                BlindBoxRarity.Uncommon => "Không Phổ Biến",
                BlindBoxRarity.Rare => "Hiếm",
                _ => "Không xác định"
            };
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

        private Color GetProbabilityColor(double probability)
        {
            if (probability < 2) return Color.Error;
            if (probability < 5) return Color.Warning;
            if (probability < 15) return Color.Info;
            return Color.Success;
        }

        private string FormatPrice(decimal price)
        {
            return string.Format("{0:#,##0} ₫", price);
        }
    }
} 