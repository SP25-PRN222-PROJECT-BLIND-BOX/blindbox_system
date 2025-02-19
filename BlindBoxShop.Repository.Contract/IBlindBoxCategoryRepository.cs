using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.User;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Repository.Contract
{
    public interface IBlindBoxCategoryRepository : IRepositoryBase<BlindBoxCategory>
    {
        Task<PagedList<BlindBoxCategory>> GetBlindBoxCategoriesAsync(BlindBoxCategoryParameter blindBoxCategoryParameter, bool trackChanges);

        Task<BlindBoxCategory?> GetBlindBoxCategoryAsync(Guid id, bool trackChanges);

        Task CreateBlindBoxCategoryAsync(BlindBoxCategory blindBoxCategory);

        void DeleteBlindBoxCategory(BlindBoxCategory blindBoxCategory);

    }
}
