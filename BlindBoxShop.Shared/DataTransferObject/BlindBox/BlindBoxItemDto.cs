using System;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBox
{
    public class BlindBoxItemDto
    {
        public Guid Id { get; set; }
        
        public Guid BlindBoxId { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int Rarity { get; set; }
        
        public string ImageUrl { get; set; }
        
        public bool IsSecret { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
} 