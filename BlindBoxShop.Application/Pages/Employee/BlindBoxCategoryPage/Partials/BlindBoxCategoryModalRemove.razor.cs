using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxCategoryPage.Partials
{
    public partial class BlindBoxCategoryModalRemove
    {
        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        private IDialogService? DialogService { get; set; }

        [Parameter]
        public BlindBoxCategoryDto? BlindBoxCategoryDto { get; set; }

        private void Cancel() => MudDialog!.Cancel();

        private async void DeleteCategory()
        {
            if (BlindBoxCategoryDto is null)
            {
                await ErrorsMessageBox();
                Cancel();
            }
            using var blindBoxCategoryService = ServiceManager!.BlindBoxCategoryService;
            var result = await blindBoxCategoryService.DeleteBlindBoxCategoryAsync(BlindBoxCategoryDto!.Id);
            if (result.IsSuccess)
            {
                Snackbar.Add($"Category name {BlindBoxCategoryDto.Name} Deleted successfully.", Severity.Success);
                MudDialog!.Close(DialogResult.Ok(BlindBoxCategoryDto.Id));
            }
            else
            {
                var errorsMessage = result.Errors!.Select(e => e.Description).ToList();
                var errorMeesage = string.Join(", ", errorsMessage).Trim();
                await ErrorsMessageBox(errorMeesage);
                Cancel();
            }

        }


        private async Task ErrorsMessageBox()
        {
            await DialogService!.ShowMessageBox(
                            "Errors",
                            @"You must select the category to remove!",
                            yesText: "Got it",
                            options: new DialogOptions() { CloseOnEscapeKey = true });
            return;
        }

        private async Task ErrorsMessageBox(string message)
        {

            await DialogService!.ShowMessageBox(
                            "Errors",
                            message,
                            yesText: "Got it",
                            options: new DialogOptions() { CloseOnEscapeKey = true });
            return;
        }

    }
}