using AutoMapper;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxCategoryPage.Partials
{
    public partial class BlindBoxCategoryTable
    {
        private IEnumerable<BlindBoxCategoryDto>? pagedData;

        private MudTable<BlindBoxCategoryDto>? table;

        private string? searchString;

        private BlindBoxCategoryDto? _blindBoxCategoryDto;

        private BlindBoxCategoryDto? _blindBoxCategoryDtoBeforeEdit;

        private Timer? _timer;

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        private MetaData? _metaData { get; set; } = new MetaData();


        private BlindBoxCategoryParameter _blindBoxCategoryParameters = new BlindBoxCategoryParameter();

        [Parameter]
        public RenderFragment? CreateButton { get; set; }

        [Parameter]
        public RenderFragment? RemoveButton { get; set; }

        private bool _disableRemoveBtn = true;


        private async Task<TableData<BlindBoxCategoryDto>> ServerReload(TableState state, CancellationToken token)
        {

            _blindBoxCategoryParameters.PageNumber = state.Page + 1;
            _blindBoxCategoryParameters.PageSize = state.PageSize;
            _blindBoxCategoryParameters.SearchByName = searchString;
            if (state.SortLabel != null)
            {
                string sortDirection = state.SortDirection != SortDirection.Ascending ? "desc" : "";
                _blindBoxCategoryParameters.OrderBy = string.Join(" ", state.SortLabel, sortDirection).Trim();

            }
            var result = await ServiceManager!.BlindBoxCategoryService.GetBlindBoxCategoriesAsync(_blindBoxCategoryParameters, false);

            if (result.IsSuccess)
            {
                pagedData = result.GetValue<IEnumerable<BlindBoxCategoryDto>>();
                _metaData = result.Paging;
            }


            return new TableData<BlindBoxCategoryDto>() { TotalItems = _metaData!.TotalCount, Items = pagedData };
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
            _blindBoxCategoryDtoBeforeEdit = Mapper!.Map<BlindBoxCategoryDto>(element);
        }

        private void RowClickEvent(TableRowClickEventArgs<BlindBoxCategoryDto> tableRowClickEventArgs)
        {
            if (_blindBoxCategoryDto != null && _blindBoxCategoryDto.Equals(tableRowClickEventArgs.Item))
            {

                _disableRemoveBtn = false;

            }
            else
                _disableRemoveBtn = true;
        }

        private async void ItemHasBeenCommitted(object element)
        {
            var editedItem = (BlindBoxCategoryDto)element;

            if (!HasChanges(editedItem))
            {
                ShowVariant("No changes made. Update not performed.", Severity.Info);
                ResetItemToOriginalValues(editedItem);
                return;
            }
            var blindBoxCategoryForUpdate = Mapper!.Map<BlindBoxCategoryForUpdate>((BlindBoxCategoryDto)element);
            if (blindBoxCategoryForUpdate is null)
            {
                ShowVariant($"Edit category has name {((BlindBoxCategoryDto)element).Name} failed.", Severity.Warning);
                return;
            }
            var result = await ServiceManager!.BlindBoxCategoryService.UpdateBlindBoxCategoryAsync(((BlindBoxCategoryDto)element).Id, blindBoxCategoryForUpdate);
            if (result.IsSuccess)
            {
                ShowVariant($"Edit category has name {blindBoxCategoryForUpdate.Name} successfully.", Severity.Success);
            }
        }

        private bool HasChanges(BlindBoxCategoryDto currentItem)
        {
            return _blindBoxCategoryDtoBeforeEdit != null &&
                   (currentItem.Name != _blindBoxCategoryDtoBeforeEdit.Name ||
                    currentItem.Description != _blindBoxCategoryDtoBeforeEdit.Description);
        }

        private void ResetItemToOriginalValues(object element)
        {
            if (_blindBoxCategoryDtoBeforeEdit != null)
            {
                Mapper!.Map(_blindBoxCategoryDtoBeforeEdit, element);
            }
            if (_blindBoxCategoryDtoBeforeEdit!.Equals(table!.SelectedItem))
            {
                _disableRemoveBtn = true;
                table.SetSelectedItem(null);
            }
        }

        private void ShowVariant(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, c => c.SnackbarVariant = Variant.Text);
        }


        private async Task OpenCreateDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialogResult = await (await DialogService!.ShowAsync<BlindBoxCategoryModalCreate>("Create Category", options)).Result;
            if (!dialogResult!.Canceled)
            {
                await ReloadDataAsync();
            }
        }

        private async Task OpenRemoveDialogAsync()
        {
            var item = table!.SelectedItem;
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var parameters = new DialogParameters<BlindBoxCategoryModalRemove> { { x => x.BlindBoxCategoryDto, item } };

            var dialog = await DialogService!.ShowAsync<BlindBoxCategoryModalRemove>("Delete category", parameters, options);
            var dialogResult = await dialog.Result;
            if (!dialogResult!.Canceled)
            {
                await ReloadDataAsync();
            }
        }

    }
}