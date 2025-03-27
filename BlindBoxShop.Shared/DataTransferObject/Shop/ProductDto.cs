using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Shared.DataTransferObject.Shop
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public Guid CategoryId { get; set; }
    }
} 