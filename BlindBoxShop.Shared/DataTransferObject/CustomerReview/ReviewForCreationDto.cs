using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.CustomerReview
{
    public class ReviewForCreationDto
    {
        public Guid BlindBoxId { get; set; }
        public Guid UserId { get; set; }
        public string? FeedBack { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "RatingStar must be greater than or equal to 0.")]
        public float RatingStar { get; set; }
    }
}
