using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Utilities;
using BlindBoxShop.Shared.Enum;

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

        public static IQueryable<BlindBox> FilterByName(this IQueryable<BlindBox> blindBoxes, string? blindBoxName)
        {
            if (string.IsNullOrWhiteSpace(blindBoxName))
            {
                return blindBoxes;
            }
            //var lowerCaseBlindBoxName = blindBoxName.Trim().ToLower();

            return blindBoxes.Where(b =>
                b.Name.Contains(blindBoxName, StringComparison.OrdinalIgnoreCase) ||
                        b.Description.Contains(blindBoxName, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<BlindBox> FilterByCategory(this IQueryable<BlindBox> blindBoxes, Guid? categoryId)
        {
            if (categoryId == null)
            {
                return blindBoxes;
            }
            return blindBoxes.Where(b => b.BlindBoxCategoryId == categoryId);
        }

        public static IQueryable<BlindBox> FilterByRarity(this IQueryable<BlindBox> blindBoxes, BlindBoxRarity? rarity)
        {
            if (rarity == null)
            {
                return blindBoxes;
            }
            return blindBoxes.Where(b => b.Rarity == rarity);
        }

        public static IQueryable<BlindBox> FilterByStatus(this IQueryable<BlindBox> blindBoxes, BlindBoxStatus? status)
        {
            if (status == null)
            {
                return blindBoxes;
            }
            return blindBoxes.Where(b => b.Status == status);
        }

        public static IQueryable<BlindBox> FilterByPacakge(this IQueryable<BlindBox> blindBoxes, Guid? packageId)
        {
            if (packageId == null)
            {
                return blindBoxes;
            }
            return blindBoxes.Where(b => b.PackageId == packageId);
        }
    }
}