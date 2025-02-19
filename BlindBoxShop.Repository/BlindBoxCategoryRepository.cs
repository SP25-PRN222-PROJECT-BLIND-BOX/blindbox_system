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

        public async Task CreateBlindBoxCategoryAsync(BlindBoxCategory blindBoxCategory)
        {
            await base.CreateAsync(blindBoxCategory);
        }

        public void DeleteBlindBoxCategory(BlindBoxCategory blindBoxCategory)
        {
            base.Delete(blindBoxCategory);
        }

        public async Task<PagedList<BlindBoxCategory>> GetBlindBoxCategoriesAsync(BlindBoxCategoryParameter blindBoxCategoryParameter, bool trackChanges)
        {
            var blindBoxCategories = await FindAll(trackChanges)
                    .Sort(blindBoxCategoryParameter.OrderBy)
                    .Skip((blindBoxCategoryParameter.PageNumber - 1) * blindBoxCategoryParameter.PageSize)
                    .Take(blindBoxCategoryParameter.PageSize)
                    .ToListAsync();

            var count = await FindAll(trackChanges)
                 .Skip((blindBoxCategoryParameter.PageNumber - 1) * blindBoxCategoryParameter.PageSize)
                 .Take(blindBoxCategoryParameter.PageSize)
                .CountAsync();


            return new PagedList<BlindBoxCategory>(
                blindBoxCategories,
                count,
                blindBoxCategoryParameter.PageNumber,
                blindBoxCategoryParameter.PageSize);
        }

        public async Task<BlindBoxCategory?> GetBlindBoxCategoryAsync(Guid id, bool trackChanges)
        {
            return await FindByCondition(e => e.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
        }
    }
}
