using BlindBoxShop.Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.Voucher
{
    [CustomValidation(typeof(VoucherForManipulation), nameof(ValidateDates))]
    public class VoucherForManipulation
    {
        [Required(ErrorMessage = "{0} of Voucher is required.")]
        [Range(1, int.MaxValue)]
        public int Value { get; set; }

        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public VoucherType Type { get; set; } = VoucherType.Cash;

        [Required(ErrorMessage = "{0} of Voucher is required.")]
        public VoucherStatus Status { get; set; }

        // Phương thức xác thực tùy chỉnh
        public static ValidationResult? ValidateDates(object? value, ValidationContext validationContext)
        {
            var voucher = validationContext.ObjectInstance as VoucherForManipulation;
            if (voucher?.StartDate != null && voucher.EndDate != null)
            {
                if (voucher.EndDate <= voucher.StartDate)
                {
                    return new ValidationResult("EndDate must be greater than to StartDate.", new[] { nameof(VoucherForManipulation.EndDate) });
                }
            }

            return ValidationResult.Success;
        }
    }
}
