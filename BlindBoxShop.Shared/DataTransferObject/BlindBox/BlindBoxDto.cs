using BlindBoxShop.Shared.Enum;
using System.Collections.Generic;
using System;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBox
{
    public class BlindBoxDto
    {
        public Guid Id { get; set; }
        public Guid? BlindBoxCategoryId { get; set; }
        public Guid PackageId { get; set; }
        public BlindBoxRarity Rarity { get; set; }
        public BlindBoxStatus Status { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float TotalRatingStar { get; set; }
        public float Probability { get; set; }
        public decimal CurrentPrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public string MainImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<BlindBoxItemDto> Items { get; set; } = new List<BlindBoxItemDto>();
        
        // Debugging property to track probability source - not persisted to database
        public string ProbabilitySource { get; set; } = "Unknown";
    }
} 