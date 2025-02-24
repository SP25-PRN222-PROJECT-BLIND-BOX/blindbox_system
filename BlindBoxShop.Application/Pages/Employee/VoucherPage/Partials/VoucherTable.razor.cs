using AutoMapper;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Voucher;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.VoucherPage.Partials
{
    public partial class VoucherTable
    {
        private IEnumerable<VoucherDto>? pagedData;

        private MudTable<VoucherDto>? table;

        private string? searchString;

        private VoucherDto? _voucherDto;

        private VoucherDto? _voucherDtoBeforeEdit { get; set; }

        private Timer? _timer;

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        private MetaData? _metaData { get; set; } = new MetaData();


        private VoucherParameter _voucherParameters = new VoucherParameter();

        private bool _disableRemoveBtn = true;


        private async Task<TableData<VoucherDto>> ServerReload(TableState state, CancellationToken token)
        {

            _voucherParameters.PageNumber = state.Page + 1;
            _voucherParameters.PageSize = state.PageSize;
            _voucherParameters.SearchById = searchString;
            if (state.SortLabel != null)
            {
                string sortDirection = state.SortDirection != SortDirection.Ascending ? "desc" : "";
                _voucherParameters.OrderBy = string.Join(" ", state.SortLabel, sortDirection).Trim();

            }
            using var voucherService = ServiceManager!.VoucherService;
            var result = await voucherService.GetVouchersAsync(_voucherParameters, false);
            if (result.IsSuccess)
            {
                pagedData = result.GetValue<IEnumerable<VoucherDto>>();
                _metaData = result.Paging;
            }

            return new TableData<VoucherDto>() { TotalItems = _metaData!.TotalCount, Items = pagedData };
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
            _voucherDtoBeforeEdit = Mapper!.Map<VoucherDto>(element);
        }

        private void RowClickEvent(TableRowClickEventArgs<VoucherDto> tableRowClickEventArgs)
        {
            if (_voucherDto != null && _voucherDto.Equals(tableRowClickEventArgs.Item))
            {

                _disableRemoveBtn = false;

            }
            else
                _disableRemoveBtn = true;
            StateHasChanged();
        }

        private async void ItemHasBeenCommitted(object element)
        {
            var editedItem = (VoucherDto)element;

            if (!HasChanges(editedItem))
            {
                ShowVariant("No changes made. Update not performed.", Severity.Info);
                ResetItemToOriginalValues(editedItem);
                return;
            }
            var voucherForUpdate = Mapper!.Map<VoucherForUpdate>((VoucherDto)element);
            if (voucherForUpdate is null)
            {
                ShowVariant($"Edit category has Id {((VoucherDto)element).Id} failed.", Severity.Warning);
                return;
            }

            using var voucherService = ServiceManager!.VoucherService;
            var result = await voucherService.UpdateVoucherAsync(((VoucherDto)element).Id, voucherForUpdate);
            if (result.IsSuccess)
            {
                ShowVariant($"Edit category has Id {((VoucherDto)element).Id} successfully.", Severity.Success);
                _disableRemoveBtn = true;
                StateHasChanged();
            }

        }

        private bool HasChanges(VoucherDto currentItem)
        {
            return _voucherDtoBeforeEdit != null &&
                   (currentItem.EndDate!.Value.CompareTo(_voucherDtoBeforeEdit.EndDate) != 0 ||
                    currentItem.StartDate!.Value.CompareTo(_voucherDtoBeforeEdit.StartDate) != 0 ||
                    !currentItem.Type.Equals(_voucherDtoBeforeEdit.Type) ||
                    !currentItem.Value.Equals(_voucherDtoBeforeEdit.Value) ||
                    !currentItem.Status.Equals(_voucherDtoBeforeEdit.Status));
        }

        private void ResetItemToOriginalValues(object element)
        {
            if (_voucherDtoBeforeEdit != null)
            {
                Mapper!.Map(_voucherDtoBeforeEdit, element);
            }

            _disableRemoveBtn = true; 
            _voucherDto = null;
            table!.SetSelectedItem(null);
            StateHasChanged();
        }

        private void ShowVariant(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, c => c.SnackbarVariant = Variant.Text);
        }


        private async Task OpenCreateDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialogResult = await (await DialogService!.ShowAsync<VoucherModalCreate>("Create voucher", options)).Result;
            if (!dialogResult!.Canceled)
            {
                await ReloadDataAsync();
            }
        }

        private async Task OpenRemoveDialogAsync(Guid Id)
        {
            var parameter = new DialogParameters();
            parameter.Add("Id", Id);
            var dialog = await _dialogService.ShowAsync<ConfirmDeleteDialog>("Delete Confiamtion", parameter);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await ReloadDataAsync();
            }
        }
    }
}
