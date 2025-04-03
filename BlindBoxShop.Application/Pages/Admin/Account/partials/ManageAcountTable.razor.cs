using AutoMapper;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Account;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Admin.Account.partials
{
    public partial class ManageAcountTable: ComponentBase
    {
           private IEnumerable<AccountDto>? pagedData;
        private MudTable<AccountDto>? table;
        private string? searchString;
        private AccountDto? _accountDtoBeforeEdit;
        private Timer? _timer;

        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

        private MetaData? _metaData { get; set; } = new MetaData();
        private AccountParameter _accountParameters = new AccountParameter();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _accountParameters.PageSize = 10; // Set default page size
        }

        /// <summary>
        /// Load table data from server with pagination and sorting support
        /// </summary>
        private async Task<TableData<AccountDto>> ServerReload(TableState state, CancellationToken token)
        {
            try
            {
                _accountParameters.PageNumber = state.Page + 1;
                _accountParameters.PageSize = state.PageSize;
                _accountParameters.SearchTerm = searchString?.Trim();

                using var accountService = ServiceManager!.UserService;
                var result = await accountService.GetAllAccountsAsync(_accountParameters, false);

                if (result.IsSuccess && result.Value != null)
                {
                    pagedData = result.Value;
                    var totalItems = await accountService.GetTotalCountAsync(_accountParameters, false);
                    return new TableData<AccountDto>()
                    {
                        TotalItems = totalItems,
                        Items = pagedData
                    };
                }

                ShowSnackbar("Failed to load package data", Severity.Error);
                return new TableData<AccountDto>() { TotalItems = 0, Items = Enumerable.Empty<AccountDto>() };
            }
            catch (Exception ex)
            {
                ShowSnackbar($"Error: {ex.Message}", Severity.Error);
                return new TableData<AccountDto>() { TotalItems = 0, Items = Enumerable.Empty<AccountDto>() };
            }
        }

        /// <summary>
        /// Reload table data and reset related values
        /// </summary>
        public async Task ReloadDataAsync()
        {
            if (table != null)
            {
                await table.ReloadServerData();
            }
        }

        /// <summary>
        /// Handle search text change event
        /// </summary>
        private async Task OnSearch(string? text)
        {
            searchString = text;
            if (table != null)
            {
                table.CurrentPage = 0; // Reset to first page
                await table.ReloadServerData();
            }
        }

        /// <summary>
        /// Handle type filter change event
        /// </summary>
        // private async Task OnTypeChanged(int? type)
        // {
        //     _accountParameters.Role = Role;
        //     if (table != null)
        //     {
        //         table.CurrentPage = 0; // Reset to first page
        //         await table.ReloadServerData();
        //     }
        // }

        /// <summary>
        /// Backup package data before editing
        /// </summary>
        private void BackupItem(object element)
        {
            if (element is AccountDto dto)
            {
                _accountDtoBeforeEdit = Mapper!.Map<AccountDto>(dto);
            }
        }

        /// <summary>
        /// Handle when user commits package edit
        /// Check conditions and perform update
        /// </summary>
        private async void ItemHasBeenCommitted(object element)
        {
            if (element is not AccountDto editedItem)
                return;

            // Check if package is Opened, don't allow update
            // if (editedItem.Type == PackageType.Opened)
            // {
            //     ShowSnackbar($"Cannot update package '{editedItem.Name}' because it is currently opened.", Severity.Warning);
            //     ResetItemToOriginalValues(element);
            //     return;
            // }

            if (!HasChanges(editedItem))
            {
                ShowSnackbar("No changes made. Update not performed.", Severity.Info);
                ResetItemToOriginalValues(element);
                return;
            }

            var accountForUpdate = Mapper!.Map<AccountForUpdateDto>(editedItem);
            if (accountForUpdate is null)
            {
                ShowSnackbar("Failed to update package", Severity.Error);
                return;
            }

            var accountService = ServiceManager!.UserService;
            var result = await accountService.UpdateAccountAsync(editedItem.Id, accountForUpdate, false);

            if (result.IsSuccess)
            {
                ShowSnackbar($"Account {accountForUpdate.FirstName} updated successfully.", Severity.Success);
                await ReloadDataAsync();
            }
            else
            {
                var errorMessages = result.Errors?.Select(e => e.Description).ToList() ?? new List<string> { "Unknown error occurred" };
                ShowSnackbar(string.Join(", ", errorMessages), Severity.Error);
                ResetItemToOriginalValues(element);
            }
        }

        /// <summary>
        /// Check if package has changes compared to backup data
        /// </summary>
        private bool HasChanges(AccountDto currentItem)
        {
            return _accountDtoBeforeEdit != null &&
                  (currentItem.FirstName != _accountDtoBeforeEdit.FirstName ||
                   currentItem.LastName != _accountDtoBeforeEdit.LastName ||
                   currentItem.Email != _accountDtoBeforeEdit.Email ||
                   currentItem.PhoneNumber != _accountDtoBeforeEdit.PhoneNumber ||
                //    currentItem.Role != _accountDtoBeforeEdit.Role ||
                   currentItem.IsActive != _accountDtoBeforeEdit.IsActive);
        }

        private void ResetItemToOriginalValues(object element)
        {
            if (_accountDtoBeforeEdit != null && element is AccountDto dto)
            {
                Mapper!.Map(_accountDtoBeforeEdit, dto);
            }
        }

        /// <summary>
        /// Display snackbar notification
        /// </summary>
        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, config => config.SnackbarVariant = Variant.Text);
        }

        /// <summary>
        /// Open create package dialog
        /// </summary>
        private async Task OpenCreateDialogAsync()
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                Position = DialogPosition.Center,
                NoHeader = false
            };

            var dialog = await DialogService!.ShowAsync<CreateAccountModal>("Create Account", options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await ResetTableState();
            }
        }

        /// <summary>
        /// Open remove package dialog
        /// </summary>
        private async Task OpenRemoveDialogAsync(AccountDto account)
        {
            if (account == null) return;

            var options = new DialogOptions { CloseOnEscapeKey = true };
            var parameters = new DialogParameters();
            parameters.Add("AccountItem", account);

            var dialog = await DialogService!.ShowAsync<AccountDeleteModal>("Delete Account", parameters, options);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Canceled)
            {
                await ResetTableState();
            }
        }

        private async Task ResetTableState()
        {
            if (table != null)
            {
                await table.ReloadServerData();
            }
        }

        /// <summary>
        /// Format price in Vietnamese currency
        /// </summary>
       
        /// <summary>
        /// Get display color for each package type
        /// </summary>

    }
}