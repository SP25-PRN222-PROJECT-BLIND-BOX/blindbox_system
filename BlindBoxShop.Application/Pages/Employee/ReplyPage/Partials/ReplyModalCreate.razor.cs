using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Reply;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using BlindBoxShop.Shared.Extension;

namespace BlindBoxShop.Application.Pages.Employee.ReplyPage.Partials
{
    public partial class ReplyModalCreate
    {
        private ReplyForCreationDto _replyForCreate = new ReplyForCreationDto
        {
            CustomerReviewsId = Guid.Parse("86a131dc-9bc7-4d5f-882f-de46ee292d84"),
            UserId = Guid.Parse("4b58c3fa-e57e-49a0-8f2b-e436e70d6e17"),
        };

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Inject]
        private IDialogService? DialogService { get; set; }

        private async Task ValidSubmit(EditContext context)
        {
            if (string.IsNullOrWhiteSpace(_replyForCreate.Reply))
            {
                Snackbar.Add("Reply content is required.", Severity.Error);
                return;
            }

            using var replyService = ServiceManager!.ReplyService;
            var result = await replyService.CreateReplyAsync(_replyForCreate);

            if (result.IsSuccess)
            {
                _replyForCreate = new ReplyForCreationDto();

                MudDialog!.Close(DialogResult.Ok(result.GetValue<ReplyDto>()));
                ShowSnackbar("Reply created successfully.", Severity.Success);
            }
            else
            {
                var errorsMessage = string.Join(", ", result.Errors!.Select(e => e.Description));
                ShowSnackbar(errorsMessage, Severity.Error);
            }
        }

        private async Task InvalidSubmit(EditContext context)
        {
            await DialogService!.ShowMessageBox(
                "Validation Error",
                "Please fill in all required fields correctly.",
                yesText: "OK",
                options: new DialogOptions { CloseOnEscapeKey = true });
        }

        private void Cancel()
        {
            MudDialog!.Cancel();
        }

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
                    Cancel();
                    break;
            }
        }

        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, options => options.SnackbarVariant = Variant.Text);
        }
    }
}