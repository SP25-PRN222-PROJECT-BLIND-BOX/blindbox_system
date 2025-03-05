using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Shared.DataTransferObject.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;

        public Guid? VoucherId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus Status { get; set; }

        public string Address { get; set; } = null!;

        public string Province { get; set; } = null!;

        public string Wards { get; set; } = null!;

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
