using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.OrderDetail
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0.")]
        public int Quantity { get; set; }
        public string BlindBoxName { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public string ImageUrl { get; set; }
        public Guid BlindBoxId { get; set; }
        public Guid? BlindBoxItemId { get; set; }
    }
}
