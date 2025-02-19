using Microsoft.AspNetCore.Components;

namespace BlindBoxShop.Application.Pages
{
    public partial class NotFoundPage
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public void NavigateToHome()
        {
            NavigationManager.NavigateTo("/");
        }
    }
}