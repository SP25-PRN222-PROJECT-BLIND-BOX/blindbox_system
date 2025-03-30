using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlindBoxShop.Repository
{
    public class BlindBoxPriceHistoryRepository : RepositoryBase<BlindBoxPriceHistory>, IBlindBoxPriceHistoryRepository
    {
        public BlindBoxPriceHistoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<BlindBoxPriceHistory> GetLatestPriceHistoryByBlindBoxIdAsync(Guid blindBoxId, bool trackChanges)
        {
            var query = trackChanges 
                ? RepositoryContext.BlindBoxPriceHistories
                : RepositoryContext.BlindBoxPriceHistories.AsNoTracking();

            var priceHistory = await query
                .Where(bph => bph.BlindBoxId == blindBoxId)
                .Include(bph => bph.BlindBox)
                .Include(bph => bph.BlindBox.BlindBoxImages)
                .OrderByDescending(bph => bph.CreatedAt)
                .FirstOrDefaultAsync();

            // Log information for debugging
            if (priceHistory != null)
            {
                Console.WriteLine($"Price History ID: {priceHistory.Id}");
                Console.WriteLine($"BlindBox ID: {priceHistory.BlindBoxId}");
                Console.WriteLine($"BlindBox Name: {priceHistory.BlindBox?.Name ?? "null"}");
                Console.WriteLine($"BlindBox Images: {priceHistory.BlindBox?.BlindBoxImages?.Count() ?? 0}");
                
                if (priceHistory.BlindBox?.BlindBoxImages != null && priceHistory.BlindBox.BlindBoxImages.Any())
                {
                    foreach (var image in priceHistory.BlindBox.BlindBoxImages)
                    {
                        Console.WriteLine($"Image URL: '{image.ImageUrl}'");
                    }
                }
                else
                {
                    Console.WriteLine("No images found for this BlindBox");
                }
            }
            else
            {
                Console.WriteLine($"No price history found for BlindBox ID: {blindBoxId}");
            }

            return priceHistory;
        }


        

        public async Task<BlindBoxPriceHistory> FindByIdAsync(Guid id, bool trackChanges)
        {
            return await FindByCondition(bph => bph.Id.Equals(id), trackChanges)
                .SingleOrDefaultAsync();
        }

        public async Task CreateAsync(BlindBoxPriceHistory blindBoxPriceHistory)
        {
            await RepositoryContext.BlindBoxPriceHistories.AddAsync(blindBoxPriceHistory);
        }

        public async Task UpdateAsync(BlindBoxPriceHistory blindBoxPriceHistory)
        {
            RepositoryContext.BlindBoxPriceHistories.Update(blindBoxPriceHistory);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(BlindBoxPriceHistory blindBoxPriceHistory)
        {
            RepositoryContext.BlindBoxPriceHistories.Remove(blindBoxPriceHistory);
            await Task.CompletedTask;
        }

        public Task<IQueryable<BlindBoxPriceHistory>> FindByConditionAsync(
            Expression<Func<BlindBoxPriceHistory, bool>> expression,
            bool trackChanges)
        {
            return Task.FromResult(FindByCondition(expression, trackChanges));
        }
    }
}
