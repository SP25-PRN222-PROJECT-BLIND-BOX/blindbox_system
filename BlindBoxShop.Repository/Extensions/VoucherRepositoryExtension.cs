using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Utilities;
using System.Linq.Dynamic.Core;

namespace BlindBoxShop.Repository.Extensions
{
    public static class VoucherRepositoryExtension
    {
        public static IQueryable<Voucher> SearchById(this IQueryable<Voucher> vouchers, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return vouchers;
            }

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return vouchers.Where(v =>
                (v.Id.Equals(lowerCaseSearchTerm)));
        }

        public static IQueryable<Voucher> Sort(this IQueryable<Voucher> vouchers, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return vouchers.OrderBy(e => e.StartDate); // Default sorting by StartDate

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Voucher>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return vouchers.OrderBy(e => e.StartDate);

            return vouchers.OrderBy(orderQuery);
        }
    }
}
