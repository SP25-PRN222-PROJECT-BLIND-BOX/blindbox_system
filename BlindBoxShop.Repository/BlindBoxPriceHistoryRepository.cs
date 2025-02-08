using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class BlindBoxPriceHistoryRepository : RepositoryBase<BlindBoxPriceHistory>, IBlindBoxPriceHistoryRepository
    {
        public BlindBoxPriceHistoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
