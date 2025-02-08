namespace BlindBoxShop.Service.Contract
{
    public interface IServiceManager
    {
        IUserService UserService { get; }

        IBlindBoxService BlindBoxService { get; }

        IBlindBoxImageService BlindBoxImageService { get; }

        IBlindBoxPriceHistoryService BlindBoxPriceHistoryService { get; }

        IBlindBoxCategoryService BlindBoxCategoryService { get; }

        ICustomerReviewsService CustomerReviewsService { get; }

        IOrderDetailService OrderDetailService { get; }

        IOrderService OrderService { get; }

        IPackageService PackageService { get; }

        IVoucherService VoucherService { get; }

    }
}
