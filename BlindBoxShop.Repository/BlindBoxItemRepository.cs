using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class BlindBoxItemRepository : RepositoryBase<BlindBoxItem>, IBlindBoxItemRepository
    {
        public BlindBoxItemRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
