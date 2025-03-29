using System;

namespace BlindBoxShop.Application.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public Guid BlindBoxId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
} 