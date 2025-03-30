using System;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBox
{
    public class BlindBoxImageDto
    {
        public Guid Id { get; set; }
        public Guid BlindBoxId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
} 