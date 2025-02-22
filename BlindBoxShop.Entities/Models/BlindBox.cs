using BlindBoxShop.Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Entities.Models
{
    public class BlindBox : BaseEntity, IBaseEntityWithUpdatedAt
    {
        public Guid? BlindBoxCategoryId { get; set; }

        public Guid PackageId { get; set; }

        [EnumDataType(typeof(BlindBoxRarity))]
        public BlindBoxRarity Rarity { get; set; }

        [EnumDataType(typeof(BlindBoxStatus))]
        public BlindBoxStatus Status { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        [Range(0, float.MaxValue, ErrorMessage = "TotalRatingStar must be greater than or equal to 0.")]
        public float TotalRatingStar { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Probability must be greater than or equal to 0.")]
        public float Probability { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual BlindBoxCategory? BlindBoxCategory { get; set; }

        public virtual Package? Package { get; set; }

        public virtual ICollection<CustomerReviews>? CustomerReviews { get; set; }

        public virtual ICollection<BlindBoxImage>? BlindBoxImages { get; set; }

        public virtual ICollection<BlindBoxPriceHistory>? BlindBoxPriceHistories { get; set; }
    }

}
