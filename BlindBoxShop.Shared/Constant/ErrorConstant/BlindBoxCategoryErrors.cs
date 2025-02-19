using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Shared.Constant.ErrorConstant
{
    public class BlindBoxCategoryErrors
    {
        #region Errors Message
        public const string BlindBoxCategoryNotFound = "Blind Box category with id {0} not found.";
        public const string BlindBoxCategoryExist = "Blind Box category with name {0} already exist.";
        #endregion

        #region Get Errors Result
        public static ErrorResult GetBlindBoxCategoryNotFoundError(Guid id)
        => new ErrorResult()
        {
            Code = nameof(BlindBoxCategoryNotFound),
            Description = string.Format(BlindBoxCategoryNotFound, id)

        };

        public static ErrorResult GetBlindBoxCategoryExistError(string name)
        => new ErrorResult()
        {
            Code = nameof(BlindBoxCategoryExist),
            Description = string.Format(BlindBoxCategoryExist, name)

        };
        #endregion
    }
}
