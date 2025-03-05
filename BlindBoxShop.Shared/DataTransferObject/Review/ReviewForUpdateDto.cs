using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.Review
{
    public class ReviewForUpdateDto
    {
        public string? FeedBack { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "RatingStar must be greater than 0.")]
        public float RatingStar { get; set; }
    }
}
