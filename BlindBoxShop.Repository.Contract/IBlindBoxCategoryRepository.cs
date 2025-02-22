using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Repository.Contract
{
    public interface IBlindBoxCategoryRepository : IRepositoryBase<BlindBoxCategory>
    {
        Task<PagedList<BlindBoxCategory>> GetBlindBoxCategoriesAsync(BlindBoxCategoryParameter blindBoxCategoryParameter, bool trackChanges);
    }
}
