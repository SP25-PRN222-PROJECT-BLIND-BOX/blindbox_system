using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class CustomerReviewsService : BaseService, ICustomerReviewsService
    {
        private readonly ICustomerReviewsRepository _customerReviewsRepository;
        public CustomerReviewsService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _customerReviewsRepository = repositoryManager.CustomerReviews;
        }

        public void Dispose()
        {
            _customerReviewsRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
