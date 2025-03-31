using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Repository.Contract
{
    public interface IBlindBoxRepository : IRepositoryBase<BlindBox>
    {
        Task<PagedList<BlindBox>> GetBlindBoxesAsync(BlindBoxParameter blindBoxParameter, bool trackChanges);
    }
}
