using Microsoft.AspNetCore.Components;

namespace BlindBoxShop.Application.Components
{
    public partial class PageSizeDropDown
    {
        [Parameter]
        public EventCallback<int> SelectedPageSize { get; set; }


        private async Task OnPageSizeChange(int value)
        {
            await SelectedPageSize.InvokeAsync(value);
        }

    }
}