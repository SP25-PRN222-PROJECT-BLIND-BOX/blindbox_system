using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.Enum;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Manager.Package.Partials
{
    public partial class PackageModelRemove : ComponentBase
    {
        [Parameter]
        public PackageManageDto? PackageItem { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        /// <summary>
        /// Initialize data when component is loaded
        /// </summary>
        protected override void OnInitialized()
        {
            if (PackageItem != null)
            {
                _packageManageDto = PackageItem;
            }
        }

        private PackageManageDto? _packageManageDto { get; set; }

        /// <summary>
        /// Close dialog when user cancels
        /// </summary>
        private void Cancel() => MudDialog!.Cancel();

        /// <summary>
        /// Handle package deletion
        /// Check conditions and change package status to Remove
        /// </summary>
        private async Task DeleteAsync()
        {
            if (_packageManageDto != null)
            {
                try
                {
                    // Check if package is Opened, don't allow deletion
                    if (_packageManageDto.Type == PackageType.Opened)
                    {
                        ShowSnackbar($"Cannot delete package '{_packageManageDto.Name}' because it is currently opened.", Severity.Warning);
                        return;
                    }

                    var packageService = ServiceManager!.PackageService;
                    
                    // Check if package is being opened
                    var isOpenResult = await packageService.IsPackageOpenAsync(_packageManageDto.Id, false);
                    if (isOpenResult.IsSuccess && isOpenResult.Value)
                    {
                        ShowSnackbar($"Cannot delete package '{_packageManageDto.Name}' because it has BlindBoxes being opened.", Severity.Warning);
                        return;
                    }

                    // Create object to update package
                    var packageForUpdate = new PackageForUpdate
                    {
                        Name = _packageManageDto.Name,
                        Barcode = _packageManageDto.Barcode,
                        TotalBlindBox = _packageManageDto.TotalBlindBox,
                        CurrentTotalBlindBox = _packageManageDto.CurrentTotalBlindBox,
                        Type = PackageType.Remove
                    };

                    var result = await packageService.UpdatePackageAsync(_packageManageDto.Id, packageForUpdate, false);

                    if (result.IsSuccess)
                    {
                        MudDialog!.Close(DialogResult.Ok(true));
                        ShowSnackbar($"Package '{_packageManageDto.Name}' has been set to Remove status.", Severity.Success);
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

        /// <summary>
        /// Display snackbar notification
        /// </summary>
        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Add(message, severity);
        }
    }
}