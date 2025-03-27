using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages.Employee.ProductPage.Partials
{
    public partial class ProductTable : ComponentBase
    {
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        private List<ProductDto> _products = new();
        private string _searchString = string.Empty;
        private bool _isLoading = false;
        private int _totalItems = 0;
        private int _currentPage = 1;
        private int _pageSize = 12;
        private int _totalPages => (int)Math.Ceiling(_totalItems / (double)_pageSize);

        protected override async Task OnInitializedAsync()
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                _isLoading = true;
                await Task.Delay(500); // Simulate API call

                // Mock data for demonstration
                _products = GenerateMockProducts();
                _totalItems = 45; // Mock total
                
                _isLoading = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                _isLoading = false;
                Snackbar.Add($"Error loading products: {ex.Message}", Severity.Error);
            }
        }

        private async Task OnSearch()
        {
            _currentPage = 1;
            await LoadProductsAsync();
        }

        private async Task PageChanged(int page)
        {
            _currentPage = page;
            await LoadProductsAsync();
        }

        private async Task OpenCreateDialog()
        {
            // Display success message instead of dialog
            Snackbar.Add("Product created successfully!", Severity.Success);
            await LoadProductsAsync();
        }

        private async Task OpenEditDialog(ProductDto product)
        {
            // Display success message instead of dialog
            Snackbar.Add($"Product '{product.Name}' updated successfully!", Severity.Success);
            await LoadProductsAsync();
        }

        private async Task OpenDeleteDialog(ProductDto product)
        {
            // Display success message instead of dialog
            Snackbar.Add($"Product '{product.Name}' deleted successfully!", Severity.Success);
            await LoadProductsAsync();
        }

        private string FormatPrice(decimal price)
        {
            return $"{price:N0}₫";
        }

        private List<ProductDto> GenerateMockProducts()
        {
            var mockProducts = new List<ProductDto>();
            var random = new Random();

            string[] names = { 
                "Advanced Moisturizing Cream", 
                "Anti-Aging Serum", 
                "Deep Cleansing Foam", 
                "Vitamin C Brightening Mask", 
                "Hyaluronic Acid Essence",
                "Retinol Night Cream",
                "Collagen Boosting Ampoule",
                "Gentle Exfoliating Scrub",
                "UV Protection Sunscreen",
                "Niacinamide Face Toner"
            };

            string[] descriptions = {
                "Deeply hydrates and rejuvenates skin for 24 hours",
                "Reduces fine lines and wrinkles for youthful appearance",
                "Removes impurities without stripping natural oils",
                "Brightens dull skin and fades dark spots effectively",
                "Provides intense hydration for plump, bouncy skin",
                "Stimulates cell renewal while you sleep",
                "Enhances skin elasticity and firmness",
                "Removes dead skin cells for smoother texture",
                "Protects against UVA/UVB rays with lightweight formula",
                "Minimizes pores and controls excess sebum"
            };

            for (int i = 0; i < 12; i++)
            {
                var nameIndex = random.Next(names.Length);
                var descIndex = random.Next(descriptions.Length);
                var originalPrice = random.Next(200, 1500) * 1000;
                var discounted = random.Next(100) < 30; // 30% chance of discount
                var currentPrice = discounted ? originalPrice - (originalPrice * random.Next(10, 30) / 100) : originalPrice;

                mockProducts.Add(new ProductDto
                {
                    Id = i + 1,
                    Name = $"{names[nameIndex]} {random.Next(100, 999)}",
                    Description = descriptions[descIndex],
                    CurrentPrice = currentPrice,
                    OriginalPrice = discounted ? originalPrice : currentPrice,
                    ImageUrl = $"/images/products/product-{random.Next(1, 10)}.jpg",
                    Stock = random.Next(0, 100)
                });
            }

            if (!string.IsNullOrEmpty(_searchString))
            {
                mockProducts = mockProducts
                    .Where(p => p.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) || 
                                p.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return mockProducts;
        }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
} 