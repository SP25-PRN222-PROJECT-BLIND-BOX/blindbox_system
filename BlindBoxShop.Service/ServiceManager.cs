using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IBlindBoxCategoryService> _blindBoxCategoryService;
        private readonly Lazy<IBlindBoxService> _blindBoxService;
        private readonly Lazy<IBlindBoxImageService> _blindBoxImageService;
        private readonly Lazy<IBlindBoxPriceHistoryService> _blindBoxPriceHistoryService;
        private readonly Lazy<ICustomerReviewsService> _customerReviewsService;
        private readonly Lazy<IOrderDetailService> _orderDetailService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IPackageService> _packageService;
        private readonly Lazy<IVoucherService> _voucherService;

        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, mapper));

            _blindBoxCategoryService = new Lazy<IBlindBoxCategoryService>(() => new BlindBoxCategoryService(repositoryManager, mapper));

            _blindBoxService = new Lazy<IBlindBoxService>(() => new BlindBoxService(repositoryManager, mapper));

            _blindBoxImageService = new Lazy<IBlindBoxImageService>(() => new BlindBoxImageService(repositoryManager, mapper));

            _blindBoxPriceHistoryService = new Lazy<IBlindBoxPriceHistoryService>(() => new BlindBoxPriceHistoryService(repositoryManager, mapper));

            _customerReviewsService = new Lazy<ICustomerReviewsService>(() => new CustomerReviewsService(repositoryManager, mapper));

            _orderDetailService = new Lazy<IOrderDetailService>(() => new OrderDetailService(repositoryManager, mapper));

            _orderService = new Lazy<IOrderService>(() => new OrderService(repositoryManager, mapper));

            _packageService = new Lazy<IPackageService>(() => new PackageService(repositoryManager, mapper));

            _voucherService = new Lazy<IVoucherService>(() => new VoucherService(repositoryManager, mapper));
        }

        public IUserService UserService => _userService.Value;

        public IBlindBoxService BlindBoxService => _blindBoxService.Value;

        public IBlindBoxImageService BlindBoxImageService => _blindBoxImageService.Value;

        public IBlindBoxPriceHistoryService BlindBoxPriceHistoryService => _blindBoxPriceHistoryService.Value;

        public IBlindBoxCategoryService BlindBoxCategoryService => _blindBoxCategoryService.Value;

        public ICustomerReviewsService CustomerReviewsService => _customerReviewsService.Value;

        public IOrderDetailService OrderDetailService => _orderDetailService.Value;

        public IOrderService OrderService => _orderService.Value;

        public IPackageService PackageService => _packageService.Value;

        public IVoucherService VoucherService => _voucherService.Value;
    }
}
