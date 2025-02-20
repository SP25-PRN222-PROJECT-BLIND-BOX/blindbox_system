using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BlindBoxShop.Shared.Extension;
using BlindBoxShop.Shared.Features;
using Microsoft.AspNetCore.Components;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxCategoryPage
{
    public partial class BlindBoxCategory
    {
        [Inject]
        public IServiceManager ServiceManager { get; set; }

        public MetaData? MetaData { get; set; } = new MetaData();

        private BlindBoxCategoryParameter _blindBoxCategoryParameters = new BlindBoxCategoryParameter();

        public IEnumerable<BlindBoxCategoryDto> BlindBoxCategories { get; set; } = default!;


        protected override async Task OnInitializedAsync()
        {
            await GetBlindBoxCategories();
        }

        private async Task SelectedPage(int page)
        {
            _blindBoxCategoryParameters.PageNumber = page;
            await GetBlindBoxCategories();
        }

        private async Task SetPageSize(int pageSize)
        {
            _blindBoxCategoryParameters.PageSize = pageSize;
            _blindBoxCategoryParameters.PageNumber = 1;

            await GetBlindBoxCategories();
        }

        private async Task SearchChanged(string searchTerm)
        {
            _blindBoxCategoryParameters.PageNumber = 1;
            _blindBoxCategoryParameters.SearchByName = searchTerm;

            await GetBlindBoxCategories();
        }

        private async Task SortChanged(string orderBy)
        {
            _blindBoxCategoryParameters.OrderBy = orderBy;
            await GetBlindBoxCategories();
        }

        private async Task GetBlindBoxCategories()
        {
            var result = await ServiceManager.BlindBoxCategoryService.GetBlindBoxCategoriesAsync(_blindBoxCategoryParameters, false);

            if (result.IsSuccess)
            {
                BlindBoxCategories = result.GetValue<IEnumerable<BlindBoxCategoryDto>>();
                MetaData = result.Paging;
            }
        }



    }
}