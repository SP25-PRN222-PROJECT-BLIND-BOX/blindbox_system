using BlindBoxShop.Shared.Enum;

using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBox
{
    public abstract class BlindBoxForManipulation
    {
        [Required(ErrorMessage = "BlindBox name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "BlindBox category is required")]
        public Guid? BlindBoxCategoryId { get; set; }

        [Required(ErrorMessage = "Package is required")]
        public Guid? PackageId { get; set; }

        [Required(ErrorMessage = "Rarity is required")]
        [EnumDataType(typeof(BlindBoxRarity), ErrorMessage = "Invalid rarity value")]
        public BlindBoxRarity Rarity { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [EnumDataType(typeof(BlindBoxStatus), ErrorMessage = "Invalid status value")]
        public BlindBoxStatus Status { get; set; }

        [Required(ErrorMessage = "Probability is required")]
        [Range(0, 100, ErrorMessage = "Probability must be between 0 and 100")]
        public float Probability { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 1000000000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string? MainImageUrl { get; set; }
    }
}