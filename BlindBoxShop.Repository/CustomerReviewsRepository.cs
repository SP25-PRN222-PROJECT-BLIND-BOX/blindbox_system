using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Repository.Extensions;
using BlindBoxShop.Shared.Features;
using Microsoft.EntityFrameworkCore;

namespace BlindBoxShop.Repository
{
    public class CustomerReviewRepository : RepositoryBase<CustomerReviews>, ICustomerReviewsRepository
    {
        public CustomerReviewRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<PagedList<CustomerReviews>> GetReviewsAsync(CustomerReviewParameter reviewParameter, bool trackChanges)
        {
            var reviews = await FindAll(trackChanges)
                .Include(r => r.User)
                .Include(r => r.BlindBox)
                .SearchById(reviewParameter.SearchById)
                .SearchByContent(reviewParameter.SearchByContent)
                .Sort(reviewParameter.OrderBy)
                .Skip((reviewParameter.PageNumber - 1) * reviewParameter.PageSize)
                .Take(reviewParameter.PageSize)
                .ToListAsync();

            var count = await FindAll(trackChanges)
                .SearchById(reviewParameter.SearchById)
                .SearchByContent(reviewParameter.SearchByContent)
                .CountAsync();

            return new PagedList<CustomerReviews>(
                reviews,
                count,
                reviewParameter.PageNumber,
                reviewParameter.PageSize);
        }

        public async Task<PagedList<CustomerReviews>> GetReviewsByUserIdAsync(Guid userId, CustomerReviewParameter customerReviewParameter, bool trackChanges)
        {
            var query = FindByCondition(r => r.UserId == userId, trackChanges)
                .Include(r => r.User)
                .Include(r => r.BlindBox)
                .SearchByContent(customerReviewParameter.SearchByContent)
                .Sort(customerReviewParameter.OrderBy);

            var count = await query.CountAsync();

            var reviews = await query
                .Skip((customerReviewParameter.PageNumber - 1) * customerReviewParameter.PageSize)
                .Take(customerReviewParameter.PageSize)
                .ToListAsync();

            return new PagedList<CustomerReviews>(
                reviews,
                count,
                customerReviewParameter.PageNumber,
                customerReviewParameter.PageSize);
        }

        public async Task<PagedList<CustomerReviews>> GetReviewsByBlindBoxIdAsync(Guid blindBoxId, CustomerReviewParameter customerReviewParameter, bool trackChanges)
        {
            var query = FindByCondition(r => r.BlindBoxId == blindBoxId, trackChanges)
                .Include(r => r.User)
                .Include(r => r.BlindBox)
                .SearchByContent(customerReviewParameter.SearchByContent)
                .Sort(customerReviewParameter.OrderBy);

            var count = await query.CountAsync();

            var reviews = await query
                .Skip((customerReviewParameter.PageNumber - 1) * customerReviewParameter.PageSize)
                .Take(customerReviewParameter.PageSize)
                .ToListAsync();

            return new PagedList<CustomerReviews>(
                reviews,
                count,
                customerReviewParameter.PageNumber,
                customerReviewParameter.PageSize);
        }
    }
}
