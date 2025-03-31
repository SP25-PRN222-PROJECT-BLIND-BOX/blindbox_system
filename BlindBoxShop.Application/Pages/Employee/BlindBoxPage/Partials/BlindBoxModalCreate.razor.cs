using AutoMapper;

using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxItems;
using BlindBoxShop.Shared.DataTransferObject.Package;
using BlindBoxShop.Shared.Features;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using System.Globalization;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxPage.Partials
{
    public partial class BlindBoxModalCreate : ComponentBase
    {
        private IEnumerable<BlindBoxCategoryDto>? _categories = new List<BlindBoxCategoryDto>();
        private IEnumerable<PackageDto>? _packages = new List<PackageDto>();
        private bool _isProcessing = false;
        private string? _errorMessage;
        private BlindBoxForCreate _blindBoxForCreate = new();
        private List<BlindBoxItemDtoForCreation> _blindBoxItems = new();

        // For item dialog
        private bool _showItemDialog = false;
        private BlindBoxItemDtoForCreation _currentItem = new();
        private int _editItemIndex = -1;
        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium };

        [CascadingParameter]
        public IMudDialogInstance? MudDialog { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _blindBoxForCreate.BlindBoxItems = _blindBoxItems;
            await LoadCategoriesAsync();
            await LoadPackagesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            using var blindBoxCategoryService = ServiceManager!.BlindBoxCategoryService;
            var categoryParams = new BlindBoxCategoryParameter { PageSize = 100 }; // Get all for dropdown
            var result = await blindBoxCategoryService.GetBlindBoxCategoriesAsync(categoryParams, false);

            if (result.IsSuccess)
            {
                _categories = result.Value;
            }
        }

        private async Task LoadPackagesAsync()
        {
            using var packageService = ServiceManager!.PackageService;
            var result = await packageService.GetAllPackagesAsync(false);

            if (result.IsSuccess)
            {
                _packages = result.Value;
            }
        }

        private void Close() => MudDialog!.Close();

        private string FormatPrice(decimal price)
        {
            return string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0}", price) + " ₫";
        }

        private bool IsFormValid()
        {
            return !string.IsNullOrWhiteSpace(_blindBoxForCreate.Name) &&
                   !string.IsNullOrWhiteSpace(_blindBoxForCreate.Description) &&
                   _blindBoxForCreate.BlindBoxCategoryId != Guid.Empty &&
                   _blindBoxForCreate.PackageId != Guid.Empty &&
                   _blindBoxForCreate.Price > 0;
        }

        private async Task CreateBlindBoxAsync()
        {
            if (!IsFormValid() || _isProcessing)
            {
                ShowSnackbar("Please fill in all required fields", Severity.Warning);
                return;
            }

            _isProcessing = true;
            _errorMessage = null;

            // Update the collection of items
            _blindBoxForCreate.BlindBoxItems = _blindBoxItems;

            using var blindBoxService = ServiceManager!.BlindBoxService;
            var result = await blindBoxService.CreateBlindBoxAsync(_blindBoxForCreate);

            if (result.IsSuccess)
            {
                ShowSnackbar($"Create BlindBox with name {_blindBoxForCreate.Name} successfully.", Severity.Success);
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

        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, config => config.SnackbarVariant = Variant.Text);
        }

        // Methods for BlindBox Items
        private void AddNewBlindBoxItem()
        {
            _currentItem = new BlindBoxItemDtoForCreation();
            _editItemIndex = -1;
            _showItemDialog = true;
        }

        private void EditBlindBoxItem(BlindBoxItemDtoForCreation item)
        {
            _editItemIndex = _blindBoxItems.IndexOf(item);
            _currentItem = new BlindBoxItemDtoForCreation
            {
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageUrl
            };
            _showItemDialog = true;
        }

        private void RemoveBlindBoxItem(BlindBoxItemDtoForCreation item)
        {
            _blindBoxItems.Remove(item);
            StateHasChanged();
        }

        private void SaveBlindBoxItem()
        {
            if (string.IsNullOrWhiteSpace(_currentItem.Name))
            {
                ShowSnackbar("Item name is required", Severity.Warning);
                return;
            }

            if (_editItemIndex >= 0)
            {
                // Update existing item
                _blindBoxItems[_editItemIndex] = _currentItem;
            }
            else
            {
                // Add new item
                _blindBoxItems.Add(_currentItem);
            }

            _showItemDialog = false;
            StateHasChanged();
        }

        private void CancelItemEdit()
        {
            _showItemDialog = false;
        }
    }
}
