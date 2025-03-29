using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Timers;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using BlindBoxShop.Entities.Models;
using BlindBoxShop.Application.Models;

namespace BlindBoxShop.Application.Components.Layout
{
    public partial class MainLayout : IDisposable
    {
        // Static instance cho phép truy cập từ bất kỳ component nào
        public static MainLayout Instance { get; private set; }
        
        private bool _drawerOpen = false;
        private string _selectedCategory = "All";
        private int _cartItemCount = 0;
        private System.Timers.Timer _cartCheckTimer;
        
        // Thông tin người dùng từ localStorage
        private bool _isUserLoggedIn = false;
        private string _userName = "";
        private string _userEmail = "";
        private List<string> _userRoles = new List<string>();
        
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        
        [Inject]
        private SignInManager<User> SignInManager { get; set; }
        
        [Inject]
        private ISnackbar Snackbar { get; set; }

        protected override void OnInitialized()
        {
            // Lưu instance hiện tại để các component khác có thể truy cập
            Instance = this;
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await CheckUserLoginStatus();
            
            _cartCheckTimer = new System.Timers.Timer(2000); // Kiểm tra mỗi 2 giây
            _cartCheckTimer.Elapsed += CheckCartItemsCount;
            _cartCheckTimer.AutoReset = true;
            _cartCheckTimer.Start();

            // Gọi hàm kiểm tra ngay lập tức
            await UpdateCartItemCount();
        }
        
        /// <summary>
        /// Phương thức công khai để cập nhật trạng thái đăng nhập từ các component khác.
        /// Được gọi sau khi người dùng đăng nhập thành công để cập nhật UI của MainLayout.
        /// </summary>
        public async Task NotifyUserLogin()
        {
            await CheckUserLoginStatus();
            StateHasChanged();
        }
        
        private async Task CheckUserLoginStatus()
        {
            try
            {
                // Kiểm tra localStorage
                var userId = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_id");
                _isUserLoggedIn = !string.IsNullOrEmpty(userId);
                
                if (_isUserLoggedIn)
                {
                    _userName = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_name") ?? "";
                    _userEmail = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_email") ?? "";
                    var rolesString = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user_roles") ?? "";
                    _userRoles = !string.IsNullOrEmpty(rolesString) ? rolesString.Split(',').ToList() : new List<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking login status: {ex.Message}");
                _isUserLoggedIn = false;
            }
        }

        private async void CheckCartItemsCount(object sender, ElapsedEventArgs e)
        {
            await InvokeAsync(UpdateCartItemCount);
        }

        private async Task UpdateCartItemCount()
        {
            try
            {
                var cartJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "blindbox_cart");
                
                if (!string.IsNullOrEmpty(cartJson))
                {
                    var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                    
                    if (cartItems != null)
                    {
                        var totalCount = cartItems.Sum(item => item.Quantity);
                        
                        // Chỉ cập nhật UI nếu số lượng sản phẩm thay đổi
                        if (_cartItemCount != totalCount)
                        {
                            _cartItemCount = totalCount;
                            StateHasChanged();
                        }
                    }
                    else
                    {
                        _cartItemCount = 0;
                    }
                }
                else
                {
                    _cartItemCount = 0;
                }
            }
            catch (Exception)
            {
                _cartItemCount = 0;
            }
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
        
        private async Task HandleLogout()
        {
            try
            {
                // Đầu tiên, xóa dữ liệu người dùng khỏi localStorage
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "user_id");
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "user_email");
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "user_name");
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "user_roles");
                
                // Cập nhật trạng thái ứng dụng
                _isUserLoggedIn = false;
                
                // Thực hiện đăng xuất từ Identity
                try
                {
                    await SignInManager.SignOutAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SignOut error: {ex.Message}");
                    // Tiếp tục xử lý ngay cả khi có lỗi đăng xuất
                }
                
                // Thông báo thành công
                Snackbar.Add("Đăng xuất thành công", Severity.Success);
                
                // Chuyển hướng về trang chủ - đặt cuối cùng để tránh lỗi header
                // forceLoad = true để tải lại trang và làm mới trạng thái hoàn toàn
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during logout: {ex.Message}");
                Snackbar.Add("Đã xảy ra lỗi khi đăng xuất", Severity.Error);
            }
        }

        public void Dispose()
        {
            _cartCheckTimer?.Stop();
            _cartCheckTimer?.Dispose();
        }
    }
}