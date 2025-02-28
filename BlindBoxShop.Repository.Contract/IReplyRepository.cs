using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Features;
using System.Linq.Expressions;

namespace BlindBoxShop.Repository.Contract
{
    public interface IReplyRepository : IRepositoryBase<ReplyReviews>
    {
        Task<PagedList<ReplyReviews>> GetRepliesAsync(ReplyParameter replyParameter, bool trackChanges);
        Task<PagedList<ReplyReviews>> GetRepliesByReviewIdAsync(Guid reviewId, ReplyParameter replyParameter, bool trackChanges);
        Task<ReplyReviews?> FindAsync(Expression<Func<ReplyReviews, bool>> predicate);
    }
}
