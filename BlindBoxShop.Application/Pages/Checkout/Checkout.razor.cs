using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BlindBoxShop.Application.Models;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.DataTransferObject.OrderDetail;
using BlindBoxShop.Shared.DataTransferObject.Order;

namespace BlindBoxShop.Application.Pages.Checkout
{
    public partial class Checkout : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] private IServiceManager ServiceManager { get; set; }

        [Parameter]
        public string BlindBoxId { get; set; }

        private List<CartItem> _cartItems = new List<CartItem>();
        private CheckoutInfo _checkoutInfo = new CheckoutInfo();
        private bool _isLoading = true;
        private bool _isDirectPurchase = false;
        private bool _isOpenableBlindBox = false;
        private bool _isProcessing = false;
        private int _quantity = 1;
        private decimal Subtotal => _cartItems.Sum(item => item.Price * item.Quantity);
        private decimal ShippingCost => 20000; // Phí vận chuyển cố định là 20.000₫

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            
            // Check if this is a direct purchase
            _isDirectPurchase = !string.IsNullOrEmpty(BlindBoxId);
            
            if (_isDirectPurchase)
            {
                await LoadDirectPurchaseItem();
            }
            else
            {
                // Load cart items from localStorage
                await LoadCartItemsFromLocalStorage();
            }
            
            // Load checkout info (address, payment method, etc.)
            await LoadCheckoutInfoFromLocalStorage();
            
            // Đảm bảo phương thức thanh toán luôn có giá trị mặc định
            if (string.IsNullOrEmpty(_checkoutInfo.PaymentMethod))
            {
                _checkoutInfo.PaymentMethod = "Cash on Delivery";
            }
            
            // Get user info to auto-fill address fields
            await GetUserInfo();
            
            _isLoading = false;
        }

        private async Task LoadCartItemsFromLocalStorage()
        {
            try
            {
                var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                
                if (!string.IsNullOrEmpty(cartJson))
                {
                    _cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
                    
                    // Debug: in ra thông tin hình ảnh của các sản phẩm
                    foreach (var item in _cartItems)
                    {
                        Console.WriteLine($"Cart Item: {item.ProductName}, ImageUrl: {item.ImageUrl}");
                    }
                }
                
                // If no cart in localStorage or it's empty, load sample data for preview
                if (_cartItems.Count == 0 && NavigationManager.Uri.Contains("?sample=true"))
                {
                    LoadSampleData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading cart data: {ex.Message}");
                Snackbar.Add("Failed to load cart data", Severity.Error);
                LoadSampleData(); // Load sample data on error for preview
            }
        }

        private async Task LoadCheckoutInfoFromLocalStorage()
        {
            try
            {
                var infoJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_checkout_info");
                
                if (!string.IsNullOrEmpty(infoJson))
                {
                    _checkoutInfo = JsonSerializer.Deserialize<CheckoutInfo>(infoJson) ?? new CheckoutInfo();
                }
                else
                {
                    // Default payment method
                    _checkoutInfo.PaymentMethod = "Cash on Delivery";
                }
            }
            catch (Exception)
            {
                Snackbar.Add("Failed to load checkout info", Severity.Error);
            }
        }

        private void LoadSampleData()
        {
            _cartItems = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1,
                    ProductName = "LUCKY EMMA - Emma Secret Forest Masked Ball Blind Box Series",
                    ImageUrl = "/images/shop/emma-forest.jpg",
                    Price = 360000,
                    Quantity = 1
                },
                new CartItem
                {
                    Id = 2,
                    ProductName = "LUCKY EMMA - Alice Fairy Tale Blind Box Series",
                    ImageUrl = "/images/shop/emma-alice.jpg",
                    Price = 240000,
                    Quantity = 1
                }
            };
        }

        private async Task SaveCheckoutInfoToLocalStorage()
        {
            try
            {
                var infoJson = JsonSerializer.Serialize(_checkoutInfo);
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "blindbox_checkout_info", infoJson);
            }
            catch (Exception)
            {
                Snackbar.Add("Failed to save checkout info", Severity.Error);
            }
        }

        private bool IsFormValid()
        {
            return !string.IsNullOrWhiteSpace(_checkoutInfo.FirstName) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.LastName) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.Address) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.Province) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.District) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.Ward) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.Phone) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.Email) &&
                   !string.IsNullOrWhiteSpace(_checkoutInfo.PaymentMethod);
        }

        private string FormatPrice(decimal price)
        {
            return $"{price:N0}₫";
        }

        private async Task LoadDirectPurchaseItem()
        {
            try
            {
                // Parse the query string for quantity
                var uri = new Uri(NavigationManager.Uri);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                if (int.TryParse(query["quantity"], out int qty))
                {
                    _quantity = qty;
                }

                // Get blindbox info
                var blindBoxId = Guid.Parse(BlindBoxId);
                var result = await ServiceManager.BlindBoxService.GetBlindBoxByIdAsync(blindBoxId, false);
                
                if (result.IsSuccess && result.Value != null)
                {
                    var blindBox = result.Value;
                    
                    // Check if this is an openable blindbox
                    var packageResult = await ServiceManager.PackageService.GetPackageByIdAsync(blindBox.PackageId, false);
                    if (packageResult.IsSuccess && packageResult.Value?.Type == PackageType.Opened)
                    {
                        _isOpenableBlindBox = true;
                    }
                    
                    // Create cart item
                    _cartItems = new List<CartItem>
                    {
                        new CartItem
                        {
                            Id = blindBox.Id.GetHashCode(),
                            BlindBoxId = blindBox.Id,
                            ProductName = blindBox.Name,
                            ImageUrl = !string.IsNullOrEmpty(blindBox.MainImageUrl) ? blindBox.MainImageUrl : "/images/box-placeholder.jpg",
                            Price = blindBox.CurrentPrice,
                            Quantity = _quantity
                        }
                    };
                }
                else
                {
                    Snackbar.Add("Không thể tải thông tin sản phẩm.", Severity.Error);
                    NavigationManager.NavigateTo("/");
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi: {ex.Message}", Severity.Error);
                NavigationManager.NavigateTo("/");
            }
        }

        private string GetPaymentMethodClass(string method)
        {
            return _checkoutInfo.PaymentMethod == method 
                ? "bg-cyan-50 border-cyan-500" 
                : "bg-white hover:bg-gray-50 border-gray-200";
        }

        private void SelectPaymentMethod(string method)
        {
            // Update directly and trigger UI refresh
            _checkoutInfo.PaymentMethod = method;
            
            // Debug log when payment method is changed
            Console.WriteLine($"Payment method changed to: {method}");
            
            // Immediately save to localStorage when selected
            _ = SaveCheckoutInfoToLocalStorage();
            
            StateHasChanged();
        }

        private async Task SavePaymentMethod()
        {
            if (!string.IsNullOrEmpty(_checkoutInfo.PaymentMethod))
            {
                await SaveCheckoutInfoToLocalStorage();
                Snackbar.Add($"Phương thức thanh toán đã được cập nhật: {_checkoutInfo.PaymentMethod}", Severity.Success);
                
                // Debug: print current payment method
                Console.WriteLine($"Current payment method: {_checkoutInfo.PaymentMethod}");
                await JSRuntime.InvokeVoidAsync("console.log", "Current payment method: " + _checkoutInfo.PaymentMethod);
            }
        }
        
        private async Task ProcessPayment()
        {
            try
            {
                _isProcessing = true;
                StateHasChanged();
                
                // Check for valid form
                if (!IsFormValid())
                {
                    Snackbar.Add("Vui lòng điền đầy đủ thông tin giao hàng", Severity.Warning);
                    _isProcessing = false;
                    StateHasChanged();
                    return;
                }
                
                // Make another confirmation of payment method
                Console.WriteLine($"FINAL PAYMENT METHOD SELECTED: {_checkoutInfo.PaymentMethod}");
                await JSRuntime.InvokeVoidAsync("console.log", "PAYMENT METHOD SELECTED: " + _checkoutInfo.PaymentMethod);
                
                // Save checkout info to localStorage for future use
                await SaveCheckoutInfoToLocalStorage();
                
                // Use the helper method based on payment method
                if (_checkoutInfo.PaymentMethod == "Cash on Delivery")
                {
                    await ProcessCashOnDeliveryPayment();
                }
                else if (_checkoutInfo.PaymentMethod == "VNPay")
                {
                    await ProcessVNPayPayment();
                }
                else
                {
                    Console.WriteLine($"Invalid payment method: {_checkoutInfo.PaymentMethod}");
                    Snackbar.Add($"Phương thức thanh toán không hợp lệ: {_checkoutInfo.PaymentMethod}", Severity.Error);
                    _isProcessing = false;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi xử lý thanh toán: {ex.Message}", Severity.Error);
                _isProcessing = false;
                StateHasChanged();
            }
        }
        
        private async Task ProcessCashOnDeliveryPayment()
        {
            // Create order with the CreateOrder helper method
            var order = await CreateOrder("Processing");
            
            if (order != null)
            {
                // Save order details to localStorage for order success page
                List<OrderDetailData> orderDetails = _cartItems.Select(item => new OrderDetailData
                {
                    OrderId = order.Id,
                    BlindBoxId = item.BlindBoxId,
                    BlindBoxItemId = null, // For physical products, BlindBoxItemId is null
                    Quantity = item.Quantity,
                    Price = item.Price,
                    BlindBoxName = item.ProductName
                }).ToList();
                
                var orderDetailsJson = JsonSerializer.Serialize(orderDetails);
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "order_details_" + order.Id, orderDetailsJson);
                
                // Clear the cart
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "blindbox_cart");
                
                Snackbar.Add("Đặt hàng thành công!", Severity.Success);
                
                // Redirect to order success page
                NavigationManager.NavigateTo($"/order-success/{order.Id}");
            }
            else
            {
                _isProcessing = false;
                StateHasChanged();
            }
        }
        
        private async Task ProcessVNPayPayment()
        {
            // Create order with the CreateOrder helper method
            var order = await CreateOrder("Pending");
            
            if (order != null)
            {
                try
                {
                    // Get current user ID
                    var userId = await GetUserIdAsync();
                    
                    // Get VNPay payment URL
                    var baseUrl = NavigationManager.BaseUri.TrimEnd('/');
                    var paymentResult = await ServiceManager.VNPayService.GetPaymentUrlAsync(order.Id, userId, baseUrl);
                    
                    if (paymentResult.IsSuccess && !string.IsNullOrEmpty(paymentResult.Value))
                    {
                        // Log URL để debug
                        Console.WriteLine($"Redirecting to VNPay URL: {paymentResult.Value}");
                        
                        // Clear cart before redirect
                        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "blindbox_cart");
                        
                        // Lưu thông tin đơn hàng vào localStorage để xử lý khi trở về
                        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "pending_order_id", order.Id.ToString());
                        
                        // Hiển thị thông báo cho người dùng
                        Snackbar.Add("Đang chuyển đến cổng thanh toán VNPay...", Severity.Info);
                        
                        // Chuyển hướng đến trang thanh toán VNPay trong cùng cửa sổ thay vì mở cửa sổ mới
                        NavigationManager.NavigateTo(paymentResult.Value);
                        
                        // Không cần thiết lập _isProcessing = false vì chúng ta đang chuyển hướng
                    }
                    else
                    {
                        var errorMessage = paymentResult.Errors != null && paymentResult.Errors.Any() 
                            ? string.Join(", ", paymentResult.Errors.Select(e => e.Description))
                            : "Đã xảy ra lỗi khi tạo URL thanh toán. Vui lòng thử lại.";
                            
                        // Hiển thị thông báo lỗi cho người dùng
                        Snackbar.Add(errorMessage, Severity.Error);
                        
                        // Không chuyển hướng đến trang thất bại, chỉ hiện thông báo lỗi
                        _isProcessing = false;
                        StateHasChanged();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi xử lý thanh toán VNPay: {ex.Message}");
                    
                    // Hiển thị thông báo lỗi cho người dùng
                    Snackbar.Add($"Lỗi khi xử lý thanh toán: {ex.Message}", Severity.Error);
                    
                    // Không chuyển hướng đến trang thất bại, chỉ hiện thông báo lỗi
                    _isProcessing = false;
                    StateHasChanged();
                }
            }
            else
            {
                // Hiển thị thông báo lỗi cho người dùng
                Snackbar.Add("Không thể tạo đơn hàng. Vui lòng thử lại.", Severity.Error);
                
                // Không chuyển hướng đến trang thất bại, chỉ hiện thông báo lỗi
                _isProcessing = false;
                StateHasChanged();
            }
        }

        // Helper method to get the current user ID
        private async Task<Guid> GetUserIdAsync()
        {
            try
            {
                var userIdStr = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_id");
                if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var userId))
                {
                    return userId;
                }
            }
            catch (Exception)
            {
                // Fallback to anonymous user
            }
            
            // Return a default guest ID or anonymous user ID
            return Guid.Parse("00000000-0000-0000-0000-000000000001"); // Guest user ID
        }

        private async Task GetUserInfo()
        {
            try
            {
                // Get the current user ID from localStorage
                var userId = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_id");
                
                if (string.IsNullOrEmpty(userId))
                {
                    // User is not logged in, don't try to autofill
                    return;
                }

                // Use the user service to get user info
                var userObj = await ServiceManager.UserService.GetUserByIdAsync(Guid.Parse(userId), false);
                var user = userObj as BlindBoxShop.Entities.Models.User;
                
                if (user == null)
                {
                    return;
                }

                // Only fill in fields that are empty to avoid overwriting user-entered data
                if (string.IsNullOrEmpty(_checkoutInfo.FirstName))
                    _checkoutInfo.FirstName = user.FirstName ?? string.Empty;
                    
                if (string.IsNullOrEmpty(_checkoutInfo.LastName))
                    _checkoutInfo.LastName = user.LastName ?? string.Empty;
                    
                if (string.IsNullOrEmpty(_checkoutInfo.Address))
                    _checkoutInfo.Address = user.Address ?? string.Empty;
                    
                if (string.IsNullOrEmpty(_checkoutInfo.Province))
                    _checkoutInfo.Province = user.Provinces ?? string.Empty;
                    
                if (string.IsNullOrEmpty(_checkoutInfo.District))
                    _checkoutInfo.District = user.District ?? string.Empty;
                    
                if (string.IsNullOrEmpty(_checkoutInfo.Ward))
                    _checkoutInfo.Ward = user.Wards ?? string.Empty;
                    
                if (string.IsNullOrEmpty(_checkoutInfo.Phone))
                    _checkoutInfo.Phone = user.PhoneNumber ?? string.Empty;
                    
                if (string.IsNullOrEmpty(_checkoutInfo.Email))
                    _checkoutInfo.Email = user.Email ?? string.Empty;
                    
                // Save the filled checkout info to localStorage
                await SaveCheckoutInfoToLocalStorage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user info: {ex.Message}");
                // Don't show error to user as this is just a convenience feature
            }
        }

        // Helper method to create an order with the specified status
        private async Task<OrderDto> CreateOrder(string status)
        {
            try
            {
                // Create order object with proper status
                var orderStatus = status == "Pending" 
                    ? BlindBoxShop.Shared.Enum.OrderStatus.AwaitingPayment 
                    : BlindBoxShop.Shared.Enum.OrderStatus.Processing;
                    
                var paymentMethod = _checkoutInfo.PaymentMethod == "VNPay" 
                    ? BlindBoxShop.Shared.Enum.PaymentMethod.VnPay 
                    : BlindBoxShop.Shared.Enum.PaymentMethod.Cash;
                    
                var orderForCreation = new OrderForCreationDto
                {
                    UserId = await GetUserIdAsync(),
                    Status = orderStatus,
                    PaymentMethod = paymentMethod,
                    Address = $"{_checkoutInfo.FirstName} {_checkoutInfo.LastName}, {_checkoutInfo.Phone}, {_checkoutInfo.Address}",
                    Province = _checkoutInfo.Province,
                    Wards = _checkoutInfo.Ward,
                    SubTotal = Subtotal,
                    Total = Subtotal + ShippingCost
                };
                
                // Call OrderService to create the order
                var orderResult = await ServiceManager.OrderService.CreateOrderAsync(orderForCreation);
                
                if (orderResult.IsSuccess && orderResult.Value != null)
                {
                    var orderId = orderResult.Value.Id;
                    
                    // Create order details for each cart item
                    var orderDetailsForCreation = new List<OrderDetailForCreationDto>();
                    
                    foreach (var item in _cartItems)
                    {
                        if (item.BlindBoxId != Guid.Empty)
                        {
                            var orderDetailDto = new OrderDetailForCreationDto
                            {
                                OrderId = orderId,
                                BlindBoxId = item.BlindBoxId,
                                BlindBoxItemId = null, // For regular products, BlindBoxItemId is null
                                Quantity = item.Quantity,
                                Price = item.Price
                            };
                            
                            orderDetailsForCreation.Add(orderDetailDto);
                        }
                    }
                    
                    // Create order details
                    if (orderDetailsForCreation.Any())
                    {
                        var orderDetailsResult = await ServiceManager.OrderDetailService.CreateBatchOrderDetailsAsync(orderDetailsForCreation);
                        
                        if (!orderDetailsResult.IsSuccess)
                        {
                            Console.WriteLine($"Error creating order details: {(orderDetailsResult.Errors != null && orderDetailsResult.Errors.Any() ? string.Join(", ", orderDetailsResult.Errors.Select(e => e.Description)) : "Unknown error")}");
                            Snackbar.Add("Order created but there was an error saving order details.", Severity.Warning);
                        }
                    }
                    
                    return orderResult.Value;
                }
                else
                {
                    // Handle order creation failure
                    var errorMessage = orderResult.Errors != null && orderResult.Errors.Any() 
                        ? string.Join(", ", orderResult.Errors.Select(e => e.Description))
                        : "An error occurred while creating your order. Please try again.";
                        
                    Snackbar.Add(errorMessage, Severity.Error);
                    
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                Snackbar.Add($"Error creating order: {ex.Message}", Severity.Error);
                
                return null;
            }
        }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Guid BlindBoxId { get; set; }
        public Guid? BlindBoxItemId { get; set; }
    }

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
    
    // Lớp tạm thời để lưu thông tin chi tiết đơn hàng
    public class OrderDetailData
    {
        public Guid OrderId { get; set; }
        public Guid BlindBoxId { get; set; }
        public Guid? BlindBoxItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string BlindBoxName { get; set; }
    }
} 