using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Repository.Contract
{
    public interface IBlindBoxCategoryRepository : IRepositoryBase<BlindBoxCategory>
    {
        Task<PagedList<BlindBoxCategory>> GetBlindBoxCategoriesAsync(BlindBoxCategoryParameter blindBoxCategoryParameter, bool trackChanges);
    }
}
