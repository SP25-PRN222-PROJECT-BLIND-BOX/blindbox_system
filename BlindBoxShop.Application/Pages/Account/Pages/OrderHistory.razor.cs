using BlindBoxShop.Entities.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Account.Pages
{
    public partial class OrderHistory : ComponentBase
    {
        private User _user;
        private List<OrderViewModel> _orders;
        private bool _loading = true;
        private string _searchString = "";
        private string _selectedStatus = "All";

        [Inject]
        private UserManager<User> UserManager { get; set; }

        [Inject]
        private IdentityUserAccessor UserAccessor { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadUserData();
            await LoadOrderHistory();
        }
        
        private async Task LoadUserData()
        {
            try
            {
                // Trong triển khai thực tế, bạn sẽ lấy dữ liệu người dùng từ API/Service
                // Ví dụ: _user = await UserService.GetCurrentUserAsync();
                
                // Tạo dữ liệu mẫu cho demo
                _user = new User
                {
                    FirstName = "Nguyễn",
                    LastName = "Văn A",
                    Email = "nguyenvana@example.com",
                    CreatedAt = new DateTime(2022, 10, 15)
                };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading user data: {ex.Message}", Severity.Error);
            }
        }
        
        private async Task LoadOrderHistory()
        {
            try
            {
                _loading = true;
                
                // Giả lập việc tải dữ liệu từ API
                await Task.Delay(800);
                
                // Mock data cho demo
                _orders = GetMockOrders();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading orders: {ex.Message}", Severity.Error);
            }
            finally
            {
                _loading = false;
            }
        }
        
        private IEnumerable<OrderViewModel> GetFilteredOrders()
        {
            if (_orders == null)
                return new List<OrderViewModel>();
                
            return _orders
                .Where(o => (_selectedStatus == "All" || o.Status == _selectedStatus) &&
                        (string.IsNullOrEmpty(_searchString) || 
                         o.OrderId.Contains(_searchString) || 
                         o.PhoneNumber.Contains(_searchString)))
                .ToList();
        }
        
        private void ViewOrderDetails(string orderId)
        {
            // Chuyển đến trang chi tiết đơn hàng
            NavigationManager.NavigateTo($"/order-detail/{orderId}");
        }
        
        private string GetUserInitials()
        {
            if (_user == null) return "?";
            
            string initials = "";
            if (!string.IsNullOrEmpty(_user.FirstName) && _user.FirstName.Length > 0)
                initials += _user.FirstName[0];
            
            if (!string.IsNullOrEmpty(_user.LastName) && _user.LastName.Length > 0)
                initials += _user.LastName[0];
                
            return initials.ToUpper();
        }
        
        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "New order" => Color.Info,
                "Inproduction" => Color.Warning,
                "Shipped" => Color.Success,
                "Cancelled" => Color.Error,
                "Rejected" => Color.Error,
                "Draft" => Color.Default,
                _ => Color.Default
            };
        }
        
        private string GetChipBgColor(string status)
        {
            return status switch
            {
                "New order" => "#1976d2",   // Info blue
                "Inproduction" => "#ff9800", // Warning orange
                "Shipped" => "#4caf50",      // Success green
                "Cancelled" => "#f44336",    // Error red
                "Rejected" => "#f44336",     // Error red
                "Draft" => "#9e9e9e",        // Default gray
                _ => "#9e9e9e"               // Default gray
            };
        }
        
        private string FormatPrice(decimal price)
        {
            return $"{price.ToString("N0")} ₫";
        }
        
        // Dữ liệu mẫu
        private List<OrderViewModel> GetMockOrders()
        {
            return new List<OrderViewModel>
            {
                new OrderViewModel { OrderId = "59217", PhoneNumber = "59217342", Status = "New order", ItemCount = 1, OrderDate = new DateTime(2023, 12, 26), Total = 400000 },
                new OrderViewModel { OrderId = "59213", PhoneNumber = "59217342", Status = "Inproduction", ItemCount = 2, OrderDate = new DateTime(2023, 12, 26), Total = 400000 },
                new OrderViewModel { OrderId = "59219", PhoneNumber = "59217342", Status = "Shipped", ItemCount = 12, OrderDate = new DateTime(2023, 12, 26), Total = 400000 },
                new OrderViewModel { OrderId = "59220", PhoneNumber = "59217342", Status = "Cancelled", ItemCount = 22, OrderDate = new DateTime(2023, 12, 26), Total = 400000 },
                new OrderViewModel { OrderId = "59223", PhoneNumber = "59217342", Status = "Rejected", ItemCount = 32, OrderDate = new DateTime(2023, 12, 26), Total = 400000 },
                new OrderViewModel { OrderId = "592182", PhoneNumber = "59217342", Status = "Draft", ItemCount = 41, OrderDate = new DateTime(2023, 12, 26), Total = 400000 },
                new OrderViewModel { OrderId = "592183", PhoneNumber = "59217342", Status = "Draft", ItemCount = 41, OrderDate = new DateTime(2023, 12, 26), Total = 400000 },
                new OrderViewModel { OrderId = "592184", PhoneNumber = "59217342", Status = "Draft", ItemCount = 41, OrderDate = new DateTime(2023, 12, 26), Total = 400000 },
                new OrderViewModel { OrderId = "592185", PhoneNumber = "59217342", Status = "Draft", ItemCount = 41, OrderDate = new DateTime(2023, 12, 26), Total = 400000 },
                new OrderViewModel { OrderId = "592186", PhoneNumber = "59217342", Status = "Draft", ItemCount = 44, OrderDate = new DateTime(2023, 12, 26), Total = 400000 }
            };
        }
        
        public class OrderViewModel
        {
            public string OrderId { get; set; }
            public string PhoneNumber { get; set; }
            public string Status { get; set; }
            public int ItemCount { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal Total { get; set; }
        }
    }
} 