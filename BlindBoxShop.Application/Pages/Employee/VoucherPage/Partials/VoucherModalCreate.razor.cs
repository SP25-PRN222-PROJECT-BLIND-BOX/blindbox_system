using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Voucher;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Application.Pages.Employee.VoucherPage.Partials
{
    public partial class VoucherModalCreate
    {
        private VoucherForCreate? _voucherForCreate = new VoucherForCreate()
        {
            StartDate = DateTime.Today, // Ngày hiện tại, 0h
            EndDate = DateTime.Today.AddDays(1)
        };

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Inject]
        private IDialogService? DialogService { get; set; }



        private async Task ValidSubmit(EditContext context)
        {
            if (_voucherForCreate.EndDate < _voucherForCreate.StartDate)
            {
                Snackbar.Add("End date cannot be earlier than start date.", Severity.Error);
                return;
            }

            using var voucherService = ServiceManager!.VoucherService;
            var result = await voucherService.CreateVoucherAsync(_voucherForCreate!);

            if (result.IsSuccess)
            {
                _voucherForCreate = new();
                MudDialog!.Close(DialogResult.Ok(result.GetValue<VoucherDto>()));
                ShowVariant("Create voucher successfully.", Severity.Info);
            }
            else
            {
                var errorsMessage = result.Errors!.Select(e => e.Description).ToList();
                var errorMeesage = string.Join(", ", errorsMessage).Trim();
                ShowVariant(errorMeesage, Severity.Warning);
            }
        }

        private async Task InvalidSubmit(EditContext context)
        {
            await DialogService!.ShowMessageBox(
                            "Sorry",
                            @"You must fill all the field!",
                            yesText: "Got it",
                            options: new DialogOptions() { CloseOnEscapeKey = true });
            return;
        }

        private void Cancel() => MudDialog!.Cancel();

        private async Task OnKeyDownAsync(KeyboardEventArgs args, EditContext context)
        {
            switch (args.Key)
            {
                case "Enter":
                case "NumpadEnter":
                    if (context.Validate())
                    {
                        await ValidSubmit(context);
                    }
                    else
                    {
                        await InvalidSubmit(context);
                    }
                    break;
                case "Escape":
                    {
                        Cancel();
                        break;
                    }
            }
        }

        private void ShowVariant(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, c => c.SnackbarVariant = Variant.Text);
        }
    }
}
