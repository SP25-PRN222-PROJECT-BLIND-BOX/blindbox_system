using Blazored.Toast.Services;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace BlindBoxShop.Application.Pages.Employee.BlindBoxCategoryPage.Partials
{
    public partial class BlindBoxCategoryModalCreate : IDisposable
    {
        [Inject]
        public IServiceManager ServiceManager { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }

        private BlindBoxCategoryForCreate? _blindBoxCategoryForCreate = new BlindBoxCategoryForCreate();

        [NotNull]
        private Modal? DragModal { get; set; }

        private async Task Create()
        {
            var result = await ServiceManager.BlindBoxCategoryService.CreateBlindBoxCategoryAsync(_blindBoxCategoryForCreate!);

            if (result.IsSuccess)
            {
                ToastService.ShowSuccess($"Action successful. Category \"{_blindBoxCategoryForCreate.Name}\" successfully addd.");

                await DragModal.Toggle();
                _blindBoxCategoryForCreate = new();
            }
        }

        public void Dispose()
        {
        }
    }
}