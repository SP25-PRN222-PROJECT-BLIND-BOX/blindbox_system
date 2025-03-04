using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Features;
using System.Linq.Expressions;

namespace BlindBoxShop.Repository.Contract
{
    public interface IReviewRepository : IRepositoryBase<CustomerReviews>
    {
        Task<PagedList<CustomerReviews>> GetReviewsAsync(ReviewParameter voucherParameter, bool trackChanges);
        Task<PagedList<CustomerReviews>> GetReviewsByUserIdAsync(Guid userId, ReviewParameter customerReviewParameter, bool trackChanges);
        Task<PagedList<CustomerReviews>> GetReviewsByBlindBoxIdAsync(Guid blindBoxId, ReviewParameter customerReviewParameter, bool trackChanges);
        Task<CustomerReviews?> FindAsync(Expression<Func<CustomerReviews, bool>> predicate);
    }
}
