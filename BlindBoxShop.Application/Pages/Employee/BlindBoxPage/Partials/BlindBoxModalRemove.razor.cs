using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.Extension;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxPage.Partials
{
    public partial class BlindBoxModalRemove : ComponentBase
    {
        [Parameter]
        public BlindBoxDto? BlindBoxDto { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        private void Cancel() => MudDialog!.Cancel();

        private async Task DeleteAsync()
        {
            if (BlindBoxDto != null)
            {
                try
                {
                    var blindBoxService = ServiceManager!.BlindBoxService;
                    var result = await blindBoxService.DeleteBlindBoxAsync(BlindBoxDto.Id, false);

                    if (result.IsSuccess)
                    {
                        MudDialog!.Close(DialogResult.Ok(true));
                        ShowSnackbar($"BlindBox '{BlindBoxDto.Name}' has been deleted successfully.", Severity.Success);
                    }
                    else
                    {
                        var errorsMessage = result.Errors!.Select(e => e.Description).ToList();
                        var errorMessage = string.Join(", ", errorsMessage).Trim();
                        ShowSnackbar(errorMessage, Severity.Error);
                    }
                }
                catch (Exception ex)
                {
                    ShowSnackbar(ex.Message, Severity.Error);
                }
            }
        }

        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, config => config.SnackbarVariant = Variant.Text);
        }
    }
} 