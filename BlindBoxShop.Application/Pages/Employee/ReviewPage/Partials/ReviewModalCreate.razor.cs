using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.CustomerReview;
using BlindBoxShop.Shared.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.ReviewPage.Partials
{
    public partial class ReviewModalCreate
    {
        private ReviewForCreationDto _reviewForCreate = new ReviewForCreationDto
        {
            BlindBoxId = Guid.Parse("15d17b33-4081-4ea2-b90b-6b6fde7a9ec5"),
            UserId = Guid.Parse("66c29a04-70ed-49df-8f85-6568aa80cc10"),
            RatingStar = 0,
        };

        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Inject]
        private IDialogService? DialogService { get; set; }

        private async Task ValidSubmit(EditContext context)
        {
            if (_reviewForCreate.RatingStar < 1 || _reviewForCreate.RatingStar > 5)
            {
                Snackbar.Add("Rating must be between 1 and 5.", Severity.Error);
                return;
            }

            using var reviewService = ServiceManager!.CustomerReviewsService;
            var result = await reviewService.CreateReviewAsync(_reviewForCreate);

            if (result.IsSuccess)
            {
                _reviewForCreate = new ReviewForCreationDto();

                MudDialog!.Close(DialogResult.Ok(result.GetValue<ReviewDto>()));
                ShowSnackbar("Review created successfully.", Severity.Success);
            }
            else
            {
                // Display errors if any
                var errorsMessage = string.Join(", ", result.Errors!.Select(e => e.Description));
                ShowSnackbar(errorsMessage, Severity.Error);
            }
        }

        private async Task InvalidSubmit(EditContext context)
        {
            // Display a message box for invalid form submission
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
