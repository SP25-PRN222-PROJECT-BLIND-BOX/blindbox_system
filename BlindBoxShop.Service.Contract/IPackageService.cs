using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.ResultModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Service.Contract
{
    public interface IPackageService : IDisposable
    {
        Task<Result<IEnumerable<PackageDto>>> GetAllPackagesAsync(bool trackChanges);
        Task<Result<PackageDto>> GetPackageByIdAsync(Guid packageId, bool trackChanges);
        
        
    }
}
