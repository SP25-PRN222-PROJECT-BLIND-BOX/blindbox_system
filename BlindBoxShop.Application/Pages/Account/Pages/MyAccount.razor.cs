using BlindBoxShop.Entities.Models;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BlindBoxShop.Application.Pages.Account.Pages
{
    public partial class MyAccount : ComponentBase
    {
        private User _user;
        private ProfileModel _profileModel = new();
        private int _activeTab = 0;
        private List<OrderDto> _orders;
        private bool _loading = true;
        private bool _uploading;
        
        // Form fields
        public string Username { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneWithCode { get; set; }
        
        // Address related properties
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }

        [Inject]
        private UserManager<User> UserManager { get; set; }

        [Inject]
        private IServiceManager ServiceManager { get; set; }

        [Inject]
        private IDialogService DialogService { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }
        
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadUserData();
            await LoadOrderHistory();
        }

        private async Task LoadUserData()
        {
            try
            {
                _loading = true;
                
                // Get the current user ID from localStorage
                var userId = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_id");
                
                if (string.IsNullOrEmpty(userId))
                {
                    Snackbar.Add("Không tìm thấy thông tin đăng nhập, vui lòng đăng nhập lại", Severity.Warning);
                    NavigationManager.NavigateTo("/account/login");
                    return;
                }

                try
                {
                    // Use the user service to get user info
                    var userObj = await ServiceManager.UserService.GetUserByIdAsync(Guid.Parse(userId), false);
                    _user = userObj as User;
                    
                    if (_user == null)
                    {
                        Snackbar.Add("Không tìm thấy thông tin người dùng", Severity.Error);
                        NavigationManager.NavigateTo("/account/login");
                        return;
                    }

                    // Initialize profile model
                    _profileModel = new ProfileModel
                    {
                        FirstName = _user.FirstName,
                        LastName = _user.LastName,
                        PhoneNumber = _user.PhoneNumber,
                        // Try to get address info from user object if available
                        Address = _user.Address,
                        Province = _user.Provinces,
                        District = _user.District,
                        Ward = _user.Wards
                    };

                    // Populate form fields
                    Username = _user.UserName;
                    Phone = _user.PhoneNumber ?? "";
                    FullName = $"{_user.FirstName} {_user.LastName}";
                    Email = _user.Email;
                    PhoneWithCode = _user.PhoneNumber ?? "";
                    
                    // Address related properties
                    Address = _user.Address ?? "";
                    Province = _user.Provinces ?? "";
                    District = _user.District ?? "";
                    Ward = _user.Wards ?? "";
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Lỗi khi lấy thông tin người dùng: {ex.Message}", Severity.Error);
                    Console.WriteLine($"Error fetching user data: {ex}");
                    NavigationManager.NavigateTo("/account/login");
                    return;
                }
                
                _loading = false;
            }
            catch (Exception ex)
            {
                _loading = false;
                Snackbar.Add($"Lỗi khi tải thông tin: {ex.Message}", Severity.Error);
                Console.WriteLine($"General error in LoadUserData: {ex}");
            }
        }

        private async Task LoadOrderHistory()
        {
            try
            {
                _loading = true;
                
                // Get user ID from localStorage
                var userId = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_id");
                
                if (string.IsNullOrEmpty(userId))
                {
                    Snackbar.Add("Không tìm thấy thông tin người dùng để lấy lịch sử đơn hàng", Severity.Warning);
                    _orders = new List<OrderDto>();
                    return;
                }
                
                // Create parameter for pagination and sorting
                var orderParameter = new BlindBoxShop.Shared.Features.OrderParameter
                {
                    PageSize = 10,  // Show 10 orders per page
                    PageNumber = 1, // Start with first page
                    OrderBy = "CreatedAt desc" // Most recent orders first
                };
                
                // Get orders for the user from database
                var result = await ServiceManager.OrderService.GetOrdersByUserIdAsync(
                    Guid.Parse(userId), 
                    orderParameter, 
                    false
                );
                
                if (result.IsSuccess)
                {
                    _orders = result.Value.ToList();
                    
                    if (_orders.Count == 0)
                    {
                        Snackbar.Add("Bạn chưa có đơn hàng nào", Severity.Info);
                    }
                }
                else
                {
                    Snackbar.Add($"Lỗi khi tải lịch sử đơn hàng: {result.Errors?.FirstOrDefault()?.Description}", Severity.Error);
                    _orders = new List<OrderDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order history: {ex}");
                Snackbar.Add($"Lỗi khi tải lịch sử đơn hàng: {ex.Message}", Severity.Error);
                _orders = new List<OrderDto>();
            }
            finally
            {
                _loading = false;
            }
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

        private Color GetOrderStatusColor(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => Color.Warning,
                OrderStatus.Cancelled => Color.Error,
                _ => Color.Default
            };
        }

        private string FormatPrice(decimal price)
        {
            return $"{price.ToString("N0")} ₫";
        }

        private async Task ViewOrderDetails(OrderWithDetailsDto order)
        {
            var parameters = new DialogParameters { ["OrderWithDetails"] = order };
            var options = new DialogOptions 
            { 
                CloseButton = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            };
            var dialog = await DialogService.ShowAsync<Components.Dialogs.OrderDetailsDialog>("Order Details", parameters, options);
            var result = await dialog.Result;
        }

        private async Task ViewOrderDetailsByIdAsync(Guid orderId)
        {
            try
            {
                var result = await ServiceManager.OrderService.GetOrderWithDetailsByIdAsync(orderId, false);
                
                if (result.IsSuccess)
                {
                    var orderWithDetails = result.Value;
                    await ViewOrderDetails(orderWithDetails);
                }
                else
                {
                    Snackbar.Add($"Cannot view order details: {result.Errors?.FirstOrDefault()?.Description}", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error viewing order details: {ex}");
                Snackbar.Add($"Error loading order details: {ex.Message}", Severity.Error);
            }
        }

        private async Task SaveChanges()
        {
            try
            {
                // Parse full name into first and last name
                var nameParts = FullName?.Split(' ');
                if (nameParts?.Length > 0)
                {
                    _user.FirstName = nameParts[0];
                    
                    if (nameParts.Length > 1)
                    {
                        _user.LastName = string.Join(" ", nameParts, 1, nameParts.Length - 1);
                    }
                }
                
                // Update the user profile data
                _user.Email = Email;
                _user.PhoneNumber = Phone;
                
                // Update address information directly on user object
                _user.Address = Address;
                _user.Provinces = Province;
                _user.District = District;
                _user.Wards = Ward;
                
                // Save the user to the database
                var result = await ServiceManager.UserService.UpdateUserAsync(_user.Id, _user);
                
                if (result)
                {
                    Snackbar.Add("Profile updated successfully", Severity.Success);
                }
                else
                {
                    Snackbar.Add("Failed to update profile", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error saving changes: {ex.Message}", Severity.Error);
            }
        }
        
        private void CancelChanges()
        {
            // Reset form to current user data
            Username = _user.UserName;
            Phone = _user.PhoneNumber;
            FullName = $"{_user.FirstName} {_user.LastName}";
            Email = _user.Email;
            PhoneWithCode = _user.PhoneNumber;
            
            // Reset address related properties
            Address = _user.Address ?? "";
            Province = _user.Provinces ?? "";
            District = _user.District ?? "";
            Ward = _user.Wards ?? "";
            
            Snackbar.Add("Changes cancelled", Severity.Info);
        }
        
        private async Task UploadProfilePicture(InputFileChangeEventArgs e)
        {
            try
            {
                _uploading = true;
                
                // In a real implementation, this would upload the file to a server
                // For demo purposes, just simulate a delay
                await Task.Delay(1500);
                
                _uploading = false;
                
                Snackbar.Add("Profile picture updated successfully", Severity.Success);
            }
            catch (Exception ex)
            {
                _uploading = false;
                Snackbar.Add($"Error uploading profile picture: {ex.Message}", Severity.Error);
            }
        }

        public class ProfileModel
        {
            [Required(ErrorMessage = "First name is required")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last name is required")]
            public string LastName { get; set; }

            [Phone(ErrorMessage = "Invalid phone number")]
            public string PhoneNumber { get; set; }

            public string Address { get; set; }

            public string Province { get; set; }

            public string District { get; set; }

            public string Ward { get; set; }
        }
    }
} 