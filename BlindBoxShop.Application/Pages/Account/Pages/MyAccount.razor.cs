using BlindBoxShop.Entities.Models;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
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
using Microsoft.AspNetCore.WebUtilities;

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
        
        // Image preview properties
        private bool _imagePreviewVisible;
        private string _selectedImage;
        private bool _isZoomed;
        
        // Dictionary to store BlindBoxItems for order details
        private Dictionary<Guid, BlindBoxItemDto> _blindBoxItems = new Dictionary<Guid, BlindBoxItemDto>();
        private Dictionary<Guid, bool> _itemsLoading = new Dictionary<Guid, bool>();
        
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

        [Parameter]
        [SupplyParameterFromQuery(Name = "tab")]
        public string TabParam { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _loading = true;
                
                // Process tab parameter from URL
                if (!string.IsNullOrEmpty(TabParam) && int.TryParse(TabParam, out int tabIndex))
                {
                    _activeTab = tabIndex;
                }
                
                await LoadUserData();
                await LoadOrderHistory();
                
                _loading = false;
            }
            catch (Exception ex)
            {
                // Snackbar.Add($"Error loading account data: {ex.Message}", Severity.Error);
                _loading = false;
            }
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
                    Snackbar.Add("User information not found, please login again", Severity.Warning);
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
                        Snackbar.Add("User information not found", Severity.Error);
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
                    // Snackbar.Add($"Error retrieving user information: {ex.Message}", Severity.Error);
                    Console.WriteLine($"Error fetching user data: {ex}");
                    NavigationManager.NavigateTo("/account/login");
                    return;
                }
                
                _loading = false;
            }
            catch (Exception ex)
            {
                _loading = false;
                // Snackbar.Add($"Error loading information: {ex.Message}", Severity.Error);
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
                    Snackbar.Add("User information not found to retrieve order history", Severity.Warning);
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
                        Snackbar.Add("You don't have any orders yet", Severity.Info);
                    }
                }
                else
                {
                    // Snackbar.Add($"Error loading order history: {result.Errors?.FirstOrDefault()?.Description}", Severity.Error);
                    _orders = new List<OrderDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order history: {ex}");
                // Snackbar.Add($"Error loading order history: {ex.Message}", Severity.Error);
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
                OrderStatus.Processing => Color.Info,
                OrderStatus.Delivered => Color.Success,
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
                // Fetch order details
                var result = await ServiceManager.OrderService.GetOrderWithDetailsByIdAsync(orderId, false);
                
                if (result.IsSuccess && result.Value != null)
                {
                    // Pre-load BlindBoxItems for this order
                    await LoadBlindBoxItemsForOrder(result.Value);
                    
                    // Show order details dialog
                    var parameters = new DialogParameters { 
                        ["OrderWithDetails"] = result.Value,
                        ["Phone"] = _user?.PhoneNumber ?? "N/A",
                        ["BlindBoxItems"] = _blindBoxItems
                    };
                    var options = new DialogOptions 
                    { 
                        CloseButton = true,
                        MaxWidth = MaxWidth.Medium,
                        FullWidth = true
                    };
                    
                    var dialog = await DialogService.ShowAsync<Components.Dialogs.OrderDetailsDialog>("Order Details", parameters, options);
                    var dialogResult = await dialog.Result;
                    
                    // If dialog result is Ok, refresh order history
                    if (!dialogResult.Canceled)
                    {
                        await LoadOrderHistory();
                    }
                }
                else
                {
                    var errorMsg = result.Errors?.FirstOrDefault()?.Description ?? "Cannot load order information";
                    Snackbar.Add(errorMsg, Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
        }

        // Load BlindBoxItems for an order
        private async Task LoadBlindBoxItemsForOrder(OrderWithDetailsDto order)
        {
            try
            {
                if (order?.OrderDetails == null || !order.OrderDetails.Any()) 
                    return;
                
                foreach (var detail in order.OrderDetails)
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading BlindBoxItems: {ex.Message}");
            }
        }
        
        // Load a single BlindBoxItem
        private async Task LoadBlindBoxItem(Guid orderDetailId, Guid blindBoxItemId)
        {
            try
            {
                using var blindBoxItemService = ServiceManager.BlindBoxItemService;
                var result = await blindBoxItemService.GetBlindBoxItemByIdAsync(blindBoxItemId, false);
                
                if (result.IsSuccess && result.Value != null)
                {
                    _blindBoxItems[orderDetailId] = result.Value;
                }
                else
                {
                    Console.WriteLine($"Failed to load BlindBoxItem {blindBoxItemId}: {result.Errors?.FirstOrDefault()?.Description}");
                    _blindBoxItems[orderDetailId] = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading BlindBoxItem: {ex.Message}");
                _blindBoxItems[orderDetailId] = null;
            }
        }

        // Image preview methods
        private void OpenImagePreview(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl) || imageUrl == "/images/box-placeholder.jpg")
            {
                return;
            }
            
            _selectedImage = imageUrl;
            _imagePreviewVisible = true;
            StateHasChanged();
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