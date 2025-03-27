using BlindBoxShop.Shared.DataTransferObject.Shop;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages.Pages
{
    public class ProductReviewDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Rating { get; set; }
        public int ReviewCount { get; set; }
        public int StockQuantity { get; set; }
        public string Dimensions { get; set; } = string.Empty;
        public string ModelNumber { get; set; } = string.Empty;
        public DateTime? ReleaseDate { get; set; }
        public string CountryOrigin { get; set; } = string.Empty;
        public string BestSellersRank { get; set; } = string.Empty;
    }

    public partial class ProductDetail : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }

        private ProductDetailDto? _product;
        private List<string> _thumbnails = new();
        private string _selectedThumbnail = string.Empty;
        private int _quantity = 1;
        private List<BreadcrumbItem> _breadcrumbs = new();
        private List<ProductReviewDto> _reviews = new();
        private List<ProductReviewDto> _reviewsWithImages = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadProduct();
            await LoadReviews();
            InitializeBreadcrumbs();
        }

        private async Task LoadProduct()
        {
            // In a real app, this would be an API call to fetch product details
            // For now, we'll use mock data
            await Task.Delay(500); // Simulate API delay

            _product = new ProductDetailDto
            {
                Id = Id,
                Name = "Lulu The Piggy's Travel Blind Box Series",
                Brand = "POPMART",
                Description = "Lulu The Piggy's Travel Series features the adorable Lulu character in different travel-themed outfits and poses. Each blind box contains one random collectible figure. There are 6 regular designs to collect, plus 1 secret chase figure with a rarity of 1/144.",
                ImageUrl = "/images/box-kawaii.jpg",
                ImageUrls = new List<string>
                {
                    "/images/box-kawaii.jpg",
                    "/images/box-cyberpunk.jpg",
                    "/images/box-dragons.jpg",
                    "/images/box-heroes.jpg"
                },
                OriginalPrice = 200000,
                DiscountPrice = 180000,
                Rating = 5,
                ReviewCount = 3257,
                StockQuantity = 15,
                Dimensions = "10x5x5 cm",
                ModelNumber = "LLP2023",
                ReleaseDate = new DateTime(2022, 4, 1),
                CountryOrigin = "China",
                BestSellersRank = "#1"
            };

            _thumbnails = _product.ImageUrls;
            _selectedThumbnail = _thumbnails.Count > 0 ? _thumbnails[0] : string.Empty;
        }

        private async Task LoadReviews()
        {
            // In a real app, this would be an API call to fetch reviews
            await Task.Delay(200); // Simulate API delay

            _reviews = new List<ProductReviewDto>
            {
                new ProductReviewDto
                {
                    Id = Guid.NewGuid(),
                    CustomerName = "Saminatha",
                    AvatarUrl = "/images/avatar1.jpg",
                    Rating = 5,
                    Title = "Overall - great for all price!",
                    Comment = "Hello, I am a lover of toys. I don't have the luxury of time to meticulously plan every purchase or review things the moment they arrive. I literally squealed like a child on 1/30 in the afternoon during lunch-and-learn meeting when I got a great Lulu! So this review with a month or so delay as I've been busy with a quad of children that I love dearly!",
                    ReviewDate = DateTime.Now.AddDays(-15)
                },
                new ProductReviewDto
                {
                    Id = Guid.NewGuid(),
                    CustomerName = "Saminatha",
                    AvatarUrl = "/images/avatar2.jpg",
                    Rating = 5,
                    Title = "Overall - great for all price!",
                    Comment = "Hello, I am a lover of toys. I don't have the luxury of time to meticulously plan every purchase or review things the moment they arrive. I literally squealed like a child on 1/30 in the afternoon during lunch-and-learn meeting when I got a great Lulu! So this review with a month or so delay as I've been busy with a quad of children that I love dearly!",
                    ReviewDate = DateTime.Now.AddDays(-27),
                    ImageUrl = "/images/review1.jpg"
                }
            };

            _reviewsWithImages = _reviews.Where(r => !string.IsNullOrEmpty(r.ImageUrl)).ToList();
        }

        private void InitializeBreadcrumbs()
        {
            _breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Home", href: "/"),
                new BreadcrumbItem("Products", href: "/shop"),
                new BreadcrumbItem(_product?.Name ?? "Product Detail", href: null, disabled: true)
            };
        }

        private string FormatPrice(decimal price)
        {
            return $"{price.ToString("N0")} ₫";
        }

        private void SelectThumbnail(string thumbnail)
        {
            _selectedThumbnail = thumbnail;
            _product!.ImageUrl = thumbnail;
        }

        private async Task AddToCart()
        {
            // Here you would implement the logic to add the product to the cart
            // For now, we'll just show a snackbar
            await Task.Delay(100);
            // You would typically inject ISnackbar and use it here
        }
    }
} 