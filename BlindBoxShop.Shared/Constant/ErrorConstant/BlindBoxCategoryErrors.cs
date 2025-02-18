using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Shared.Constant.ErrorConstant
{
    public class BlindBoxCategoryErrors
    {
        #region Errors Message
        public const string BlindBoxCategoryNotFound = "Blind Box category with id {0} not found.";
        #endregion

        #region Get Errors Result
        public static ErrorResult GetBlindBoxCategoryNotFoundError(Guid id)
        => new ErrorResult()
        {
            Code = nameof(BlindBoxCategoryNotFound),
            Description = string.Format(BlindBoxCategoryNotFound, id)

        };
        #endregion
    }
}
