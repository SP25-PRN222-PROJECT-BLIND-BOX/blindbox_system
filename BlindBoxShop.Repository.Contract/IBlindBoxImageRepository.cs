using BlindBoxShop.Entities.Models;

namespace BlindBoxShop.Repository.Contract
{
    public interface IBlindBoxImageRepository : IRepositoryBase<BlindBoxImage>
    {
        Task<List<BlindBoxImage>> GetImageByBlindBoxId(Guid blindBoxId);
    }
}
