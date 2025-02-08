using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class BlindBoxRepository : RepositoryBase<BlindBox>, IBlindBoxRepository
    {
        public BlindBoxRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
