using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Entities.Models
{
    public class Voucher : BaseEntity, IBaseEntityWithUpdatedAt
    {
        public int Value { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public VoucherType Type { get; set; }

        public VoucherStatus Status { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }

    }

}
