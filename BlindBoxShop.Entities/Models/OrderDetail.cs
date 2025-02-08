using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Entities.Models
{
    public class OrderDetail : BaseEntity
    {
        public Guid OrderId { get; set; }

        public Guid BlindBoxPriceHistoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0.")]
        public int Quantity { get; set; }

        public Order? Order { get; set; }

        public BlindBoxPriceHistory? BlindBoxPriceHistory { get; set; }
    }

}
