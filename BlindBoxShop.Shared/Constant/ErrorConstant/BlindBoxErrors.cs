using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Shared.Constant.ErrorConstant
{
    public class BlindBoxErrors
    {
        #region Errors Message
        public const string BlindBoxNotFound = "Blind Box with id {0} not found.";
        #endregion

        #region Get Errors Result
        public static ErrorResult GetBlindBoxNotFoundError(Guid id)
        => new ErrorResult()
        {
            Code = nameof(BlindBoxNotFound),
            Description = string.Format(BlindBoxNotFound, id)

        };
        #endregion
    }
}
