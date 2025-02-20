using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Utilities;
using System.Linq.Dynamic.Core;

namespace BlindBoxShop.Repository.Extensions
{
    public static class BlindBoxCategoryRepositoryExtension
    {
        public static IQueryable<BlindBoxCategory> SearchByName(this IQueryable<BlindBoxCategory> blindBoxCategories, string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return blindBoxCategories;
            }

            var lowerCaseName = name.Trim().ToLower();

            return blindBoxCategories.Where(b => b.Name.ToLower().Contains(lowerCaseName));
        }

        public static IQueryable<BlindBoxCategory> Sort(this IQueryable<BlindBoxCategory> blindBoxCategories, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString)) return blindBoxCategories.OrderBy(e => e.Name);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<BlindBoxCategory>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return blindBoxCategories.OrderBy(e => e.Name);

            return blindBoxCategories.OrderBy(orderQuery);
        }
    }
}
