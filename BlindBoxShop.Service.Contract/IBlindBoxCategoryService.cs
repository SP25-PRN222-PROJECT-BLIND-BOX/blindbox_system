using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IBlindBoxCategoryService : IDisposable
    {
        Task<Result<IEnumerable<BlindBoxCategoryDto>>> GetBlindBoxCategoriesAsync(BlindBoxCategoryParameter blindBoxCategoryParameter, bool trackChanges);

        Task<Result<BlindBoxCategoryDto>> GetBlindBoxCategoryAsync(Guid id, bool trackChanges);

        public Task<Result<BlindBoxCategoryDto>> CreateBlindBoxCategoryAsync(BlindBoxCategoryForCreate blindBoxCategoryForCreate);

        public Task<Result> UpdateBlindBoxCategoryAsync(Guid id, BlindBoxCategoryForUpdate blindBoxCategoryForUpdate);

        public Task<Result> DeleteBlindBoxCategoryAsync(Guid id);
    }
}
