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

        private async Task PlaceOrder()
        {
            if (!IsFormValid())
            {
                Snackbar.Add("Vui lòng điền đầy đủ thông tin yêu cầu", Severity.Warning);
                return;
            }

            try
            {
                await SaveCheckoutInfoToLocalStorage();
                
                // Giả lập xử lý đơn hàng
                Snackbar.Add("Đang xử lý đơn hàng...", Severity.Info);
                
                // Tạo đối tượng đơn hàng
                var order = new
                {
                    CustomerName = $"{_checkoutInfo.FirstName} {_checkoutInfo.LastName}",
                    Address = $"{_checkoutInfo.Address}, {_checkoutInfo.Ward}, {_checkoutInfo.District}, {_checkoutInfo.Province}",
                    Phone = _checkoutInfo.Phone,
                    Email = _checkoutInfo.Email,
                    PaymentMethod = _checkoutInfo.PaymentMethod,
                    OrderDate = DateTime.Now,
                    Subtotal = Subtotal,
                    ShippingCost = ShippingCost,
                    Total = Subtotal + ShippingCost,
                    Items = _cartItems
                };
                
                // Trong ứng dụng thực tế, bạn sẽ gửi đơn hàng đến API
                // Giả lập đơn hàng thành công và xóa giỏ hàng
                
                // Tạo random OrderId (trong thực tế sẽ nhận từ server)
                var orderId = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
                
                // Xóa giỏ hàng (Không xóa ở đây vì sẽ được xử lý trong trang thành công)
                // await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "blindbox_cart");
                
                // Chuyển hướng đến trang xác nhận đơn hàng
                Snackbar.Add("Đặt hàng thành công!", Severity.Success);
                NavigationManager.NavigateTo($"/order-success/{orderId}");
                
                // Giả lập một số trường hợp lỗi (chỉ để demo)
                // Hiện tại luôn thành công, nhưng trong thực tế có thể thất bại
                // Nếu thất bại: NavigationManager.NavigateTo("/order-failed?error=payment");
            }
            catch (Exception)
            {
                Snackbar.Add("Đặt hàng thất bại. Vui lòng thử lại.", Severity.Error);
                NavigationManager.NavigateTo("/order-failed");
            }
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

        private async Task ProcessPayment()
        {
            try
            {
                if (!IsFormValid())
                {
                    Snackbar.Add("Vui lòng điền đầy đủ thông tin yêu cầu", Severity.Warning);
                    return;
                }

                _isProcessing = true;
                StateHasChanged();
                
                // Get current user ID or create guest ID if not logged in
                var userId = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_id");
                
                // Create Order when payment method is Cash on Delivery
                if (_checkoutInfo.PaymentMethod == "Cash on Delivery")
                {
                    // Create an OrderForCreationDto
                    var orderForCreation = new BlindBoxShop.Shared.DataTransferObject.Order.OrderForCreationDto
                    {
                        UserId = !string.IsNullOrEmpty(userId) ? Guid.Parse(userId) : Guid.Empty,
                        Status = BlindBoxShop.Shared.Enum.OrderStatus.Pending,
                        PaymentMethod = BlindBoxShop.Shared.Enum.PaymentMethod.Cash,
                        Address = _checkoutInfo.Address,
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
                        
                        try
                        {
                            // Tạo danh sách OrderDetailForCreationDto cho từng sản phẩm trong giỏ hàng
                            var orderDetailsForCreation = new List<OrderDetailForCreationDto>();
                            
                            foreach (var item in _cartItems)
                            {
                                if (item.BlindBoxId != Guid.Empty)
                                {
                                    // Tạo đối tượng OrderDetailForCreationDto
                                    var orderDetailDto = new OrderDetailForCreationDto
                                    {
                                        OrderId = orderId,
                                        BlindBoxId = item.BlindBoxId,
                                        Quantity = item.Quantity,
                                        Price = item.Price
                                    };
                                    
                                    orderDetailsForCreation.Add(orderDetailDto);
                                }
                            }
                            
                            // Gọi service để tạo chi tiết đơn hàng
                            if (orderDetailsForCreation.Any())
                            {
                                var orderDetailsResult = await ServiceManager.OrderDetailService.CreateBatchOrderDetailsAsync(orderDetailsForCreation);
                                
                                if (!orderDetailsResult.IsSuccess)
                                {
                                    // Ghi log lỗi nhưng vẫn tiếp tục - đơn hàng đã được tạo thành công
                                    Console.WriteLine($"Lỗi khi tạo chi tiết đơn hàng: {(orderDetailsResult.Errors != null && orderDetailsResult.Errors.Any() ? string.Join(", ", orderDetailsResult.Errors.Select(e => e.Description)) : "Unknown error")}");
                                }
                            }
                            
                            // Vẫn lưu thông tin vào localStorage để hiển thị trong trang thành công
                            List<OrderDetailData> orderDetails = _cartItems.Select(item => new OrderDetailData
                            {
                                OrderId = orderId,
                                BlindBoxId = item.BlindBoxId,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                BlindBoxName = item.ProductName
                            }).ToList();
                            
                            var orderDetailsJson = JsonSerializer.Serialize(orderDetails);
                            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "order_details_" + orderId, orderDetailsJson);
                            
                            // Clear the cart after successful order creation
                            await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "blindbox_cart");
                            
                            Snackbar.Add("Đặt hàng thành công!", Severity.Success);
                            
                            // Redirect to order success page with the order ID
                            NavigationManager.NavigateTo($"/order-success/{orderId}");
                        }
                        catch (Exception ex)
                        {
                            // Ghi log lỗi nhưng vẫn tiếp tục - đơn hàng đã được tạo thành công
                            Console.WriteLine($"Lỗi khi tạo chi tiết đơn hàng: {ex.Message}");
                            Snackbar.Add("Đơn hàng đã được tạo nhưng có lỗi khi lưu chi tiết đơn hàng.", Severity.Warning);
                            NavigationManager.NavigateTo($"/order-success/{orderId}");
                        }
                    }
                    else
                    {
                        // Handle order creation failure
                        var errorMessage = orderResult.Errors != null && orderResult.Errors.Any() 
                            ? string.Join(", ", orderResult.Errors.Select(e => e.Description))
                            : "Đã xảy ra lỗi khi tạo đơn hàng. Vui lòng thử lại.";
                            
                        Snackbar.Add(errorMessage, Severity.Error);
                        _isProcessing = false;
                        StateHasChanged();
                    }
                }
                // For other payment methods (VNPay)
                else if (_checkoutInfo.PaymentMethod == "VNPay")
                {
                    // Simulate payment processing
                    await Task.Delay(1500);
                    
                    // In a real application, you would redirect to the payment gateway
                    // For demonstration, we'll create a mock order ID
                    string orderId = Guid.NewGuid().ToString();
                    
                    // If this is an openable blindbox, redirect to open page
                    if (_isDirectPurchase && _isOpenableBlindBox)
                    {
                        NavigationManager.NavigateTo($"/open-blindbox/{BlindBoxId}?orderId={orderId}");
                    }
                    else
                    {
                        // Regular checkout - redirect to order success page
                        NavigationManager.NavigateTo($"/order-success/{orderId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Lỗi xử lý thanh toán: {ex.Message}", Severity.Error);
                _isProcessing = false;
                StateHasChanged();
            }
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
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string BlindBoxName { get; set; }
    }
} 