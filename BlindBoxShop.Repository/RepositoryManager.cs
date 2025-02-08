using BlindBoxShop.Repository.Contract;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlindBoxShop.Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<IBlindBoxCategoryRepository> _blindBoxCategoryRepository;
        private readonly Lazy<IBlindBoxImageRepository> _blindBoxImageRepository;
        private readonly Lazy<IBlindBoxPriceHistoryRepository> _blindBoxPriceHistoryRepository;
        private readonly Lazy<IBlindBoxRepository> _blindBoxRepository;
        private readonly Lazy<ICustomerReviewsRepository> _customerReviewsRepository;
        private readonly Lazy<IOrderDetailRepository> _orderDetailRepository;
        private readonly Lazy<IOrderRepository> _orderRepository;
        private readonly Lazy<IPackageRepository> _packageRepository;
        private readonly Lazy<IVoucherRepository> _voucherRepository;
        private readonly Lazy<IUserRepository> _userRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;

            _blindBoxCategoryRepository = new Lazy<IBlindBoxCategoryRepository>(() => new BlindBoxCategoryRepository(repositoryContext));
            _blindBoxImageRepository = new Lazy<IBlindBoxImageRepository>(() => new BlindBoxImageRepository(repositoryContext));
            _blindBoxPriceHistoryRepository = new Lazy<IBlindBoxPriceHistoryRepository>(() => new BlindBoxPriceHistoryRepository(repositoryContext));
            _blindBoxRepository = new Lazy<IBlindBoxRepository>(() => new BlindBoxRepository(repositoryContext));
            _customerReviewsRepository = new Lazy<ICustomerReviewsRepository>(() => new CustomerReviewsRepository(repositoryContext));
            _orderDetailRepository = new Lazy<IOrderDetailRepository>(() => new OrderDetailRepository(repositoryContext));
            _orderRepository = new Lazy<IOrderRepository>(() => new OrderRepository(repositoryContext));
            _packageRepository = new Lazy<IPackageRepository>(() => new PackageRepository(repositoryContext));
            _voucherRepository = new Lazy<IVoucherRepository>(() => new VoucherRepository(repositoryContext));
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(repositoryContext));
        }

        public IUserRepository User => _userRepository.Value;

        public IBlindBoxCategoryRepository BlindBoxCategory => _blindBoxCategoryRepository.Value;

        public IBlindBoxImageRepository BlindBoxImage => _blindBoxImageRepository.Value;

        public IBlindBoxPriceHistoryRepository BlindBoxPriceHistory => _blindBoxPriceHistoryRepository.Value;

        public IBlindBoxRepository BlindBox => _blindBoxRepository.Value;

        public ICustomerReviewsRepository CustomerReviews => _customerReviewsRepository.Value;

        public IOrderDetailRepository OrderDetail => _orderDetailRepository.Value;

        public IOrderRepository Order => _orderRepository.Value;

        public IPackageRepository Package => _packageRepository.Value;

        public IVoucherRepository Voucher => _voucherRepository.Value;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _repositoryContext.Database.BeginTransactionAsync();
        }

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _repositoryContext.Database.CreateExecutionStrategy();
        }

        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
        public void Save() => _repositoryContext.SaveChanges();
    }
}
