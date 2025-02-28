using BlindBoxShop.Repository.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlindBoxShop.Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly IDbContextFactory<RepositoryContext> _dbContextFactory;


        public RepositoryManager(IDbContextFactory<RepositoryContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IUserRepository User
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new UserRepository(context);
            }
        }

        public IBlindBoxCategoryRepository BlindBoxCategory
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new BlindBoxCategoryRepository(context);
            }
        }

        public IBlindBoxImageRepository BlindBoxImage
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new BlindBoxImageRepository(context);
            }
        }

        public IBlindBoxPriceHistoryRepository BlindBoxPriceHistory
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new BlindBoxPriceHistoryRepository(context);
            }
        }

        public IBlindBoxRepository BlindBox
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new BlindBoxRepository(context);
            }
        }

        public ICustomerReviewsRepository CustomerReviews
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new CustomerReviewRepository(context);
            }
        }

        public IOrderDetailRepository OrderDetail
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new OrderDetailRepository(context);
            }
        }

        public IOrderRepository Order
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new OrderRepository(context);
            }
        }
        public IPackageRepository Package
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new PackageRepository(context);
            }
        }

        public IVoucherRepository Voucher
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new VoucherRepository(context);
            }
        }

        public IReplyReviewsRepository ReplyReviews
        {
            get
            {
                var context = _dbContextFactory.CreateDbContext();
                return new ReplyReviewsRepository(context);
            }
        }

        public async Task ExecuteInTransactionAsync(Func<RepositoryContext, Task> operation)
        {
            // Tạo một instance DbContext từ factory
            using var context = _dbContextFactory.CreateDbContext();
            // Bắt đầu giao dịch
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Thực hiện các thao tác với cùng một DbContext
                await operation(context);
                // Lưu các thay đổi
                await context.SaveChangesAsync();
                // Commit giao dịch nếu thành công
                await transaction.CommitAsync();
            }
            catch
            {
                // Rollback nếu có lỗi
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
