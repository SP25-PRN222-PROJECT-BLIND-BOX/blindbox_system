using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
    }
}
