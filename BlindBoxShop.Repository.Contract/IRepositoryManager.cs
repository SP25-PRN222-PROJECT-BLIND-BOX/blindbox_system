using Microsoft.EntityFrameworkCore.Storage;

namespace BlindBoxShop.Repository.Contract
{
    public interface IRepositoryManager
    {

        IUserRepository User { get; }

        IBlindBoxCategoryRepository BlindBoxCategory { get; }

        IBlindBoxImageRepository BlindBoxImage { get; }

        IBlindBoxPriceHistoryRepository BlindBoxPriceHistory { get; }

        IBlindBoxRepository BlindBox { get; }

        ICustomerReviewRepository CustomerReviews { get; }

        IOrderDetailRepository OrderDetail { get; }

        IOrderRepository Order { get; }

        IPackageRepository Package { get; }

        IVoucherRepository Voucher { get; }

        IReplyRepository ReplyReviews { get; }
    }
}
