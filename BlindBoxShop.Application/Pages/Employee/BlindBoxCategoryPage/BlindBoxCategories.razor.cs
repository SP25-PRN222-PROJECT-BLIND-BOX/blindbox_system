using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.User;
using BlindBoxShop.Shared.Extension;
using Microsoft.AspNetCore.Components;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxCategoryPage
{
    public partial class BlindBoxCategories
    {
        [Inject]
        public IServiceManager ServiceManager { get; set; }
        public IEnumerable<BlindBoxCategoryDto> BlindBoxCategory { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var result = await ServiceManager.BlindBoxCategoryService.GetBlindBoxCategoriesAsync(new(), false);

            if (result.IsSuccess)
            {
                BlindBoxCategory = result.GetValue<IEnumerable<BlindBoxCategoryDto>>();
            }

        }
    }
}