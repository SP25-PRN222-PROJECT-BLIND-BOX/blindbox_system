using BlindBoxShop.Entities.Models;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.DataTransferObject.OrderDetail;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.ResultModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages.Account.Pages
{
    public partial class OrderDetail : ComponentBase
    {
        [Parameter]
        public string OrderId { get; set; }

        private User _user;
        private OrderDto Order { get; set; }
        private List<OrderDetailDto> OrderItems { get; set; } = new List<OrderDetailDto>();
        private List<StatusStep> OrderStatusSteps { get; set; } = new List<StatusStep>();
        private bool IsLoading { get; set; } = true;
        private bool IsDialogVisible { get; set; } = false;
        private decimal ShippingCost { get; set; } = 20000;
        private decimal Discount { get; set; } = 0;
        private string Phone { get; set; }
        private string Email { get; set; }
        
        // Dictionary to store and quickly access image URLs
        private Dictionary<Guid, string> _blindBoxImages = new Dictionary<Guid, string>();

        private bool _imagePreviewVisible = false;
        private string _selectedImageUrl = string.Empty;
        private bool _isZoomed = false;

        [Inject]
        private UserManager<User> UserManager { get; set; }

        [Inject]
        private IdentityUserAccessor UserAccessor { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IServiceManager ServiceManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadOrderData();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (string.IsNullOrEmpty(OrderId))
            {
                NavigationManager.NavigateTo("/account/order-history");
                return;
            }

            await LoadOrderData();
        }

        private async Task LoadUserData()
        {
            try
            {
                // Mock user data for demo
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading user data: {ex.Message}", Severity.Error);
            }
        }

        private async Task LoadOrderData()
        {
            IsLoading = true;
            
            try
            {
                if (Guid.TryParse(OrderId, out Guid orderId))
                {
                    var result = await ServiceManager.OrderService.GetOrderWithDetailsByIdAsync(orderId, false);
                    
                    if (result.IsSuccess && result.Value != null)
                    {
                        Order = result.Value.Order;
                        OrderItems = result.Value.OrderDetails.ToList();
                        
                        // Set up user contact information - would come from user data in a real app
                        Phone = "0123456789";
                        Email = "customer@example.com";
                        
                        // Process images for OrderDetails
                        await ProcessImagesAsync();
                        
                        // Debug: in ra thông tin ImageUrl của các OrderDetailDto
                        foreach (var item in OrderItems)
                        {
                            Console.WriteLine($"OrderDetail: {item.BlindBoxName}, ImageUrl: '{item.ImageUrl}'");
                        }
                        
                        SetupOrderStatusSteps();
                    }
                    else
                    {
                        // Show error notification
                        Snackbar.Add("Order not found or has been removed", Severity.Warning);
                        Order = null;
                    }
                }
                else
                {
                    // Invalid order ID
                    NavigationManager.NavigateTo("/account/order-history");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order: {ex.Message}");
                Snackbar.Add($"Error loading order: {ex.Message}", Severity.Error);
                Order = null;
            }
            
            IsLoading = false;
        }

        private string EnsureCorrectImageUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return "/images/box-placeholder.jpg";

            // Kiểm tra các trường hợp URL chuẩn
            if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                return imageUrl;

            // Thêm dấu '/' ở đầu nếu cần
            if (!imageUrl.StartsWith("/"))
                imageUrl = "/" + imageUrl;

            return imageUrl;
        }

        private async Task ProcessImagesAsync()
        {
            try
            {
                // Đảm bảo đường dẫn hình ảnh được xử lý đúng cho tất cả các OrderItems
                foreach (var orderItem in OrderItems)
                {
                    // Process image URL directly from OrderDetailDto
                    orderItem.ImageUrl = EnsureCorrectImageUrl(orderItem.ImageUrl);
                    Console.WriteLine($"OrderDetail: {orderItem.BlindBoxName}, ImageUrl: '{orderItem.ImageUrl}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing images: {ex.Message}");
            }
        }

        private void SetupOrderStatusSteps()
        {
            if (Order == null) return;

            OrderStatusSteps = new List<StatusStep>
            {
                new StatusStep 
                { 
                    Name = "Order Placed", 
                    Description = "Your order has been received",
                    Icon = Icons.Material.Filled.Receipt,
                    IsCompleted = true,
                    IsActive = Order.Status == OrderStatus.Pending,
                    Date = Order.CreatedAt
                },
                new StatusStep 
                { 
                    Name = "Processing", 
                    Description = "We're preparing your order",
                    Icon = Icons.Material.Filled.Inventory2,
                    IsCompleted = Order.Status != OrderStatus.Pending && Order.Status != OrderStatus.Cancelled,
                    IsActive = false,
                    Date = Order.Status != OrderStatus.Pending ? DateTime.Now.AddHours(-2) : null
                },
                new StatusStep 
                { 
                    Name = "Shipping", 
                    Description = "Your order is on the way",
                    Icon = Icons.Material.Filled.LocalShipping,
                    IsCompleted = false,
                    IsActive = false,
                    Date = null
                },
                new StatusStep 
                { 
                    Name = "Delivered", 
                    Description = "Package delivered successfully",
                    Icon = Icons.Material.Filled.CheckCircle,
                    IsCompleted = false,
                    IsActive = false,
                    Date = null
                }
            };

            if (Order.Status == OrderStatus.Cancelled)
            {
                // Replace the steps with a cancelled status
                OrderStatusSteps = new List<StatusStep>
                {
                    new StatusStep 
                    { 
                        Name = "Order Placed", 
                        Description = "Your order was received",
                        Icon = Icons.Material.Filled.Receipt,
                        IsCompleted = true,
                        IsActive = false,
                        Date = Order.CreatedAt
                    },
                    new StatusStep 
                    { 
                        Name = "Order Cancelled", 
                        Description = "This order has been cancelled",
                        Icon = Icons.Material.Filled.Cancel,
                        IsCompleted = false,
                        IsActive = true,
                        Date = DateTime.Now
                    }
                };
            }
        }

        private string FormatPrice(decimal price)
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0}", price) + " ₫";
        }

        private string GetPaymentMethodName()
        {
            return Order?.PaymentMethod.ToString() ?? "Cash on Delivery";
        }

        private string GetPaymentIcon()
        {
            return Order?.PaymentMethod switch
            {
                PaymentMethod.Cash => Icons.Material.Filled.Money,
                PaymentMethod.VnPay => Icons.Material.Filled.Payments,
                _ => Icons.Material.Filled.Money
            };
        }

        private void CancelOrder()
        {
            IsDialogVisible = true;
        }

        private void CloseDialog()
        {
            IsDialogVisible = false;
        }

        private async Task ConfirmCancelOrder()
        {
            try
            {
                IsDialogVisible = false;
                IsLoading = true;
                
                if (Order == null || !Guid.TryParse(OrderId, out Guid orderId)) return;
                
                var result = await ServiceManager.OrderService.CancelOrderAsync(orderId);
                
                if (result.IsSuccess)
                {
                    Snackbar.Add("Order has been cancelled successfully", Severity.Success);
                    await LoadOrderData();
                }
                else
                {
                    Snackbar.Add("Failed to cancel order", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling order: {ex.Message}");
                Snackbar.Add($"Error cancelling order: {ex.Message}", Severity.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OpenImagePreview(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                _selectedImageUrl = imageUrl;
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

    public class StatusStep
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Date { get; set; }
    }
} 