using System.Globalization;
using AutoMapper;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.Enum;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Manager.Package.Partials
{
    public partial class PackageCreateModal : ComponentBase
    {
        private bool _isProcessing = false;
        private string? _errorMessage;
        private PackageForCreate _packageForCreate = new();

        [CascadingParameter]
        public IMudDialogInstance? MudDialog { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        /// <summary>
        /// Khởi tạo dữ liệu khi component được load
        /// Set giá trị mặc định cho package type là Standard
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            _packageForCreate = new PackageForCreate
            {
                Type = PackageType.Standard
            };
            await Task.CompletedTask;
        }

        /// <summary>
        /// Đóng dialog
        /// </summary>
        private void Close() => MudDialog!.Close();

        /// <summary>
        /// Format giá tiền theo định dạng tiền Việt Nam
        /// </summary>
        private string FormatPrice(decimal price)
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0}", price) + " ₫";
        }

        /// <summary>
        /// Kiểm tra form có hợp lệ không
        /// Các trường bắt buộc phải được nhập
        /// </summary>
        private bool IsFormValid()
        {
            return !string.IsNullOrWhiteSpace(_packageForCreate.Name) &&
                   !string.IsNullOrWhiteSpace(_packageForCreate.Barcode) &&
                   Enum.IsDefined(typeof(PackageType), _packageForCreate.Type) &&
           
                   _packageForCreate.TotalBlindBox > 0 &&
                   _packageForCreate.CurrentTotalBlindBox > 0;
        }

        /// <summary>
        /// Xử lý việc tạo mới package
        /// Kiểm tra form và gọi API tạo package
        /// </summary>
        private async Task CreatPackageAsync()
        {
            if (!IsFormValid() || _isProcessing)
            {
                ShowSnackbar("Please fill in all required fields", Severity.Warning);
                return;
            }

            _isProcessing = true;
            _errorMessage = null;

            using var packageService = ServiceManager!.PackageService;
            _packageForCreate.UpdateDate = DateTime.Now;
            var result = await packageService.CreatePackageAsync(_packageForCreate);

            if (result.IsSuccess)
            {
                ShowSnackbar($"Create BlindBox with name {_packageForCreate.Name} successfully.", Severity.Success);
                MudDialog!.Close(DialogResult.Ok(true));
            }
            else
            {
                var errorMessages = result.Errors?.Select(e => e.Description).ToList() ?? new List<string> { "Unknown error occurred" };
                _errorMessage = string.Join(", ", errorMessages);
                ShowSnackbar(_errorMessage, Severity.Error);
            }

            _isProcessing = false;
        }

        /// <summary>
        /// Hiển thị thông báo snackbar
        /// </summary>
        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, config => config.SnackbarVariant = Variant.Text);
        }
    }
}