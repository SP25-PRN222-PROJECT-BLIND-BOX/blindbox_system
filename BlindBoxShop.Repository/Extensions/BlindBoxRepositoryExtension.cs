using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Utilities;
using System.Linq.Dynamic.Core;

namespace BlindBoxShop.Repository.Extensions
{
    public static class BlindBoxRepositoryExtension
    {
        public static IQueryable<BlindBox> Sort(this IQueryable<BlindBox> blindBoxes, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return blindBoxes.OrderByDescending(e => e.CreatedAt);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<BlindBox>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return blindBoxes.OrderByDescending(e => e.CreatedAt);

            return blindBoxes.OrderBy(orderQuery);
        }
    }
} 