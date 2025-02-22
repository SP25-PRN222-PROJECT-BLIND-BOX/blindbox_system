using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class ReplyReviewService : BaseService, IReplyReviewsService
    {
        private readonly IReplyReviewsRepository _replyReviewsRepository;
        public ReplyReviewService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _replyReviewsRepository = repositoryManager.ReplyReviews;
        }

        public void Dispose()
        {
            _replyReviewsRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
