using AutoMapper;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.Reply;
using BlindBoxShop.Shared.DataTransferObject.Review;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Pages.Employee.ReviewPage.Partials
{
    public partial class ReviewTable
    {
        private IEnumerable<ReviewWithReplyDto>? pagedData; // Dữ liệu chứa reviews và replies
        private string? searchString;
        private Timer? _timer;
        private string replyText = string.Empty;
        private string CardClass => "w-full sm:w-[350px] md:w-[400px] lg:w-[450px]";
        [Inject]
        public IDialogService? DialogService { get; set; }

        [Inject]
        public IServiceManager? ServiceManager { get; set; }
        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Inject]
        public IMapper? Mapper { get; set; }

        private MetaData? _metaData { get; set; } = new MetaData();

        private ReviewParameter _reviewParameters = new ReviewParameter();

        /// <summary>
        /// Reloads reviews with replies from the server.
        /// </summary>
        private async Task ReloadReviewsWithRepliesAsync()
        {
            _reviewParameters.SearchByUsername = searchString;

            using var reviewService = ServiceManager!.CustomerReviewsService;
            var result = await reviewService.GetReviewsWithReplyAsync(_reviewParameters, false);

            if (result.IsSuccess)
            {
                pagedData = result.GetValue<IEnumerable<ReviewWithReplyDto>>();
                _metaData = result.Paging;
                StateHasChanged();
            }
            else
            {
                ShowVariant("Failed to load reviews", Severity.Error);
            }
        }

        private async Task SubmitReplyAsync(Guid reviewId)
        {
            if (string.IsNullOrWhiteSpace(replyText))
            {
                Snackbar.Add("Reply cannot be empty!", Severity.Error);
                return;
            }

            try
            {
                var replyForCreate = new ReplyForCreationDto
                {
                    UserId = Guid.Parse("a5798b68-246a-4ef2-83f0-8c235c54b64a"),
                    CustomerReviewsId = reviewId,
                    Reply = replyText,
                };

                using var replyService = ServiceManager!.ReplyService;
                var result = await replyService.CreateReplyAsync(replyForCreate);

                if (result.IsSuccess)
                {
                    replyText = string.Empty;

                    Snackbar.Add("Reply created successfully.", Severity.Success);

                    await ReloadDataAsync();
                }
                else
                {
                    var errorsMessage = string.Join(", ", result.Errors!.Select(e => e.Description));
                    Snackbar.Add(errorsMessage, Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await ReloadReviewsWithRepliesAsync();
        }

        public async Task ReloadDataAsync()
        {
            await ReloadReviewsWithRepliesAsync();
        }

        private async Task OnSearch(string? text)
        {
            searchString = text;
            await ReloadDataAsync();
        }

        private async void OnTimerElapsed(object? state)
        {
            await InvokeAsync(async () =>
            {
                await OnSearch(searchString);
            });

            _timer?.Dispose();
        }

        private void SearchChanged()
        {
            _timer?.Dispose();
            _timer = new Timer(OnTimerElapsed, null, 500, 0);
        }

        private void ShowVariant(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, c => c.SnackbarVariant = Variant.Text);
        }

        private void ShowSnackbar(string message, Severity severity)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 10;
            Snackbar.Add(message, severity, options => options.SnackbarVariant = Variant.Text);
        }

        private async Task OpenRemoveReviewDialogAsync(Guid id)
        {
            var parameter = new DialogParameters { { "Id", id } };
            var dialog = await DialogService!.ShowAsync<ConfirmDeleteReviewDialog>("Delete Confirmation", parameter);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await ReloadDataAsync();
            }
        }

        private async Task OpenRemoveReplyDialogAsync(Guid id)
        {
            var parameter = new DialogParameters();
            parameter.Add("Id", id);
            var dialog = await _dialogService.ShowAsync<ConfirmDeleteReplyDialog>("Delete Confirmation", parameter);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await ReloadDataAsync();
            }
        }
    }
}
