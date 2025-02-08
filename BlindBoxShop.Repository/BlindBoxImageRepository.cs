using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class BlindBoxImageRepository : RepositoryBase<BlindBoxImage>, IBlindBoxImageRepository
    {
        public BlindBoxImageRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
