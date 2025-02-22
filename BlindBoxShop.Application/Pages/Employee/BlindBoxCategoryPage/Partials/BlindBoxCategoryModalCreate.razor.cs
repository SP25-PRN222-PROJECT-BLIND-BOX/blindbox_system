using BlindBoxShop.Service;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxCategoryPage.Partials
{
    public partial class BlindBoxCategoryModalCreate
    {
        private BlindBoxCategoryForCreate? _blindBoxCategoryForCreate = new BlindBoxCategoryForCreate();

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Inject]
        private IDialogService? DialogService { get; set; }



        private async Task ValidSubmit(EditContext context)
        {
            using var blindBoxCategoryService = ServiceManager!.BlindBoxCategoryService;
            var result = await blindBoxCategoryService.CreateBlindBoxCategoryAsync(_blindBoxCategoryForCreate!);

            if (result.IsSuccess)
            {
                _blindBoxCategoryForCreate = new();
                MudDialog!.Close(DialogResult.Ok(result.GetValue<BlindBoxCategoryDto>()));
                ShowVariant("Create category successfully.", Severity.Info);
            }
            else
            {
                var errorsMessage = result.Errors!.Select(e => e.Description).ToList();
                var errorMeesage = string.Join(", ", errorsMessage).Trim();
                ShowVariant(errorMeesage, Severity.Warning);
            }
        }

        private async Task InvalidSubmit(EditContext context)
        {
            await DialogService!.ShowMessageBox(
                            "Sorry",
                            @"You must fill all the field!",
                            yesText: "Got it",
                            options: new DialogOptions() { CloseOnEscapeKey = true });
            return;
        }



        private void Cancel() => MudDialog!.Cancel();

        private async Task OnKeyDownAsync(KeyboardEventArgs args, EditContext context)
        {
            switch (args.Key)
            {
                case "Enter":
                case "NumpadEnter":
                    if (context.Validate())
                    {
                        await ValidSubmit(context);
                    }
                    else
                    {
                        await InvalidSubmit(context);
                    }
                    break;
                case "Escape":
                    {
                        Cancel();
                        break;
                    }
            }
        }

        private void ShowVariant(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, c => c.SnackbarVariant = Variant.Text);
        }
    }
}