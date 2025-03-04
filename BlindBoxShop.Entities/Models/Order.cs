using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Entities.Models
{
    public class Order : BaseEntity, IBaseEntity, IBaseEntityWithUpdatedAt
    {
        public Guid UserId { get; set; }

        public Guid? VoucherId { get; set; }

        public string Status {  get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Province { get; set; } = null!;

        public string Wards { get; set; } = null!;

        public decimal SubTotal { get; set; }

        public decimal Total { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }

        public virtual Voucher? Voucher { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }

}
