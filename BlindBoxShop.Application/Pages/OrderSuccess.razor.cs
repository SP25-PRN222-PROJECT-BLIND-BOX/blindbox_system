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
            // Xóa dữ liệu session storage liên quan đến BlindBox Gacha
            await CleanupGachaSessionStorage();
            
            // Check if OrderId is missing or null, try to get from sessionStorage
            if (string.IsNullOrEmpty(OrderId) || OrderId == "null")
            {
                // Try to get order ID from sessionStorage (set by VNPayCallback)
                var successOrderId = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "success_order_id");
                Console.WriteLine($"Found success_order_id in sessionStorage: {successOrderId}");
                
                if (!string.IsNullOrEmpty(successOrderId))
                {
                    // Set OrderId parameter using value from sessionStorage
                    OrderId = successOrderId;
                    Console.WriteLine($"Using OrderId from sessionStorage: {OrderId}");
                    
                    // Clear sessionStorage after use
                    await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", "success_order_id");
                }
                else
                {
                    // Show default success page without order details
                    IsLoading = false;
                    return;
                }
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
                    
                    // Process image URL directly from OrderDetailDto
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
                Console.WriteLine($"Attempting to load order details from localStorage for order ID: {OrderId}");
                var orderDetailsJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "order_details_" + OrderId);
                
                if (!string.IsNullOrEmpty(orderDetailsJson))
                {
                    Console.WriteLine($"Found order details in localStorage, length: {orderDetailsJson.Length}");
                    try
                    {
                        var orderDetailData = JsonSerializer.Deserialize<List<OrderDetailData>>(orderDetailsJson);
                        if (orderDetailData != null && orderDetailData.Any())
                        {
                            Console.WriteLine($"Deserialized {orderDetailData.Count} order items");
                            
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
                            Console.WriteLine($"Calculated subtotal: {Subtotal}");
                        }
                        else
                        {
                            Console.WriteLine("Deserialized order detail data was null or empty");
                        }
                    }
                    catch (JsonException jex)
                    {
                        Console.WriteLine($"JSON deserialization error: {jex.Message}");
                        Console.WriteLine($"JSON content: {orderDetailsJson.Substring(0, Math.Min(100, orderDetailsJson.Length))}...");
                        
                        // Try alternative deserialization approach
                        try
                        {
                            var orderDetailItems = JsonSerializer.Deserialize<List<OrderDetailDto>>(orderDetailsJson);
                            if (orderDetailItems != null && orderDetailItems.Any())
                            {
                                Console.WriteLine($"Successfully deserialized as OrderDetailDto directly, {orderDetailItems.Count} items");
                                OrderItems = orderDetailItems;
                                Subtotal = OrderItems.Sum(item => item.Price * item.Quantity);
                            }
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine($"Alternative deserialization also failed: {ex2.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No order details found in localStorage, trying to load from cart");
                    // Fallback to cart items if order details not found
                    var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                    if (!string.IsNullOrEmpty(cartJson))
                    {
                        Console.WriteLine($"Found cart data, length: {cartJson.Length}");
                        try
                        {
                            var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                            if (cartItems != null && cartItems.Any())
                            {
                                Console.WriteLine($"Loaded {cartItems.Count} items from cart");
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
                                Console.WriteLine($"Calculated subtotal from cart: {Subtotal}");
                            }
                            else
                            {
                                Console.WriteLine("Cart items were null or empty");
                            }
                        }
                        catch (Exception cartEx)
                        {
                            Console.WriteLine($"Error deserializing cart data: {cartEx.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No cart data found in localStorage");
                    }
                    
                    // Clear cart after showing order success
                    await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "blindbox_cart");
                }
                
                // Get payment method from localStorage if available
                var checkoutInfoJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_checkout_info");
                if (!string.IsNullOrEmpty(checkoutInfoJson))
                {
                    Console.WriteLine("Found checkout info in localStorage");
                    var checkoutInfo = JsonSerializer.Deserialize<CheckoutInfo>(checkoutInfoJson);
                    if (checkoutInfo != null && !string.IsNullOrEmpty(checkoutInfo.PaymentMethod))
                    {
                        PaymentMethod = checkoutInfo.PaymentMethod;
                        Console.WriteLine($"Set payment method to: {PaymentMethod}");
                    }
                }
                else
                {
                    Console.WriteLine("No checkout info found in localStorage");
                }
                
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order from localStorage: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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

        // Thêm phương thức này
        private async Task CleanupGachaSessionStorage()
        {
            try
            {
                // Xóa tất cả localStorage và sessionStorage liên quan đến thanh toán và blindbox gacha
                await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", "temp_order_id");
                await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", "blindbox_id");
                await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", "payment_success");
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "payment_referring_url");
                
                // In thông tin để debug
                Console.WriteLine("OrderSuccess page: Cleaned up all session storage related to gacha");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning session storage: {ex.Message}");
                // Chỉ ghi log, không dừng quá trình xử lý
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