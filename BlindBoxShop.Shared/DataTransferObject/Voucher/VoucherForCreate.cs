using BlindBoxShop.Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.Voucher
{
    public class VoucherForCreate : VoucherForManipulation
    {
        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public new VoucherType Type { get; set; } = VoucherType.Cash;
        public new VoucherStatus Status
        {
            get
            {
                return VoucherStatus.Active;
            }
        }

    }
}
