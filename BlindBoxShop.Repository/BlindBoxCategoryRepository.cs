using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Repository.Extensions;
using BlindBoxShop.Shared.Features;
using Microsoft.EntityFrameworkCore;

namespace BlindBoxShop.Repository
{
    public class BlindBoxCategoryRepository : RepositoryBase<BlindBoxCategory>, IBlindBoxCategoryRepository
    {
        public BlindBoxCategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }



        public async Task<PagedList<BlindBoxCategory>> GetBlindBoxCategoriesAsync(BlindBoxCategoryParameter blindBoxCategoryParameter, bool trackChanges)
        {
            var blindBoxCategories = await FindAll(trackChanges)
                    .SearchByName(blindBoxCategoryParameter.SearchByName)
                    .Sort(blindBoxCategoryParameter.OrderBy)
                    .Skip((blindBoxCategoryParameter.PageNumber - 1) * blindBoxCategoryParameter.PageSize)
                    .Take(blindBoxCategoryParameter.PageSize)
                    .ToListAsync();

            var count = await FindAll(trackChanges)
                .SearchByName(blindBoxCategoryParameter.SearchByName)
                .CountAsync();


            return new PagedList<BlindBoxCategory>(
                blindBoxCategories,
                count,
                blindBoxCategoryParameter.PageNumber,
                blindBoxCategoryParameter.PageSize);
        }

    }
}
