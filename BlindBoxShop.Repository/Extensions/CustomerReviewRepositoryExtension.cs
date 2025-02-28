using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Utilities;
using System.Linq.Dynamic.Core;

namespace BlindBoxShop.Repository.Extensions
{
    public static class CustomerReviewRepositoryExtension
    {
        public static IQueryable<CustomerReviews> SearchById(this IQueryable<CustomerReviews> reviews, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return reviews;
            }

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return reviews.Where(r =>
                r.Id.Equals(lowerCaseSearchTerm));
        }

        public static IQueryable<CustomerReviews> SearchByContent(this IQueryable<CustomerReviews> reviews, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return reviews;
            }

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return reviews.Where(r =>
                r.FeedBack != null && r.FeedBack.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<CustomerReviews> Sort(this IQueryable<CustomerReviews> reviews, string? orderByQueryString)
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