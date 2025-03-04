using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.OrderDetail
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }
        public Guid BlindBoxPriceHistoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0.")]
        public int Quantity { get; set; }
    }
}
