using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Features;
using System.Linq.Expressions;

namespace BlindBoxShop.Repository.Contract
{
    public interface ICustomerReviewRepository : IRepositoryBase<CustomerReviews>
    {
        Task<PagedList<CustomerReviews>> GetReviewsAsync(CustomerReviewParameter voucherParameter, bool trackChanges);
        Task<PagedList<CustomerReviews>> GetReviewsByUserIdAsync(Guid userId, CustomerReviewParameter customerReviewParameter, bool trackChanges);
        Task<PagedList<CustomerReviews>> GetReviewsByBlindBoxIdAsync(Guid blindBoxId, CustomerReviewParameter customerReviewParameter, bool trackChanges);
        Task<CustomerReviews?> FindAsync(Expression<Func<CustomerReviews, bool>> predicate);
    }
}
