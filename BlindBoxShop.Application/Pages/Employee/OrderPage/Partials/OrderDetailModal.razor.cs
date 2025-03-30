using BlindBoxShop.Entities.Models;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.DataTransferObject.OrderDetail;
using BlindBoxShop.Shared.Enum;
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
        
        [Parameter]
        public Dictionary<Guid, BlindBoxItemDto>? ExternalBlindBoxItems { get; set; }
        
        // Image preview properties
        private bool _imagePreviewVisible;
        private string _selectedImage = string.Empty;
        private bool _isZoomed;
        
        // BlindBoxItem tracking
        private Dictionary<Guid, BlindBoxItemDto> _blindBoxItems = new Dictionary<Guid, BlindBoxItemDto>();
        private Dictionary<Guid, bool> _itemsLoading = new Dictionary<Guid, bool>();

        protected override async Task OnInitializedAsync()
        {
            if (ServiceManager == null)
            {
                Console.WriteLine("ServiceManager is not initialized.");
                return;
            }

            await LoadOrderDetailsAsync();
            
            // Sử dụng BlindBoxItems từ bên ngoài nếu được cung cấp
            if (ExternalBlindBoxItems != null && ExternalBlindBoxItems.Count > 0 && OrderWithDetails != null)
            {
                _blindBoxItems = new Dictionary<Guid, BlindBoxItemDto>(ExternalBlindBoxItems);
                
                // Cập nhật ImageUrl cho các OrderDetail
                UpdateImageUrlsFromBlindBoxItems();
            }
            else if (OrderWithDetails != null && OrderWithDetails.OrderDetails.Any())
            {
                // Load BlindBoxItems for any order details that have them
                await LoadBlindBoxItemsForOrderDetails();
            }
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return Task.CompletedTask;
        }
        
        private async Task LoadBlindBoxItemsForOrderDetails()
        {
            if (OrderWithDetails?.OrderDetails == null) return;
            
            foreach (var detail in OrderWithDetails.OrderDetails)
            {
                _itemsLoading[detail.Id] = true;
                
                if (detail.BlindBoxItemId.HasValue)
                {
                    await LoadBlindBoxItem(detail.Id, detail.BlindBoxItemId.Value);
                }
                
                _itemsLoading[detail.Id] = false;
            }
            
            StateHasChanged();
        }
        
        private void UpdateImageUrlsFromBlindBoxItems()
        {
            if (OrderWithDetails?.OrderDetails == null) return;
            
            foreach (var detail in OrderWithDetails.OrderDetails)
            {
                if (detail.BlindBoxItemId.HasValue && 
                    _blindBoxItems.ContainsKey(detail.Id) && 
                    _blindBoxItems[detail.Id] != null &&
                    string.IsNullOrEmpty(detail.ImageUrl) &&
                    !string.IsNullOrEmpty(_blindBoxItems[detail.Id].ImageUrl))
                {
                    // Đảm bảo định dạng URL là chính xác
                    var imageUrl = _blindBoxItems[detail.Id].ImageUrl;
                    if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
                    {
                        imageUrl = "/" + imageUrl;
                    }
                    
                    // Cập nhật ImageUrl trong OrderDetail
                    detail.ImageUrl = imageUrl;
                }
            }
        }

        private async Task LoadBlindBoxItem(Guid orderDetailId, Guid blindBoxItemId)
        {
            try
            {
                if (ServiceManager == null) return;
                
                using var blindBoxItemService = ServiceManager.BlindBoxItemService;
                var result = await blindBoxItemService.GetBlindBoxItemByIdAsync(blindBoxItemId, false);
                
                if (result.IsSuccess && result.Value != null)
                {
                    _blindBoxItems[orderDetailId] = result.Value;
                    
                    // If the item has an image URL but our OrderDetail doesn't, update it
                    var orderDetail = OrderWithDetails?.OrderDetails.FirstOrDefault(od => od.Id == orderDetailId);
                    if (orderDetail != null && string.IsNullOrEmpty(orderDetail.ImageUrl) && !string.IsNullOrEmpty(result.Value.ImageUrl))
                    {
                        // Ensure URL format is correct
                        var imageUrl = result.Value.ImageUrl;
                        if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
                        {
                            imageUrl = "/" + imageUrl;
                        }
                        
                        // Update the image URL in the OrderDetail
                        orderDetail.ImageUrl = imageUrl;
                    }
                    
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading BlindBoxItem: {ex.Message}");
            }
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
        
        private string GetDisplayName(OrderDetailDto detail)
        {
            // If this detail has a BlindBoxItem, use its name instead
            if (detail.BlindBoxItemId.HasValue && _blindBoxItems.ContainsKey(detail.Id) && _blindBoxItems[detail.Id] != null)
            {
                return _blindBoxItems[detail.Id].Name;
            }
            
            // Otherwise use the BlindBox name
            return detail.BlindBoxName;
        }
        
        private string FormatPrice(decimal price)
        {
            return $"{price:N0} ₫";
        }
        
        private MudBlazor.Color GetOrderStatusColor(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Processing => Color.Info,
                OrderStatus.Delivered => Color.Success,
                OrderStatus.Cancelled => Color.Error,
                OrderStatus.Pending => Color.Warning,
                _ => Color.Default
            };
        }
        
        private MudBlazor.Color GetItemRarityColor(int rarity)
        {
            return rarity switch
            {
                0 => Color.Success,  // Common
                1 => Color.Info,     // Uncommon
                2 => Color.Warning,  // Rare
                3 => Color.Error,    // Epic
                4 => Color.Secondary, // Legendary
                _ => Color.Default
            };
        }
        
        private string GetRarityText(int rarity)
        {
            return rarity switch
            {
                0 => "Common",
                1 => "Uncommon",
                2 => "Rare",
                3 => "Epic",
                4 => "Legendary",
                _ => "Unknown"
            };
        }
        
        private void OpenImagePreview(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                _selectedImage = imageUrl;
                _imagePreviewVisible = true;
                _isZoomed = false;
                StateHasChanged();
            }
        }
        
        private void CloseImagePreview()
        {
            _imagePreviewVisible = false;
            _isZoomed = false;
            StateHasChanged();
        }
        
        private void ToggleZoom()
        {
            _isZoomed = !_isZoomed;
            StateHasChanged();
        }
    }
}
