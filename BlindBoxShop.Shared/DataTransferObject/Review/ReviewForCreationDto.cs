using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.Review
{
    public class ReviewForCreationDto
    {
        public Guid BlindBoxId { get; set; }
        public Guid UserId { get; set; }
        public string? FeedBack { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "RatingStar must be greater than 0.")]
        public float RatingStar { get; set; }
    }
}
