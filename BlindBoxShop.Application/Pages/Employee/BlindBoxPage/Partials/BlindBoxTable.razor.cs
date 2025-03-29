using AutoMapper;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.Enum;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Globalization;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxPage.Partials
{
    public partial class BlindBoxTable : ComponentBase
    {
        private IEnumerable<BlindBoxDto>? pagedData;
        private IEnumerable<BlindBoxCategoryDto>? _categories = new List<BlindBoxCategoryDto>();
        private MudTable<BlindBoxDto>? table;
        private string? searchString;
        private BlindBoxDto? _blindBoxDto;
        private BlindBoxDto? _blindBoxDtoBeforeEdit;
        private Timer? _timer;
        private bool _disableRemoveBtn = true;

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

        private MetaData? _metaData { get; set; } = new MetaData();
        private BlindBoxParameter _blindBoxParameters = new BlindBoxParameter();

        protected override async Task OnInitializedAsync()
        {
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            using var blindBoxCategoryService = ServiceManager!.BlindBoxCategoryService;
            var categoryParams = new BlindBoxCategoryParameter { PageSize = 100 }; // Get all for dropdown
            var result = await blindBoxCategoryService.GetBlindBoxCategoriesAsync(categoryParams, false);
            
            if (result.IsSuccess)
            {
                _categories = result.Value;
            }
        }

        private async Task<TableData<BlindBoxDto>> ServerReload(TableState state, CancellationToken token)
        {
            _blindBoxParameters.PageNumber = state.Page + 1;
            _blindBoxParameters.PageSize = state.PageSize;
            _blindBoxParameters.SearchByName = searchString;
            
            if (state.SortLabel != null)
            {
                string sortDirection = state.SortDirection != SortDirection.Ascending ? "desc" : "";
                _blindBoxParameters.OrderBy = string.Join(" ", state.SortLabel, sortDirection).Trim();
            }
            
            using var blindBoxService = ServiceManager!.BlindBoxService;
            var result = await blindBoxService.GetBlindBoxesAsync(_blindBoxParameters, false);

            if (result.IsSuccess)
            {
                pagedData = result.Value;
                _metaData = result.Paging;
            }

            return new TableData<BlindBoxDto>() { TotalItems = _metaData!.TotalCount, Items = pagedData };
        }

        public async Task ReloadDataAsync()
        {
            if (table != null)
            {
                await table.ReloadServerData();
            }
        }

        private void SearchChanged()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }

            _timer = new Timer(OnTimerElapsed, null, 500, 0);
        }

        private async void OnTimerElapsed(object? state)
        {
            await InvokeAsync(async () =>
            {
                await OnSearch(searchString);
            });

            _timer!.Dispose();
        }

        private async Task OnSearch(string? text)
        {
            searchString = text;
            await table!.ReloadServerData();
        }

        private void BackupItem(object element)
        {
            if (element is BlindBoxDto dto)
            {
                _blindBoxDtoBeforeEdit = Mapper!.Map<BlindBoxDto>(dto);
            }
        }

        private void RowClickEvent(TableRowClickEventArgs<BlindBoxDto> tableRowClickEventArgs)
        {
            if (_blindBoxDto != null && _blindBoxDto.Equals(tableRowClickEventArgs.Item))
            {
                _disableRemoveBtn = false;
            }
            else
                _disableRemoveBtn = true;
        }

        private async void ItemHasBeenCommitted(object element)
        {
            if (element is not BlindBoxDto editedItem)
                return;

            if (!HasChanges(editedItem))
            {
                ShowSnackbar("No changes made. Update not performed.", Severity.Info);
                ResetItemToOriginalValues(element);
                return;
            }
            
            var blindBoxForUpdate = Mapper!.Map<BlindBoxForUpdate>(editedItem);
            if (blindBoxForUpdate is null)
            {
                ShowSnackbar("EditBlindBoxError", Severity.Error);
                return;
            }
            
            var blindBoxService = ServiceManager!.BlindBoxService;
            var result = await blindBoxService.UpdateBlindBoxAsync(editedItem.Id, blindBoxForUpdate, false);
            
            if (result.IsSuccess)
            {
                ShowSnackbar($"Edit BlindBox with name {blindBoxForUpdate.Name} successfully.", Severity.Success);
                await ReloadDataAsync();
            }
            else
            {
                var errorMessages = result.Errors?.Select(e => e.Description).ToList() ?? new List<string> { "Unknown error occurred" };
                ShowSnackbar(string.Join(", ", errorMessages), Severity.Error);
                ResetItemToOriginalValues(element);
            }
        }

        private bool HasChanges(BlindBoxDto currentItem)
        {
            return _blindBoxDtoBeforeEdit != null &&
                  (currentItem.Name != _blindBoxDtoBeforeEdit.Name ||
                   currentItem.Description != _blindBoxDtoBeforeEdit.Description ||
                   currentItem.Probability != _blindBoxDtoBeforeEdit.Probability ||
                   currentItem.Rarity != _blindBoxDtoBeforeEdit.Rarity ||
                   currentItem.Status != _blindBoxDtoBeforeEdit.Status ||
                   currentItem.BlindBoxCategoryId != _blindBoxDtoBeforeEdit.BlindBoxCategoryId ||
                   currentItem.PackageId != _blindBoxDtoBeforeEdit.PackageId ||
                   currentItem.CurrentPrice != _blindBoxDtoBeforeEdit.CurrentPrice);
        }

        private void ResetItemToOriginalValues(object element)
        {
            if (_blindBoxDtoBeforeEdit != null && element is BlindBoxDto dto)
            {
                Mapper!.Map(_blindBoxDtoBeforeEdit, dto);
            }
            
            if (_blindBoxDtoBeforeEdit != null && _blindBoxDtoBeforeEdit.Equals(table!.SelectedItem))
            {
                _disableRemoveBtn = true;
                table.SetSelectedItem(null);
            }
        }

        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, config => config.SnackbarVariant = Variant.Text);
        }

        private async Task OpenCreateDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium };
            var dialogResult = await (await DialogService!.ShowAsync<BlindBoxModalCreate>("Create BlindBox", options)).Result;
            
            if (!dialogResult!.Canceled)
            {
                await ReloadDataAsync();
            }
        }

        private async Task OpenRemoveDialogAsync()
        {
            var item = table!.SelectedItem;
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var parameters = new DialogParameters();
            parameters.Add("BlindBoxDto", item);

            var dialog = await DialogService!.ShowAsync<BlindBoxModalRemove>("Delete BlindBox", parameters, options);
            var dialogResult = await dialog.Result;
            
            if (!dialogResult!.Canceled)
            {
                await ReloadDataAsync();
            }
        }

        private string FormatPrice(decimal price)
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0}", price) + " ₫";
        }

        private Color GetRarityColor(BlindBoxRarity rarity)
        {
            return rarity switch
            {
                BlindBoxRarity.Common => Color.Default,
                BlindBoxRarity.Uncommon => Color.Info,
                BlindBoxRarity.Rare => Color.Warning,
                _ => Color.Default
            };
        }

        private Color GetStatusColor(BlindBoxStatus status)
        {
            return status switch
            {
                BlindBoxStatus.Available => Color.Success,
                BlindBoxStatus.Sold_Out => Color.Error,
                BlindBoxStatus.Coming_Soon => Color.Info,
                _ => Color.Default
            };
        }
    }
} 