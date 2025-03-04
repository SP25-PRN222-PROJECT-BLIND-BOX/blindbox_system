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

        IReviewRepository Review { get; }

        IOrderDetailRepository OrderDetail { get; }

        IPackageRepository Package { get; }

        IVoucherRepository Voucher { get; }

        IReplyRepository Replie { get; }

        IOrderRepository Order { get; }
    }
}
