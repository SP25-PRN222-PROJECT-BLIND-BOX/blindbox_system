using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class ReplyReviewsRepository : RepositoryBase<ReplyReviews>, IReplyReviewsRepository
    {
        public ReplyReviewsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
