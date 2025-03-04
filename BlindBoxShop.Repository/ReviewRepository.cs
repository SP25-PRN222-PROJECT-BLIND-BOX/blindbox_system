using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Repository.Extensions;
using BlindBoxShop.Shared.Features;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlindBoxShop.Repository
{
    public class ReviewRepository : RepositoryBase<CustomerReviews>, IReviewRepository
    {
        public ReviewRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<CustomerReviews?> FindAsync(Expression<Func<CustomerReviews, bool>> predicate)
        {
            return await FindByCondition(predicate, trackChanges: false)
                .Include(r => r.User)
                .Include(r => r.BlindBox)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedList<CustomerReviews>> GetReviewsAsync(ReviewParameter reviewParameter, bool trackChanges)
        {
            var reviews = await FindAll(trackChanges)
                .Include(r => r.User)
                .Include(r => r.BlindBox)
                .SearchById(reviewParameter.SearchById)
                .SearchByContent(reviewParameter.SearchByContent)
                .SearchByUsername(reviewParameter.SearchByUsername)
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

        public async Task<PagedList<CustomerReviews>> GetReviewsByUserIdAsync(Guid userId, ReviewParameter customerReviewParameter, bool trackChanges)
        {
            var query = FindByCondition(r => r.UserId == userId, trackChanges)
                .Include(r => r.User)
                .Include(r => r.BlindBox)
                .SearchByUsername(customerReviewParameter.SearchByUsername)
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

        public async Task<PagedList<CustomerReviews>> GetReviewsByBlindBoxIdAsync(Guid blindBoxId, ReviewParameter customerReviewParameter, bool trackChanges)
        {
            var query = FindByCondition(r => r.BlindBoxId == blindBoxId, trackChanges)
                .Include(r => r.User)
                .Include(r => r.BlindBox)
                .SearchByUsername(customerReviewParameter.SearchByUsername)
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
