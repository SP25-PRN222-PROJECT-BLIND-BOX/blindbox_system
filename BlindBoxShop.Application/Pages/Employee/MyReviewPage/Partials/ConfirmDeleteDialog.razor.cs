using BlindBoxShop.Service.Contract;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.MyReviewPage.Partials
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
            using var reviewService = ServiceManager!.CustomerReviewsService;
            await reviewService.DeleteReviewAsync(Id);
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
