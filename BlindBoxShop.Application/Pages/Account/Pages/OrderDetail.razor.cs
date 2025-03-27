using BlindBoxShop.Entities.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages.Account.Pages
{
    public partial class OrderDetail : ComponentBase
    {
        [Parameter]
        public string OrderId { get; set; }

        private User _user;
        private OrderViewModel _order;
        private List<OrderStatusStep> _statusSteps;
        private int _statusProgressPercentage;

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
            // Khởi tạo dữ liệu mẫu ngay từ đầu để tránh null reference exception
            InitializeSampleData();
            
            await LoadUserData();
            await LoadOrderData();
            SetupStatusSteps();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (string.IsNullOrEmpty(OrderId))
            {
                NavigationManager.NavigateTo("/order-history");
                return;
            }

            await LoadOrderData();
            SetupStatusSteps();
        }

        // Phương thức khởi tạo dữ liệu mẫu
        private void InitializeSampleData()
        {
            _user = new User
            {
                FirstName = "Nguyễn",
                LastName = "Văn A",
                Email = "nguyenvana@example.com",
                CreatedAt = new DateTime(2022, 10, 15)
            };

            _order = new OrderViewModel
            {
                Id = OrderId ?? "59217",
                CustomerName = "Dianne Russell",
                Address = "4140 Parker Rd. Allentown, New Mexico 31134",
                Email = "dianne.russell@gmail.com",
                Phone = "(671) 555-0110",
                Status = "Processing",
                OrderDate = new DateTime(2023, 4, 24),
                PaymentMethod = "VNPay",
                Subtotal = 600000,
                ShippingCost = 20000,
                Total = 620000,
                Items = new List<OrderItemViewModel>
                {
                    new OrderItemViewModel
                    {
                        ProductName = "Red Capsicum",
                        Sku = "BB-RC001",
                        Price = 360000,
                        Quantity = 1,
                        ImageUrl = "images/shop/product1.jpg"
                    },
                    new OrderItemViewModel
                    {
                        ProductName = "Green Capsicum",
                        Sku = "BB-GC002",
                        Price = 240000,
                        Quantity = 1,
                        ImageUrl = "images/shop/product2.jpg"
                    }
                }
            };
        }

        private async Task LoadUserData()
        {
            try
            {
                // Mock user data for demo - nhưng đã được khởi tạo trước đó nên có thể bỏ qua
                await Task.Delay(100); // Simulate API call
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading user data: {ex.Message}", Severity.Error);
            }
        }

        private async Task LoadOrderData()
        {
            try
            {
                // _order đã được khởi tạo trước đó, nếu có OrderId cụ thể, cập nhật lại Id
                if (!string.IsNullOrEmpty(OrderId))
                {
                    _order.Id = OrderId;
                }

                // Wait a moment to simulate API call
                await Task.Delay(300);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading order data: {ex.Message}", Severity.Error);
            }
        }

        private void SetupStatusSteps()
        {
            if (_order == null) return;

            _statusSteps = new List<OrderStatusStep>
            {
                new OrderStatusStep { Step = 1, Name = "Order received", IsCompleted = true, IsActive = false },
                new OrderStatusStep { Step = 2, Name = "Processing", IsCompleted = _order.Status != "Order received", IsActive = _order.Status == "Processing" },
                new OrderStatusStep { Step = 3, Name = "On the way", IsCompleted = _order.Status == "Shipped" || _order.Status == "Delivered", IsActive = _order.Status == "On the way" },
                new OrderStatusStep { Step = 4, Name = "Delivered", IsCompleted = _order.Status == "Delivered", IsActive = _order.Status == "Delivered" }
            };

            // Calculate progress percentage
            int completedSteps = 0;
            foreach (var step in _statusSteps)
            {
                if (step.IsCompleted) completedSteps++;
            }

            if (completedSteps == 0)
                _statusProgressPercentage = 0;
            else if (completedSteps == _statusSteps.Count)
                _statusProgressPercentage = 100;
            else
                _statusProgressPercentage = (int)((completedSteps / (float)_statusSteps.Count) * 100);
        }

        private string GetStatusCircleClass(OrderStatusStep status)
        {
            string baseClass = "w-10 h-10 rounded-full flex items-center justify-center ";
            
            if (status.IsCompleted)
                return baseClass + "bg-green-500 text-white";
            else if (status.IsActive)
                return baseClass + "bg-blue-500 text-white";
            else
                return baseClass + "bg-gray-300 text-gray-600";
        }

        private string GetStatusTextClass(OrderStatusStep status)
        {
            if (status.IsCompleted)
                return "font-medium text-green-600";
            else if (status.IsActive)
                return "font-medium text-blue-600";
            else
                return "text-gray-500";
        }

        private string FormatPrice(decimal price)
        {
            return $"{price.ToString("N0")} ₫";
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

        private void CancelOrder()
        {
            try
            {
                if (_order == null) return;
                
                // In a real implementation, this would call an API to cancel the order
                _order.Status = "Cancelled";
                SetupStatusSteps();
                Snackbar.Add("Order has been cancelled successfully.", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error cancelling order: {ex.Message}", Severity.Error);
            }
        }
    }

    public class OrderViewModel
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }

    public class OrderStatusStep
    {
        public int Step { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
    }
} 