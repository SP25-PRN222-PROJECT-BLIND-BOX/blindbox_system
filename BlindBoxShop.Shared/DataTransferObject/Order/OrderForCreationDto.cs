using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Shared.DataTransferObject.Order
{
    public class OrderForCreationDto
    {
        public Guid UserId { get; set; }

        public Guid? VoucherId { get; set; }
        public string Status { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; }
        public string Address { get; set; } = null!;

        public string Province { get; set; } = null!;

        public string Wards { get; set; } = null!;

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }
    }
}
