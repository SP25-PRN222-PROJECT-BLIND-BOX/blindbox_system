using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Shared.Constant.ErrorConstant
{
    public class VoucherErrors
    {
        #region Errors Message
        public const string VoucherNotFound = "Voucher with id {0} not found.";
        public const string VoucherExist = "Voucher with name {0} already exist.";
        #endregion

        #region Get Errors Result
        public static ErrorResult GetVoucherNotFoundError(Guid id)
        => new ErrorResult()
        {
            Code = nameof(VoucherNotFound),
            Description = string.Format(VoucherNotFound, id)

        };

        public static ErrorResult GetVoucherExistError(string name)
        => new ErrorResult()
        {
            Code = nameof(VoucherExist),
            Description = string.Format(VoucherExist, name)

        };
        #endregion
    }
}
