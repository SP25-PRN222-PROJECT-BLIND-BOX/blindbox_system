using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IPackageService : IDisposable
    {
        Task<Result<IEnumerable<PackageDto>>> GetAllPackagesAsync(bool trackChanges);
        Task<Result<PackageDto>> GetPackageByIdAsync(Guid packageId, bool trackChanges);

        Task<Result<IEnumerable<PackageManageDto>>> GetAllPackagesAsync(PackageParameter parameter, bool trackChanges);

        Task<Result<PackageManageDto>> GetPackageById(Guid packageId, bool trackChanges);

        Task<Result<PackageManageDto>> CreatePackageAsync(PackageForCreate package);

        Task<Result<bool>> DeletePackageAsync(Guid packageId, bool trackChanges);


        Task<Result<PackageManageDto>> UpdatePackageAsync(Guid packageId, PackageForUpdate packageForUpdate, bool trackChanges);

        Task<Result<bool>> IsPackageOpenAsync(Guid packageId, bool trackChanges);

        Task<int> GetTotalCountAsync(PackageParameter parameter, bool trackChanges);
    }
}
