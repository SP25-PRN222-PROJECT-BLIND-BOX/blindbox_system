using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Shared.DataTransferObject.Voucher
{
    public class VoucherDto
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public VoucherType Type { get; set; }
        public VoucherStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
