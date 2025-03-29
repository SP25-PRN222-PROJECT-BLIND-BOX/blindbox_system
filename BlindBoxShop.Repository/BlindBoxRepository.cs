using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Repository
{
    public class BlindBoxRepository : RepositoryBase<BlindBox>, IBlindBoxRepository
    {
        public BlindBoxRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public Task<PagedList<BlindBox>> GetBlindBoxesAsync(BlindBoxParameter blindBoxParameter, bool trackChanges)
        {
            var blindBoxes = FindAll(trackChanges);



            return blindBoxes.(blindBoxParameter);
        }
    }
}
