using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Utilities;
using System.Linq.Dynamic.Core;

namespace BlindBoxShop.Repository.Extensions
{
    public static class ReplyRepositoryExtension
    {
        public static IQueryable<ReplyReviews> SearchById(this IQueryable<ReplyReviews> reviews, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return reviews;
            }

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return reviews.Where(r =>
                r.Id.Equals(lowerCaseSearchTerm));
        }

        public static IQueryable<ReplyReviews> SearchByContent(this IQueryable<ReplyReviews> reviews, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return reviews;
            }

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return reviews.Where(r =>
                r.Reply != null && r.Reply.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<ReplyReviews> SearchByUsername(this IQueryable<ReplyReviews> reviews, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return reviews;
            }

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return reviews.Where(r =>
                r.Reply != null && r.Reply.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<ReplyReviews> Sort(this IQueryable<ReplyReviews> reviews, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return reviews.OrderBy(r => r.CreatedAt);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<CustomerReviews>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return reviews.OrderBy(r => r.CreatedAt);

            return reviews.OrderBy(orderQuery);
        }
    }
}
