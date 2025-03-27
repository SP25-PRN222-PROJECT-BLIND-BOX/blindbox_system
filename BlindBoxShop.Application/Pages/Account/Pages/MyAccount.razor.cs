using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
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
        private string _username;
        private string _email;
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

        [Inject]
        private UserManager<User> UserManager { get; set; }

        [Inject]
        private IdentityUserAccessor UserAccessor { get; set; }

        [Inject]
        private IDialogService DialogService { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

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
                _user = await UserAccessor.GetRequiredUserAsync(HttpContext);
                _username = await UserManager.GetUserNameAsync(_user);
                _email = await UserManager.GetEmailAsync(_user);

                _profileModel = new ProfileModel
                {
                    FirstName = _user.FirstName,
                    LastName = _user.LastName,
                    PhoneNumber = _user.PhoneNumber,
                    Address = _user.Address,
                    Province = _user.Provinces,
                    District = _user.District,
                    Ward = _user.Wards
                };

                // Populate form fields
                Username = _user.UserName;
                Phone = _user.PhoneNumber;
                FullName = $"{_user.FirstName} {_user.LastName}";
                Email = _user.Email;
                PhoneWithCode = _user.PhoneNumber;
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
                // In a real implementation, you would call your service to get orders
                // For now, we'll just use dummy data
                await Task.Delay(500); // Simulate API call

                _orders = GetDummyOrders();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading order history: {ex.Message}", Severity.Error);
            }
            finally
            {
                _loading = false;
            }
        }

        private async Task SaveProfile()
        {
            try
            {
                // In a real implementation, you would call your service to update the user profile
                await Task.Delay(500); // Simulate API call

                // Update the user object with the new values
                _user.FirstName = _profileModel.FirstName;
                _user.LastName = _profileModel.LastName;
                _user.PhoneNumber = _profileModel.PhoneNumber;
                _user.Address = _profileModel.Address;
                _user.Provinces = _profileModel.Province;
                _user.District = _profileModel.District;
                _user.Wards = _profileModel.Ward;

                Snackbar.Add("Profile updated successfully", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error saving profile: {ex.Message}", Severity.Error);
            }
        }

        private void OpenOrderDetails(Guid orderId)
        {
            // In a real implementation, you would open a dialog or navigate to a details page
            Snackbar.Add($"Viewing details for order {orderId}", Severity.Info);
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

        // Dummy data for demonstration purposes - using only valid OrderStatus values
        private List<OrderDto> GetDummyOrders()
        {
            return new List<OrderDto>
            {
                new OrderDto
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now.AddDays(-2),
                    Status = OrderStatus.Pending,
                    Total = 540000,
                    Address = "123 Main St",
                    Province = "Ho Chi Minh"
                },
                new OrderDto
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now.AddDays(-7),
                    Status = OrderStatus.Pending,
                    Total = 780000,
                    Address = "456 Oak St",
                    Province = "Ha Noi"
                },
                new OrderDto
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now.AddDays(-15),
                    Status = OrderStatus.Cancelled,
                    Total = 350000,
                    Address = "789 Pine St",
                    Province = "Da Nang"
                }
            };
        }

        private void SaveChanges()
        {
            try
            {
                // In a real implementation, this would update user data
                // For demo purposes, just show a notification
                
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
                
                _user.Email = Email;
                _user.PhoneNumber = Phone;
                
                Snackbar.Add("Profile updated successfully", Severity.Success);
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