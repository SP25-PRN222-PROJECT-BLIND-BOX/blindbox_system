using BlindBoxShop.Entities.Models;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlindBoxShop.Repository.Contract
{
    public interface IBlindBoxPriceHistoryRepository : IRepositoryBase<BlindBoxPriceHistory>
    {
        Task<BlindBoxPriceHistory> GetLatestPriceHistoryByBlindBoxIdAsync(Guid blindBoxId, bool trackChanges);

        Task<BlindBoxPriceHistory> FindByIdAsync(Guid id, bool trackChanges);
        Task CreateAsync(BlindBoxPriceHistory blindBoxPriceHistory);
        Task UpdateAsync(BlindBoxPriceHistory blindBoxPriceHistory);
        Task DeleteAsync(BlindBoxPriceHistory blindBoxPriceHistory);
        Task<IQueryable<BlindBoxPriceHistory>> FindByConditionAsync(
            Expression<Func<BlindBoxPriceHistory, bool>> expression,
            bool trackChanges);
    }
}
