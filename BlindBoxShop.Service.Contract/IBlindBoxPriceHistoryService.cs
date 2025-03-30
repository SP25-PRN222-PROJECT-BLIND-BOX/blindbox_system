using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IBlindBoxPriceHistoryService : IDisposable
    {
        Task<Result<BlindBoxPriceHistoryDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<BlindBoxPriceHistoryDto>>> GetByBlindBoxIdAsync(Guid blindBoxId);
        Task<Result<BlindBoxPriceHistoryDto>> CreateAsync(BlindBoxPriceHistoryDto blindBoxPriceHistoryDto);
        Task<Result<BlindBoxPriceHistoryDto>> UpdateAsync(Guid id, BlindBoxPriceHistoryDto blindBoxPriceHistoryDto);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
