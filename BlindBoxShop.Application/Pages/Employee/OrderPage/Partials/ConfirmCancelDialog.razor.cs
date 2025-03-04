using BlindBoxShop.Service.Contract;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.OrderPage.Partials
{
    public partial class ConfirmCancelDialog
    {
        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter] public Guid Id { get; set; }

        private async void Submit()
        {
            using var orderService = ServiceManager!.OrderService;
            await orderService.CancelOrderAsync(Id);
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
