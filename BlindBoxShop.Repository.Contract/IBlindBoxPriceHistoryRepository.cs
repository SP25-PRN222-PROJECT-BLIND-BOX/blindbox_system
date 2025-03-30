using BlindBoxShop.Entities.Models;
using System;
using System.Threading.Tasks;

namespace BlindBoxShop.Repository.Contract
{
    public interface IBlindBoxPriceHistoryRepository : IRepositoryBase<BlindBoxPriceHistory>
    {
        Task<BlindBoxPriceHistory> GetLatestPriceHistoryByBlindBoxIdAsync(Guid blindBoxId, bool trackChanges);
    }
}
