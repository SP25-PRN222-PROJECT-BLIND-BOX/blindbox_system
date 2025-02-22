using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;


        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public IUserService UserService => new UserService(_repositoryManager, _mapper);

        public IBlindBoxService BlindBoxService => new BlindBoxService(_repositoryManager, _mapper);

        public IBlindBoxImageService BlindBoxImageService => new BlindBoxImageService(_repositoryManager, _mapper);

        public IBlindBoxPriceHistoryService BlindBoxPriceHistoryService => new BlindBoxPriceHistoryService(_repositoryManager, _mapper);

        public IBlindBoxCategoryService BlindBoxCategoryService => new BlindBoxCategoryService(_repositoryManager, _mapper);

        public ICustomerReviewsService CustomerReviewsService => new CustomerReviewsService(_repositoryManager, _mapper);

        public IOrderDetailService OrderDetailService => new OrderDetailService(_repositoryManager, _mapper);

        public IOrderService OrderService => new OrderService(_repositoryManager, _mapper);

        public IPackageService PackageService => new PackageService(_repositoryManager, _mapper);

        public IVoucherService VoucherService => new VoucherService(_repositoryManager, _mapper);
    }
}
