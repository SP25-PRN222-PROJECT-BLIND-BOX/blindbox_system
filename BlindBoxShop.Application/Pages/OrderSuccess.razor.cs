using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using BlindBoxShop.Application.Components.Layout;
using BlindBoxShop.Application.Models;
using BlindBoxShop.Service.Contract;
using System.Text.Json;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.DataTransferObject.OrderDetail;

namespace BlindBoxShop.Application.Pages
{
    public partial class OrderSuccess : ComponentBase
    {
        [Parameter]
        public string OrderId { get; set; }
        
        private string PaymentMethod { get; set; } = "Cash on Delivery";
        private List<OrderDetailDto> OrderItems { get; set; } = new List<OrderDetailDto>();
        private OrderDto Order { get; set; }
        private decimal Subtotal { get; set; }
        private decimal ShippingFee { get; set; } = 20000; // Shipping fee cố định là 20.000₫
        private decimal Total => Subtotal + ShippingFee;
        private bool IsLoading { get; set; } = true;
        
        // Dictionary để lưu trữ và truy cập nhanh đường dẫn hình ảnh
        private Dictionary<Guid, string> _blindBoxImages = new Dictionary<Guid, string>();
        
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        
        [Inject]
        private ISnackbar Snackbar { get; set; }

        [Inject]
        private IServiceManager ServiceManager { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            // Redirect if accessed directly without order ID
            if (string.IsNullOrEmpty(OrderId) || OrderId == "null")
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            
            IsLoading = true;
            
            try
            {
                // Try to get order from the database
                if (Guid.TryParse(OrderId, out Guid orderId))
                {
                    var orderResult = await ServiceManager.OrderService.GetOrderWithDetailsByIdAsync(orderId, false);
                    
                    if (orderResult.IsSuccess && orderResult.Value != null)
                    {
                        Order = orderResult.Value.Order;
                        OrderItems = orderResult.Value.OrderDetails.ToList();
                        
                        // Set payment method
                        PaymentMethod = Order.PaymentMethod.ToString();
                        
                        // Calculate subtotal
                        Subtotal = Order.SubTotal;
                        ShippingFee = 20000; // You may want to get this from the order data if available
                        
                        // Process and fix image URLs
                        await ProcessImagesAsync();
                        
                        // Debug: in ra thông tin ImageUrl của tất cả các OrderItems
                        foreach (var item in OrderItems)
                        {
                            Console.WriteLine($"OrderDetail in Success: {item.BlindBoxName}, ImageUrl: '{item.ImageUrl}'");
                        }
                        
                        // Clear cart after successful order
                        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "blindbox_cart");
                    }
                    else
                    {
                        // If we couldn't get the order from the database, try to get from localStorage
                        await LoadOrderFromLocalStorage();
                    }
                }
                else
                {
                    // If OrderId is not a valid Guid, load from localStorage (for demo purposes)
                    await LoadOrderFromLocalStorage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order: {ex.Message}");
                // Fallback to loading from localStorage
                await LoadOrderFromLocalStorage();
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
                    if (string.IsNullOrEmpty(orderItem.ImageUrl) || orderItem.ImageUrl == "/images/box-placeholder.jpg")
                    {
                        Console.WriteLine($"Image URL is empty or placeholder for {orderItem.BlindBoxName}, trying to fetch actual image");
                        
                        if (orderItem.BlindBoxId != Guid.Empty)
                        {
                            // Thử lấy hình ảnh từ API nếu có BlindBoxId
                            try
                            {
                                var blindBoxResult = await ServiceManager.BlindBoxService.GetBlindBoxByIdAsync(orderItem.BlindBoxId, false);
                                if (blindBoxResult.IsSuccess && blindBoxResult.Value != null)
                                {
                                    var blindBox = blindBoxResult.Value;
                                    if (!string.IsNullOrEmpty(blindBox.MainImageUrl))
                                    {
                                        orderItem.ImageUrl = blindBox.MainImageUrl;
                                        Console.WriteLine($"Retrieved MainImageUrl from BlindBox API: {orderItem.ImageUrl}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error fetching BlindBox image: {ex.Message}");
                            }
                        }
                    }
                    
                    // Xử lý URL hình ảnh trực tiếp từ OrderDetailDto
                    orderItem.ImageUrl = EnsureCorrectImageUrl(orderItem.ImageUrl);
                    _blindBoxImages[orderItem.Id] = orderItem.ImageUrl;
                    
                    Console.WriteLine($"OrderDetail in Success: {orderItem.BlindBoxName}, ImageUrl: '{orderItem.ImageUrl}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing images: {ex.Message}");
            }
        }
        
        private async Task LoadOrderFromLocalStorage()
        {
            try
            {
                // Check for order details saved in localStorage during checkout
                var orderDetailsJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "order_details_" + OrderId);
                if (!string.IsNullOrEmpty(orderDetailsJson))
                {
                    var orderDetailData = JsonSerializer.Deserialize<List<OrderDetailData>>(orderDetailsJson);
                    if (orderDetailData != null && orderDetailData.Any())
                    {
                        // Convert order detail data to OrderDetailDto
                        OrderItems = orderDetailData.Select(item => new OrderDetailDto
                        {
                            Id = Guid.NewGuid(),
                            Quantity = item.Quantity,
                            Price = item.Price,
                            BlindBoxName = item.BlindBoxName,
                            BlindBoxId = item.BlindBoxId
                        }).ToList();
                        
                        // Calculate subtotal
                        Subtotal = OrderItems.Sum(item => item.Price * item.Quantity);
                    }
                }
                else
                {
                    // Fallback to cart items if order details not found
                    var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                    if (!string.IsNullOrEmpty(cartJson))
                    {
                        var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                        if (cartItems != null)
                        {
                            // Convert cart items to order items
                            OrderItems = cartItems.Select(item => new OrderDetailDto
                            {
                                Id = Guid.NewGuid(),
                                BlindBoxName = item.ProductName,
                                Price = item.Price,
                                Quantity = item.Quantity,
                                ImageUrl = item.ImageUrl,
                                BlindBoxId = item.BlindBoxId
                            }).ToList();
                            
                            // Process image URLs
                            await ProcessImagesAsync();
                        
                        // Calculate subtotal
                        Subtotal = OrderItems.Sum(item => item.Price * item.Quantity);
                    }
                }
                
                    // Clear cart after showing order success
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "blindbox_cart");
                }
                
                // Get payment method from localStorage if available
                var checkoutInfoJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_checkout_info");
                if (!string.IsNullOrEmpty(checkoutInfoJson))
                {
                    var checkoutInfo = JsonSerializer.Deserialize<CheckoutInfo>(checkoutInfoJson);
                    if (checkoutInfo != null && !string.IsNullOrEmpty(checkoutInfo.PaymentMethod))
                    {
                        PaymentMethod = checkoutInfo.PaymentMethod;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order from localStorage: {ex.Message}");
                // Fallback to empty order for display
                OrderItems = new List<OrderDetailDto>();
            }
        }
        
        // Format price with Vietnamese currency
        private string FormatPrice(decimal price)
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0}", price) + " ₫";
        }

        // Navigate to order details page
        private void ViewOrderDetails()
        {
            if (Guid.TryParse(OrderId, out Guid orderId))
            {
                NavigationManager.NavigateTo($"/my-account?tab=1");
            }
        }
    }
    
    // Local class for deserializing checkout info from localStorage
    public class CheckoutInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; }
    }
    
    // Class for deserializing order details from localStorage
    public class OrderDetailData
    {
        public Guid OrderId { get; set; }
        public Guid BlindBoxId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string BlindBoxName { get; set; }
    }
} 