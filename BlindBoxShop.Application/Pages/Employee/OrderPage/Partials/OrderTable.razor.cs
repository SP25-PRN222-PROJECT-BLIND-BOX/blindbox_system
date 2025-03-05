using AutoMapper;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Service.Contract;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.Extension;

namespace BlindBoxShop.Application.Pages.Employee.OrderPage.Partials
{
    public partial class OrderTable
    {
        private IEnumerable<OrderDto>? pagedData;

        private MudTable<OrderDto>? table;

        private string? searchString;

        private OrderDto? _orderDto;

        private OrderDto? _orderDtoBeforeEdit { get; set; }

        private Timer? _timer;

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        private MetaData? _metaData { get; set; } = new MetaData();

        private OrderParameter _orderParameters = new OrderParameter();

        private bool _disableRemoveBtn = true;

        // Method to disable the cancel button if the order status is 'Cancelled'
        private bool IsCancelButtonDisabled(OrderDto order)
        {
            return order.Status == "Cancelled"; // Adjust the string based on your status value
        }

        private async Task<TableData<OrderDto>> ServerReload(TableState state, CancellationToken token)
        {
            _orderParameters.PageNumber = state.Page + 1;
            _orderParameters.PageSize = state.PageSize;
            _orderParameters.SearchById = searchString;
            if (state.SortLabel != null)
            {
                string sortDirection = state.SortDirection != SortDirection.Ascending ? "desc" : "";
                _orderParameters.OrderBy = string.Join(" ", state.SortLabel, sortDirection).Trim();
            }

            using var orderService = ServiceManager!.OrderService;
            var result = await orderService.GetOrdersByUserIdAsync(Guid.Parse("a5798b68-246a-4ef2-83f0-8c235c54b64a"), _orderParameters, false);
            if (result.IsSuccess)
            {
                pagedData = result.GetValue<IEnumerable<OrderDto>>();
                _metaData = result.Paging;
            }

            return new TableData<OrderDto>() { TotalItems = _metaData!.TotalCount, Items = pagedData };
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

        private void RowClickEvent(TableRowClickEventArgs<OrderDto> tableRowClickEventArgs)
        {
            _orderDto = tableRowClickEventArgs.Item;
            _disableRemoveBtn = _orderDto == null || _orderDto.Id == Guid.Empty;
            StateHasChanged();
        }
        private async Task OpenCreateDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialogResult = await (await DialogService!.ShowAsync<OrderModalCreate>("Create order", options)).Result;
            if (!dialogResult!.Canceled)
            {
                await ReloadDataAsync();
            }
        }

        private async Task OpenOrderDetailModalAsync(Guid orderId)
        {
            var parameters = new DialogParameters
            {
                { "OrderId", orderId }
            };

            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

            var dialog = await DialogService!.ShowAsync<OrderDetailModal>("Order Details", parameters, options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await ReloadDataAsync();
            }
        }

        private async Task OpenRemoveDialogAsync(Guid Id)
        {
            var parameter = new DialogParameters();
            parameter.Add("Id", Id);
            var dialog = await _dialogService.ShowAsync<ConfirmCancelDialog>("Delete Confirmation", parameter);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await ReloadDataAsync();
            }
        }

    }
}
