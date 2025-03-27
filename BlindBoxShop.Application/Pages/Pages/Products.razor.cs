using BlindBoxShop.Shared.DataTransferObject.Shop;
using BlindBoxShop.Shared.Enum;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Components.Web;

namespace BlindBoxShop.Application.Pages.Pages
{
    public partial class Products
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        
        private string _selectedCategory = "All";
        private Guid _selectedCategoryId = Guid.Empty;
        private int _currentPage = 1;
        private int _pageSize = 8;
        private int _totalItems => _filteredProducts.Count;
        private int _totalPages => (int)Math.Ceiling(_totalItems / (double)_pageSize);
        
        private List<CategoryDto> _categories = new List<CategoryDto>
        {
            new CategoryDto { Id = Guid.NewGuid(), Name = "Category 1" },
            new CategoryDto { Id = Guid.NewGuid(), Name = "Category 2" },
            new CategoryDto { Id = Guid.NewGuid(), Name = "Category 3" },
            new CategoryDto { Id = Guid.NewGuid(), Name = "Category 3" },
            new CategoryDto { Id = Guid.NewGuid(), Name = "Category 4" },
            new CategoryDto { Id = Guid.NewGuid(), Name = "Category 5" }
        };
        
        private List<ProductDto> _allProducts = new List<ProductDto>
        {
            new ProductDto {
                Id = Guid.NewGuid(),
                Name = "LuLu The Piggy's Travel Blind Box Series",
                ImageUrl = "/images/product1.jpg",
                Price = 180000,
                Rating = 5,
                CategoryId = Guid.Empty
            },
            new ProductDto {
                Id = Guid.NewGuid(),
                Name = "Rolife Nanci Blind Box-Poems and Songs Series",
                ImageUrl = "/images/product2.jpg",
                Price = 240000,
                Rating = 5,
                CategoryId = Guid.Empty
            },
            new ProductDto {
                Id = Guid.NewGuid(),
                Name = "POP MART x Grace GUMMY Daily Life Blind Box Series",
                ImageUrl = "/images/product3.jpg",
                Price = 120000,
                Rating = 5,
                CategoryId = Guid.Empty
            },
            new ProductDto {
                Id = Guid.NewGuid(),
                Name = "EMMA Celebration Series Blind Box",
                ImageUrl = "/images/product4.jpg",
                Price = 280000,
                Rating = 5,
                CategoryId = Guid.Empty
            },
            new ProductDto {
                Id = Guid.NewGuid(),
                Name = "Tokidoki Mermicorno Series 6 Blind Box",
                ImageUrl = "/images/product5.jpg",
                Price = 150000,
                Rating = 4,
                CategoryId = Guid.Empty
            },
            new ProductDto {
                Id = Guid.NewGuid(),
                Name = "Sonny Angel Animal Series 5 Blind Box",
                ImageUrl = "/images/product6.jpg",
                Price = 195000,
                Rating = 4,
                CategoryId = Guid.Empty
            },
            new ProductDto {
                Id = Guid.NewGuid(),
                Name = "Molly Space Travel Blind Box Series",
                ImageUrl = "/images/product7.jpg",
                Price = 220000,
                Rating = 5,
                CategoryId = Guid.Empty
            },
            new ProductDto {
                Id = Guid.NewGuid(),
                Name = "Dimoo Midnight Circus Blind Box",
                ImageUrl = "/images/product8.jpg",
                Price = 199000,
                Rating = 4,
                CategoryId = Guid.Empty
            }
        };
        
        private List<ProductDto> _filteredProducts => _selectedCategoryId == Guid.Empty 
            ? _allProducts 
            : _allProducts.Where(p => p.CategoryId == _selectedCategoryId).ToList();
        
        private List<ProductDto> _products => _filteredProducts
            .Skip((_currentPage - 1) * _pageSize)
            .Take(_pageSize)
            .ToList();
        
        private string FormatPrice(decimal price)
        {
            return $"{price.ToString("N0")} ₫";
        }
        
        private void SelectCategory(Guid categoryId)
        {
            _selectedCategoryId = categoryId;
            _currentPage = 1; // Reset to first page when filtering
        }
        
        private void PageChanged(int page)
        {
            _currentPage = page;
        }
        
        private void PreviousPage()
        {
            if (_currentPage > 1)
            {
                _currentPage--;
            }
        }
        
        private void NextPage()
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
            }
        }
        
        private void NavigateToProductDetail(Guid productId)
        {
            NavigationManager.NavigateTo($"/shop/{productId}");
        }
    }
} 