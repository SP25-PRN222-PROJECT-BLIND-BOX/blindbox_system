using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class ReplyReviewService : BaseService, IReplyReviewsService
    {
        public ReplyReviewService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
        }
    }
}
