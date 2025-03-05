using BlindBoxShop.Entities.Models;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.DataTransferObject.OrderDetail;
using BlindBoxShop.Shared.Extension;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.OrderPage.Partials
{
    public partial class OrderDetailModal
    {
        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        protected OrderWithDetailsDto? OrderWithDetails { get; set; }

        [Parameter]
        public Guid OrderId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (ServiceManager == null)
            {
                Console.WriteLine("ServiceManager is not initialized.");
                return;
            }

            await LoadOrderDetailsAsync();
        }

        private async Task LoadOrderDetailsAsync()
        {
            try
            {
                if (ServiceManager == null)
                {
                    Console.WriteLine("ServiceManager is not initialized.");
                    return;
                }

                // Sử dụng dịch vụ để lấy OrderWithDetails
                using var orderService = ServiceManager.OrderService;
                var result = await orderService.GetOrderWithDetailsByIdAsync(OrderId, false);

                if (result == null)
                {
                    Console.WriteLine("Result is null.");
                    return;
                }

                if (result.IsSuccess)
                {
                    if (result.Value != null)
                    {
                        // Gán giá trị cho OrderWithDetails
                        OrderWithDetails = result.Value;
                    }
                    else
                    {
                        Console.WriteLine("OrderWithDetails is null in result.");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to load order details: {result.Errors}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void Cancel()
        {
            if (MudDialog == null)
            {
                Console.WriteLine("MudDialog is not initialized.");
                return;
            }

            MudDialog.Cancel();
        }
    }
}
