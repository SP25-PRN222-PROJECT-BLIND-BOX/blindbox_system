using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Voucher;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;

namespace BlindBoxShop.Application.Pages.Employee.VoucherPage
{
    public partial class Voucher
    {
        [Inject]
        public IServiceManager ServiceManager { get; set; }

        public MetaData? MetaData { get; set; } = new MetaData();

        private VoucherParameter _voucherParameters = new VoucherParameter();

        public IEnumerable<VoucherDto> Vouchers { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await GetVouchers();
        }

        private async Task SelectedPage(int page)
        {
            _voucherParameters.PageNumber = page;
            await GetVouchers();
        }

        private async Task SetPageSize(int pageSize)
        {
            _voucherParameters.PageSize = pageSize;
            _voucherParameters.PageNumber = 1;

            await GetVouchers();
        }

        private async Task SearchChanged(string searchTerm)
        {
            _voucherParameters.PageNumber = 1;
            _voucherParameters.SearchById = searchTerm;

            await GetVouchers();
        }

        private async Task SortChanged(string orderBy)
        {
            _voucherParameters.OrderBy = orderBy;
            await GetVouchers();
        }

        private async Task GetVouchers()
        {
            var result = await ServiceManager.VoucherService.GetVouchersAsync(_voucherParameters, false);

            if (result.IsSuccess)
            {
                Vouchers = result.GetValue<IEnumerable<VoucherDto>>();
                MetaData = result.Paging;
            }
        }
    }
}
