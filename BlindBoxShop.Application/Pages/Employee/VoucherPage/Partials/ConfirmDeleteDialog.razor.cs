using BlindBoxShop.Service.Contract;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.VoucherPage.Partials
{
    public partial class ConfirmDeleteDialog
    {
        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter] public Guid Id { get; set; }

        private async void Submit()
        {
            using var voucherService = ServiceManager!.VoucherService;
            await voucherService.DeleteVoucherAsync(Id);
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
