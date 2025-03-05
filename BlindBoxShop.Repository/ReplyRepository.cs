using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Repository.Extensions;
using BlindBoxShop.Shared.Features;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlindBoxShop.Repository
{
    public class ReplyRepository : RepositoryBase<ReplyReviews>, IReplyRepository
    {
        public ReplyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<ReplyReviews?> FindAsync(Expression<Func<ReplyReviews, bool>> predicate)
        {
            return await FindByCondition(predicate, trackChanges: false)
                .Include(r => r.User)
                .Include(r => r.CustomerReviews)
                .FirstOrDefaultAsync();
        }

        // 1. Get all replies (Paged List)
        public async Task<PagedList<ReplyReviews>> GetRepliesAsync(ReplyParameter replyParameter, bool trackChanges)
        {
            var replies = await FindAll(trackChanges)
                .Include(r => r.User)
                .Include(r => r.CustomerReviews)
                .SearchByContent(replyParameter.SearchByReply)
                .SearchByUsername(replyParameter.SearchByUsername)
                .Sort(replyParameter.OrderBy)
                .Skip((replyParameter.PageNumber - 1) * replyParameter.PageSize)
                .Take(replyParameter.PageSize)
                .ToListAsync();

            var count = await FindAll(trackChanges)
                .SearchByContent(replyParameter.SearchByReply)
                .CountAsync();

            return new PagedList<ReplyReviews>(
                replies,
                count,
                replyParameter.PageNumber,
                replyParameter.PageSize);
        }

        // Get reply by review ID
        public async Task<ReplyReviews?> GetReplyByReviewIdAsync(Guid reviewId, bool trackChanges)
        {
            var reply = await FindByCondition(r => r.CustomerReviewsId == reviewId, trackChanges)
                .Include(r => r.User) // Include User details
                .Include(r => r.CustomerReviews) // Include related CustomerReviews
                .FirstOrDefaultAsync();

            return reply;
        }

        // 3. Get replies by user ID
        public async Task<PagedList<ReplyReviews>> GetRepliesByUserIdAsync(Guid userId, ReplyParameter replyParameter, bool trackChanges)
        {
            var query = FindByCondition(r => r.UserId == userId, trackChanges)
                .Include(r => r.User)
                .Include(r => r.CustomerReviews)
                .SearchByContent(replyParameter.SearchByReply)
                .SearchByUsername(replyParameter.SearchByUsername)
                .Sort(replyParameter.OrderBy);

            var count = await query.CountAsync();

            var replies = await query
                .Skip((replyParameter.PageNumber - 1) * replyParameter.PageSize)
                .Take(replyParameter.PageSize)
                .ToListAsync();

            return new PagedList<ReplyReviews>(
                replies,
                count,
                replyParameter.PageNumber,
                replyParameter.PageSize);
        }
    }
}
