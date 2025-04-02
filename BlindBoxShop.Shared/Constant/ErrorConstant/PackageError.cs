using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Shared.Constant.ErrorConstant
{
    public class PackageError
    {
        public const string PackageNotFound = "Package with id {0} not found.";

        public static ErrorResult GetPackageNotFoundError(Guid id) => new ErrorResult()
        {
            Code = nameof(PackageNotFound),
            Description = string.Format(PackageNotFound, id)
        };
    }
}
