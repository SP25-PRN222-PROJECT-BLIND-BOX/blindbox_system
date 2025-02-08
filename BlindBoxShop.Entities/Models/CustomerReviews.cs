using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Entities.Models
{
    public class CustomerReviews : BaseEntity, IBaseEntity, IBaseEntityWithUpdatedAt
    {
        public Guid UserId { get; set; }

        public Guid BlindBoxId { get; set; }

        public string? FeedBack { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "RatingStar must be greater than or equal to 0.")]
        public float RatingStar { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }

        public virtual BlindBox? BlindBox { get; set; }
    }

}
