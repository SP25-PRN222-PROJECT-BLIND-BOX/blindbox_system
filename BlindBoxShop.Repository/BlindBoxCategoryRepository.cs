using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class BlindBoxCategoryRepository : RepositoryBase<BlindBoxCategory>, IBlindBoxCategoryRepository
    {
        public BlindBoxCategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
