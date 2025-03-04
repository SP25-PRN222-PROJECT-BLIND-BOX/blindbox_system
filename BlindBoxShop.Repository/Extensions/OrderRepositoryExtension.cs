using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Utilities;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace BlindBoxShop.Repository.Extensions
{
    public static class OrderRepositoryExtension
    {
        // Search by Order ID
        public static IQueryable<Order> SearchById(this IQueryable<Order> orders, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return orders;
            }

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return orders.Where(r =>
                r.Id.Equals(lowerCaseSearchTerm));
        }

        // Search by Order Creation Date (within a date range)
        public static IQueryable<Order> SearchByDate(this IQueryable<Order> orders, DateTime? startDate, DateTime? endDate = null)
        {
            if (!startDate.HasValue && !endDate.HasValue)
            {
                return orders;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                return orders.Where(r => r.CreatedAt >= startDate.Value && r.CreatedAt <= endDate.Value);
            }

            if (startDate.HasValue)
            {
                return orders.Where(r => r.CreatedAt >= startDate.Value);
            }

            return orders.Where(r => r.CreatedAt <= endDate.Value);
        }

        // Search by Order Status
        public static IQueryable<Order> SearchByStatus(this IQueryable<Order> orders, string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return orders;
            }

            var lowerCaseStatus = status.Trim().ToLower();

            return orders.Where(r =>
                r.Status.Equals(lowerCaseStatus));
        }

        // Sorting method for orders
        public static IQueryable<Order> Sort(this IQueryable<Order> orders, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return orders.OrderBy(r => r.CreatedAt);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Order>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return orders.OrderBy(r => r.CreatedAt);

            return orders.OrderBy(orderQuery);
        }
    }
}
