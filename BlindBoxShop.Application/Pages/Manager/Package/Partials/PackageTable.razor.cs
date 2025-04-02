using System.Globalization;
using AutoMapper;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Manager.Package.Partials
{
    public partial class PackageTable : ComponentBase
    {
        private IEnumerable<PackageManageDto>? pagedData;
        private MudTable<PackageManageDto>? table;
        private string? searchString;
        private PackageManageDto? _packageDtoBeforeEdit;
        private Timer? _timer;

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

        private MetaData? _metaData { get; set; } = new MetaData();
        private PackageParameter _packageParameters = new PackageParameter();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _packageParameters.PageSize = 10; // Set default page size
        }

        /// <summary>
        /// Load table data from server with pagination and sorting support
        /// </summary>
        private async Task<TableData<PackageManageDto>> ServerReload(TableState state, CancellationToken token)
        {
            try
            {
                _packageParameters.PageNumber = state.Page + 1;
                _packageParameters.PageSize = state.PageSize;
                _packageParameters.SearchByName = searchString?.Trim();

                using var packageService = ServiceManager!.PackageService;
                var result = await packageService.GetAllPackagesAsync(_packageParameters, false);

                if (result.IsSuccess && result.Value != null)
                {
                    pagedData = result.Value;
                    var totalItems = await packageService.GetTotalCountAsync(_packageParameters, false);
                    return new TableData<PackageManageDto>()
                    {
                        TotalItems = totalItems,
                        Items = pagedData
                    };
                }

                ShowSnackbar("Failed to load package data", Severity.Error);
                return new TableData<PackageManageDto>() { TotalItems = 0, Items = Enumerable.Empty<PackageManageDto>() };
            }
            catch (Exception ex)
            {
                ShowSnackbar($"Error: {ex.Message}", Severity.Error);
                return new TableData<PackageManageDto>() { TotalItems = 0, Items = Enumerable.Empty<PackageManageDto>() };
            }
        }

        /// <summary>
        /// Reload table data and reset related values
        /// </summary>
        public async Task ReloadDataAsync()
        {
            if (table != null)
            {
                await table.ReloadServerData();
            }
        }

        /// <summary>
        /// Handle search text change event
        /// </summary>
        private async Task OnSearch(string? text)
        {
            searchString = text;
            if (table != null)
            {
                table.CurrentPage = 0; // Reset to first page
                await table.ReloadServerData();
            }
        }

        /// <summary>
        /// Handle type filter change event
        /// </summary>
        private async Task OnTypeChanged(int? type)
        {
            _packageParameters.Type = type;
            if (table != null)
            {
                table.CurrentPage = 0; // Reset to first page
                await table.ReloadServerData();
            }
        }

        /// <summary>
        /// Backup package data before editing
        /// </summary>
        private void BackupItem(object element)
        {
            if (element is PackageManageDto dto)
            {
                _packageDtoBeforeEdit = Mapper!.Map<PackageManageDto>(dto);
            }
        }

        /// <summary>
        /// Handle when user commits package edit
        /// Check conditions and perform update
        /// </summary>
        private async void ItemHasBeenCommitted(object element)
        {
            if (element is not PackageManageDto editedItem)
                return;

            // Check if package is Opened, don't allow update
            if (editedItem.Type == PackageType.Opened)
            {
                ShowSnackbar($"Cannot update package '{editedItem.Name}' because it is currently opened.", Severity.Warning);
                ResetItemToOriginalValues(element);
                return;
            }

            if (!HasChanges(editedItem))
            {
                ShowSnackbar("No changes made. Update not performed.", Severity.Info);
                ResetItemToOriginalValues(element);
                return;
            }

            var packageForUpdate = Mapper!.Map<PackageForUpdate>(editedItem);
            if (packageForUpdate is null)
            {
                ShowSnackbar("Failed to update package", Severity.Error);
                return;
            }

            var packageService = ServiceManager!.PackageService;
            var result = await packageService.UpdatePackageAsync(editedItem.Id, packageForUpdate, false);

            if (result.IsSuccess)
            {
                ShowSnackbar($"Package {packageForUpdate.Name} updated successfully.", Severity.Success);
                await ReloadDataAsync();
            }
            else
            {
                var errorMessages = result.Errors?.Select(e => e.Description).ToList() ?? new List<string> { "Unknown error occurred" };
                ShowSnackbar(string.Join(", ", errorMessages), Severity.Error);
                ResetItemToOriginalValues(element);
            }
        }

        /// <summary>
        /// Check if package has changes compared to backup data
        /// </summary>
        private bool HasChanges(PackageManageDto currentItem)
        {
            return _packageDtoBeforeEdit != null &&
                  (currentItem.Name != _packageDtoBeforeEdit.Name ||
                   currentItem.CurrentTotalBlindBox != _packageDtoBeforeEdit.CurrentTotalBlindBox ||
                   currentItem.Type != _packageDtoBeforeEdit.Type ||
                   currentItem.TotalBlindBox != _packageDtoBeforeEdit.TotalBlindBox);
        }

        private void ResetItemToOriginalValues(object element)
        {
            if (_packageDtoBeforeEdit != null && element is PackageManageDto dto)
            {
                Mapper!.Map(_packageDtoBeforeEdit, dto);
            }
        }

        /// <summary>
        /// Display snackbar notification
        /// </summary>
        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, config => config.SnackbarVariant = Variant.Text);
        }

        /// <summary>
        /// Open create package dialog
        /// </summary>
        private async Task OpenCreateDialogAsync()
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                Position = DialogPosition.Center,
                NoHeader = false
            };

            var dialog = await DialogService!.ShowAsync<PackageCreateModal>("Create Package", options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await ResetTableState();
            }
        }

        /// <summary>
        /// Open remove package dialog
        /// </summary>
        private async Task OpenRemoveDialogAsync(PackageManageDto package)
        {
            if (package == null) return;

            var options = new DialogOptions { CloseOnEscapeKey = true };
            var parameters = new DialogParameters();
            parameters.Add("PackageItem", package);

            var dialog = await DialogService!.ShowAsync<PackageModelRemove>("Delete Package", parameters, options);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Canceled)
            {
                await ResetTableState();
            }
        }

        private async Task ResetTableState()
        {
            if (table != null)
            {
                await table.ReloadServerData();
            }
        }

        /// <summary>
        /// Format price in Vietnamese currency
        /// </summary>
        private string FormatPrice(decimal price)
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0}", price) + " ₫";
        }

        /// <summary>
        /// Get display color for each package type
        /// </summary>
        private Color GetTypeColor(PackageType type)
        {
            return type switch
            {
                PackageType.Opened => Color.Success,
                PackageType.Standard => Color.Warning,
                _ => Color.Default
            };
        }
    }
}