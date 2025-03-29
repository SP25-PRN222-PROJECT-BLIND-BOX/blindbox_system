using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Entities.Models
{
    public class BlindBoxItem : BaseEntity, IBaseEntity
    {
        public Guid BlindBoxId { get; set; }
        
        public string Name { get; set; } = null!;
        
        public string? Description { get; set; }
        
        public BlindBoxRarity Rarity { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public bool IsSecret { get; set; }
        
        // Navigation property
        public virtual BlindBox? BlindBox { get; set; }
    }
} 