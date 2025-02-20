using Microsoft.AspNetCore.Components;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxCategoryPage.Partials
{
    public partial class SortBar
    {
        [Parameter]
        public EventCallback<string> OnSortChanged { get; set; }

        private async Task ApplySort(string args)
        {
            if (args.Equals("-1"))
                return;

            await OnSortChanged.InvokeAsync(args);
        }
    }
}