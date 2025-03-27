using BlindBoxShop.Shared.Enum;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Pages
{
    // DTO classes for Home page
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class BlindBoxDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public BlindBoxRarity Rarity { get; set; }
        public int Rating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class CollectionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class TestimonialDto
    {
        public string Name { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public partial class Home
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Sample data
        private List<CategoryDto> _featuredCategories = new List<CategoryDto>
        {
            new CategoryDto
            {
                Id = Guid.NewGuid(),
                Name = "Anime Collectibles",
                Description = "Discover exclusive anime figurines and collectibles from your favorite series.",
                ImageUrl = "/images/category-anime.jpg"
            },
            new CategoryDto
            {
                Id = Guid.NewGuid(),
                Name = "Gaming Mystery Boxes",
                Description = "Unbox gaming merchandise, accessories, and collectibles from popular franchises.",
                ImageUrl = "/images/category-gaming.jpg"
            },
            new CategoryDto
            {
                Id = Guid.NewGuid(),
                Name = "Limited Edition",
                Description = "Ultra-rare and exclusive items with limited availability. Collectors' paradise!",
                ImageUrl = "/images/category-limited.jpg"
            }
        };

        private List<BlindBoxDto> _newArrivals = new List<BlindBoxDto>
        {
            new BlindBoxDto
            {
                Id = Guid.NewGuid(),
                Name = "Mystic Heroes Collection",
                Description = "Limited edition collectibles featuring your favorite superheroes with special powers.",
                ImageUrl = "/images/box-heroes.jpg",
                Price = 299000,
                Rarity = BlindBoxRarity.Rare,
                Rating = 4,
                ReviewCount = 28
            },
            new BlindBoxDto
            {
                Id = Guid.NewGuid(),
                Name = "Cyberpunk Edition",
                Description = "Futuristic merchandise inspired by the neon-lit dystopian world of cyberpunk.",
                ImageUrl = "/images/box-cyberpunk.jpg",
                Price = 199000,
                Rarity = BlindBoxRarity.Uncommon,
                Rating = 5,
                ReviewCount = 42
            },
            new BlindBoxDto
            {
                Id = Guid.NewGuid(),
                Name = "Kawaii Surprise",
                Description = "Adorable Japanese-inspired cute items that will bring joy to your collection.",
                ImageUrl = "/images/box-kawaii.jpg",
                Price = 149000,
                Rarity = BlindBoxRarity.Common,
                Rating = 4,
                ReviewCount = 63
            },
            new BlindBoxDto
            {
                Id = Guid.NewGuid(),
                Name = "Legendary Dragons",
                Description = "Ultra-rare dragon figurines with detailed craftsmanship and premium quality.",
                ImageUrl = "/images/box-dragons.jpg",
                Price = 499000,
                Rarity = BlindBoxRarity.Rare,
                Rating = 5,
                ReviewCount = 17
            }
        };

        private List<CollectionDto> _popularCollections = new List<CollectionDto>
        {
            new CollectionDto
            {
                Id = Guid.NewGuid(),
                Name = "Summer 2023 Special",
                Description = "Limited time collection featuring exclusive summer-themed collectibles and accessories.",
                ImageUrl = "/images/collection-summer.jpg",
                ItemCount = 12
            },
            new CollectionDto
            {
                Id = Guid.NewGuid(),
                Name = "Retro Gaming Nostalgia",
                Description = "Classic gaming memorabilia from the golden era of video games. Perfect for retro gamers!",
                ImageUrl = "/images/collection-retro.jpg",
                ItemCount = 8
            }
        };

        private List<TestimonialDto> _testimonials = new List<TestimonialDto>
        {
            new TestimonialDto
            {
                Name = "Minh Anh",
                Comment = "I've been collecting BlindBoxes for years, but this shop has the best variety and rarest items I've ever seen. Super satisfied with my legendary dragon figurine!",
                Rating = 5,
                AvatarUrl = "/images/avatar1.jpg",
                Date = DateTime.Now.AddDays(-5)
            },
            new TestimonialDto
            {
                Name = "Thanh Hà",
                Comment = "The packaging is beautiful and the surprise element makes it so exciting. I got an uncommon item in my first purchase!",
                Rating = 4,
                AvatarUrl = "/images/avatar2.jpg",
                Date = DateTime.Now.AddDays(-12)
            },
            new TestimonialDto
            {
                Name = "Quang Huy",
                Comment = "Fast shipping and excellent customer service. The quality of items exceeds my expectations. Will definitely buy again!",
                Rating = 5,
                AvatarUrl = "/images/avatar3.jpg",
                Date = DateTime.Now.AddDays(-20)
            }
        };

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
            NavigationManager.NavigateTo($"/shop/{productId}");
        }
    }
}