using AutoMapper;
using BlindBoxShop.Application.Pages.Account.Shared;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.CustomerReview;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.ReviewPage.Partials
{
    public partial class ReviewTable
    {
        private IEnumerable<ReviewDto>? pagedData;

        private MudTable<ReviewDto>? table;

        private string? searchString;

        private ReviewDto? _reviewDto;

        private ReviewDto? _reviewDtoBeforeEdit;

        private Timer? _timer;

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        private MetaData? _metaData { get; set; } = new MetaData();

        private CustomerReviewParameter _reviewParameters = new CustomerReviewParameter();

        private bool _disableRemoveBtn = true;

        private async Task<TableData<ReviewDto>> ServerReload(TableState state, CancellationToken token)
        {
            _reviewParameters.PageNumber = state.Page + 1;
            _reviewParameters.PageSize = state.PageSize;
            _reviewParameters.SearchByUsername = searchString;

            if (state.SortLabel != null)
            {
                string sortDirection = state.SortDirection != SortDirection.Ascending ? "desc" : "";
                _reviewParameters.OrderBy = $"{state.SortLabel} {sortDirection}".Trim();
            }

            using var reviewService = ServiceManager!.CustomerReviewsService;
            var result = await reviewService.GetReviewsAsync(_reviewParameters, false);
            if (result.IsSuccess)
            {
                pagedData = result.GetValue<IEnumerable<ReviewDto>>();
                _metaData = result.Paging;
            }

            return new TableData<ReviewDto>() { TotalItems = _metaData!.TotalCount, Items = pagedData };
        }

        public async Task ReloadDataAsync()
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

        private void RowClickEvent(TableRowClickEventArgs<ReviewDto> tableRowClickEventArgs)
        {
            _reviewDto = tableRowClickEventArgs.Item;
            _disableRemoveBtn = _reviewDto == null || _reviewDto.Id == Guid.Empty;
            StateHasChanged();
        }

        private async void ItemHasBeenCommitted(object element)
        {
            var editedItem = (ReviewDto)element;

            if (!HasChanges(editedItem))
            {
                ShowVariant("No changes made. Update not performed.", Severity.Info);
                ResetItemToOriginalValues(editedItem);
                return;
            }

            var reviewForUpdate = Mapper!.Map<ReviewForUpdateDto>(editedItem);
            if (reviewForUpdate == null)
            {
                ShowVariant($"Edit review with Id {editedItem.Id} failed.", Severity.Warning);
                return;
            }

            using var reviewService = ServiceManager!.CustomerReviewsService;
            var result = await reviewService.UpdateReviewAsync(editedItem.Id, reviewForUpdate);
            if (result.IsSuccess)
            {
                ShowVariant($"Review with Id {editedItem.Id} updated successfully.", Severity.Success);
                _disableRemoveBtn = true;
                StateHasChanged();
            }
            else
            {
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                ShowVariant($"Failed to update review with Id {editedItem.Id}: {errorMessage}", Severity.Error);
                await ReloadDataAsync();
            }
        }

        private bool HasChanges(ReviewDto currentItem)
        {
            return _reviewDtoBeforeEdit != null && !_reviewDtoBeforeEdit.Equals(currentItem);
        }

        private void BackupItem(object element)
        {
            _reviewDtoBeforeEdit = new ReviewDto
            {
                Id = ((ReviewDto)element).Id,
                FeedBack = ((ReviewDto)element).FeedBack,
                RatingStar = ((ReviewDto)element).RatingStar,
            };
        }

        private void ResetItemToOriginalValues(object element)
        {
            if (_reviewDtoBeforeEdit != null)
            {
                Mapper!.Map(_reviewDtoBeforeEdit, element);
            }

            _disableRemoveBtn = true;
            _reviewDto = null;
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
            var dialogResult = await (await DialogService!.ShowAsync<ReviewModalCreate>("Create review", options)).Result;
            if (!dialogResult!.Canceled)
            {
                await ReloadDataAsync();
            }
        }

        private async Task OpenRemoveDialogAsync(Guid id)
        {
            var parameter = new DialogParameters();
            parameter.Add("Id", id);
            var dialog = await _dialogService.ShowAsync<ConfirmDeleteDialog>("Delete Confiamtion", parameter);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await ReloadDataAsync();
            }
        }
    }
}