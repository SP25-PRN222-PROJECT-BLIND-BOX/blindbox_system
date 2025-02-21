using BlindBoxShop.Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.Voucher
{
    public class VoucherForManipulation
    {
        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public int Value { get; set; }
        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public VoucherType Type { get; set; }
        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public VoucherStatus Status { get; set; }
    }
}
