using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.ResultModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Service.Contract
{
    public interface IBlindBoxService : IDisposable
    {
        Task<Result<IEnumerable<BlindBoxDto>>> GetBlindBoxesAsync(BlindBoxParameter blindBoxParameter, bool trackChanges);
        Task<Result<BlindBoxDto>> GetBlindBoxByIdAsync(Guid blindBoxId, bool trackChanges);
        Task<Result<BlindBoxDto>> CreateBlindBoxAsync(BlindBoxForCreate blindBoxForCreate);
        Task<Result<BlindBoxDto>> UpdateBlindBoxAsync(Guid blindBoxId, BlindBoxForUpdate blindBoxForUpdate, bool trackChanges);
        Task<Result<bool>> DeleteBlindBoxAsync(Guid blindBoxId, bool trackChanges);
        Task<Result<IEnumerable<BlindBoxDto>>> GetBlindBoxesByPackageIdAsync(Guid packageId, bool trackChanges);
        Task<Result<bool>> ResetBlindBoxProbabilityAsync(Guid blindBoxId);
        Task<Result<bool>> IncrementBlindBoxProbabilityAsync(Guid blindBoxId);
    }
}
