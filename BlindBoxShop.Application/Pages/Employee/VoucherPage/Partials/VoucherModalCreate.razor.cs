using Blazored.Toast.Services;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Voucher;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace BlindBoxShop.Application.Pages.Employee.VoucherPage.Partials
{
    public partial class VoucherModalCreate : IDisposable
    {
        [Inject]
        public IServiceManager ServiceManager { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }

        private VoucherForCreate? _voucherForCreate = new VoucherForCreate();

        [NotNull]
        private Modal? DragModal { get; set; }

        private async Task Create()
        {
            var result = await ServiceManager.VoucherService.CreateVoucherAsync(_voucherForCreate!);

            if (result.IsSuccess)
            {
                ToastService.ShowSuccess($"Action successful. Voucher \"{_voucherForCreate.Value}\" successfully added.");

                await DragModal.Toggle();
                _voucherForCreate = new();
            }
        }

        public void Dispose()
        {
        }
    }
}
