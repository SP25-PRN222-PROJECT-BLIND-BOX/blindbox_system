using Microsoft.EntityFrameworkCore.Storage;

namespace BlindBoxShop.Repository.Contract
{
    public interface IRepositoryManager
    {
        Task<IDbContextTransaction> BeginTransactionAsync();

        IExecutionStrategy CreateExecutionStrategy();

        Task SaveAsync();

        void Save();

        IUserRepository User { get; }

        IBlindBoxCategoryRepository BlindBoxCategory { get; }

        IBlindBoxImageRepository BlindBoxImage { get; }

        IBlindBoxPriceHistoryRepository BlindBoxPriceHistory { get; }

        IBlindBoxRepository BlindBox { get; }

        ICustomerReviewsRepository CustomerReviews { get; }

        IOrderDetailRepository OrderDetail { get; }

        IOrderRepository Order { get; }

        IPackageRepository Package { get; }

        IVoucherRepository Voucher { get; }
    }
}
