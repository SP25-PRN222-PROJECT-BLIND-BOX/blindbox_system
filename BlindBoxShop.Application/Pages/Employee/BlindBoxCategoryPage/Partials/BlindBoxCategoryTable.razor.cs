using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using Microsoft.AspNetCore.Components;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxCategoryPage.Partials
{
    public partial class BlindBoxCategoryTable
    {
        [Parameter]
        public IEnumerable<BlindBoxCategoryDto>? BlindBoxCategories { get; set; }
    }
}