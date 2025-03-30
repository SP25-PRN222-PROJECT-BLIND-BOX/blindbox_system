using BlindBoxShop.Repository.Utilities;
using BlindBoxShop.Shared.Features;

using Microsoft.EntityFrameworkCore;

using System.Linq.Dynamic.Core;



namespace BlindBoxShop.Repository.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, RequestParameters requestParameters)
        {
            return query
                .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
                .Take(requestParameters.PageSize);
        }

        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, RequestParameters requestParameters)
        {
            var count = await query.CountAsync();
            var data = await query.ApplyPaging(requestParameters).ToListAsync();

            return new PagedList<T>(data, count, requestParameters.PageNumber, requestParameters.PageSize);
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString)) return query;

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<T>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return query;

            return query.OrderBy(orderQuery);
        }
    }
}
