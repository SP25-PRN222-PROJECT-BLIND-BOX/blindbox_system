﻿using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.Review
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public Guid BlindBoxId { get; set; }
        public string BlindBoxName { get; set; } = string.Empty;
        public string? FeedBack { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "RatingStar must be greater than 0.")]
        public float RatingStar { get; set; }
        public DateTime CreatedAt {  get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
