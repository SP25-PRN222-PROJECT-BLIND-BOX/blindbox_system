using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.OrderPage.Partials
{
    public partial class OrderModalCreate
    {
        private OrderForCreationDto _orderForCreate = new OrderForCreationDto
        {
            UserId = Guid.Parse("5eca5610-cce6-4c8a-90d5-dd667e4bf032"),
            Status = OrderStatus.Pending,
        };

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Inject]
        private IDialogService? DialogService { get; set; }

        private async Task ValidSubmit(EditContext context)
        {
            // Validate the necessary fields
            if (_orderForCreate.SubTotal <= 0 || _orderForCreate.Total <= 0)
            {
                Snackbar.Add("SubTotal and Total must be greater than 0.", Severity.Error);
                return;
            }
            using var orderService = ServiceManager!.OrderService;
            var result = await orderService.CreateOrderAsync(_orderForCreate);

            if (result.IsSuccess)
            {
                _orderForCreate = new OrderForCreationDto(); // Reset form

                MudDialog!.Close(DialogResult.Ok(result.GetValue<OrderDto>()));
                ShowSnackbar("Order created successfully.", Severity.Success);
            }
            else
            {
                // Display errors if any
                var errorsMessage = string.Join(", ", result.Errors!.Select(e => e.Description));
                ShowSnackbar(errorsMessage, Severity.Error);
            }
        }

        private async Task InvalidSubmit(EditContext context)
        {
            await DialogService!.ShowMessageBox(
                "Validation Error",
                "Please fill in all required fields correctly.",
                yesText: "OK",
                options: new DialogOptions { CloseOnEscapeKey = true });
        }

        private void Cancel()
        {
            MudDialog!.Cancel();
        }

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
                    Cancel();
                    break;
            }
        }

        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, options => options.SnackbarVariant = Variant.Text);
        }
    }
}
