using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Repository.Extensions;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Repository
{
    public class BlindBoxRepository : RepositoryBase<BlindBox>, IBlindBoxRepository
    {
        public BlindBoxRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<PagedList<BlindBox>> GetBlindBoxesAsync(BlindBoxParameter blindBoxParameter, bool trackChanges)
        {
            var blindBoxes = FindAll(trackChanges)
                .FilterByName(blindBoxParameter.SearchByName)
                .FilterByPacakge(blindBoxParameter.PackageId)
                .FilterByCategory(blindBoxParameter.CategoryId)
                .FilterByRarity(blindBoxParameter.Rarity)
                .FilterByStatus(blindBoxParameter.Status)
                .Sort(blindBoxParameter.OrderBy);

            return await blindBoxes.ToPagedListAsync(blindBoxParameter);
        }
    }
}
