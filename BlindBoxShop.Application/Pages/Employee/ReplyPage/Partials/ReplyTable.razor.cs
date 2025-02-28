using AutoMapper;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Reply;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.ReplyPage.Partials
{
    public partial class ReplyTable
    {
        private IEnumerable<ReplyDto>? pagedData;

        private MudTable<ReplyDto>? table;

        private string? searchString;

        private ReplyDto? _replyDto;

        private ReplyDto? _replyDtoBeforeEdit;

        private Timer? _timer;

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        private MetaData? _metaData { get; set; } = new MetaData();

        private ReplyParameter _replyParameters = new ReplyParameter();

        private bool _disableRemoveBtn = true;

        private async Task<TableData<ReplyDto>> ServerReload(TableState state, CancellationToken token)
        {
            _replyParameters.PageNumber = state.Page + 1;
            _replyParameters.PageSize = state.PageSize;
            _replyParameters.SearchByReply = searchString;

            if (state.SortLabel != null)
            {
                string sortDirection = state.SortDirection != SortDirection.Ascending ? "desc" : "";
                _replyParameters.OrderBy = $"{state.SortLabel} {sortDirection}".Trim();
            }

            using var replyService = ServiceManager!.ReplyService;
            var result = await replyService.GetRepliesAsync(_replyParameters, false);
            if (result.IsSuccess)
            {
                pagedData = result.GetValue<IEnumerable<ReplyDto>>();
                _metaData = result.Paging;
            }

            return new TableData<ReplyDto>() { TotalItems = _metaData!.TotalCount, Items = pagedData };
        }

        private async Task ReloadDataAsync()
        {
            if (table != null)
            {
                await table.ReloadServerData();
            }
        }

        private async Task OnSearch(string? text)
        {
            searchString = text;
            await table!.ReloadServerData();
        }

        private async void OnTimerElapsed(object? state)
        {
            await InvokeAsync(async () =>
            {
                await OnSearch(searchString);
            });

            _timer!.Dispose();
        }

        private void SearchChanged()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }

            _timer = new Timer(OnTimerElapsed, null, 500, 0);
        }

        private void RowClickEvent(TableRowClickEventArgs<ReplyDto> tableRowClickEventArgs)
        {
            _replyDto = tableRowClickEventArgs.Item;
            _disableRemoveBtn = _replyDto == null || _replyDto.Id == Guid.Empty;
            StateHasChanged();
        }

        private async void ItemHasBeenCommitted(object element)
        {
            var editedItem = (ReplyDto)element;

            if (!HasChanges(editedItem))
            {
                ShowVariant("No changes made. Update not performed.", Severity.Info);
                ResetItemToOriginalValues(editedItem);
                return;
            }

            var replyForUpdate = Mapper!.Map<ReplyForUpdateDto>(editedItem);
            if (replyForUpdate == null)
            {
                ShowVariant($"Edit reply with Id {editedItem.Id} failed.", Severity.Warning);
                return;
            }

            using var replyService = ServiceManager!.ReplyService;
            var result = await replyService.UpdateReplyAsync(editedItem.Id, replyForUpdate);
            if (result.IsSuccess)
            {
                ShowVariant($"Reply with Id {editedItem.Id} updated successfully.", Severity.Success);
                _disableRemoveBtn = true;
                StateHasChanged();
            }
            else
            {
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                ShowVariant($"Failed to update reply with Id {editedItem.Id}: {errorMessage}", Severity.Error);
                await ReloadDataAsync();
            }
        }

        private bool HasChanges(ReplyDto currentItem)
        {
            return _replyDtoBeforeEdit != null && !_replyDtoBeforeEdit.Equals(currentItem);
        }

        private void BackupItem(object element)
        {
            _replyDtoBeforeEdit = new ReplyDto
            {
                Id = ((ReplyDto)element).Id,
                Reply = ((ReplyDto)element).Reply,
                UpdatedAt = ((ReplyDto)element).UpdatedAt,
            };
        }

        private void ResetItemToOriginalValues(object element)
        {
            if (_replyDtoBeforeEdit != null)
            {
                Mapper!.Map(_replyDtoBeforeEdit, element);
            }

            _disableRemoveBtn = true;
            _replyDto = null;
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
            var dialogResult = await (await DialogService!.ShowAsync<ReplyModalCreate>("Create Reply", options)).Result;
            if (!dialogResult!.Canceled)
            {
                await ReloadDataAsync();
            }
        }

        private async Task OpenRemoveDialogAsync(Guid id)
        {
            var parameter = new DialogParameters();
            parameter.Add("Id", id);
            var dialog = await _dialogService.ShowAsync<ConfirmDeleteDialog>("Delete Confirmation", parameter);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await ReloadDataAsync();
            }
        }
    }
}
