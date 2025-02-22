
namespace BlindBoxShop.Entities.Models
{
    public class ReplyReviews : BaseEntity, IBaseEntityWithUpdatedAt
    {
        public Guid UserId { get; set; }

        public Guid CustomerReviewsId { get; set; }

        public string Reply { get; set; } = null!;

        public DateTime? UpdatedAt { get; set; }

        public User? User { get; set; }

        public CustomerReviews? CustomerReviews { get; set; }
    }
}
