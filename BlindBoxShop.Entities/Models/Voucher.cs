using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Entities.Models
{
    public class Voucher : BaseEntity, IBaseEntity, IBaseEntityWithUpdatedAt
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public VoucherType Type { get; set; }

        public VoucherStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }

    }

}
