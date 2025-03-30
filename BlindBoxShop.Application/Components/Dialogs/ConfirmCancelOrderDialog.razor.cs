using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlindBoxShop.Application.Components.Dialogs
{
    public partial class ConfirmCancelOrderDialog
    {
        [Inject]
        public IServiceManager? ServiceManager { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter] public Guid OrderId { get; set; }
        [Parameter] public string Content { get; set; }

        private async void Submit()
        {
            using var orderService = ServiceManager!.OrderService;
            await orderService.CancelOrderAsync(OrderId);
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel() => MudDialog.Cancel();
        
        /// <summary>
        /// Trả về URL hình ảnh đã định dạng đúng cho BlindBoxItem
        /// </summary>
        /// <param name="item">BlindBoxItem cần lấy hình ảnh</param>
        /// <returns>URL hình ảnh đã định dạng hoặc hình ảnh mặc định nếu không có</returns>
        private string GetImageUrl(BlindBoxItemDto? item)
        {
            if (item == null || string.IsNullOrEmpty(item.ImageUrl))
            {
                return "/images/box-placeholder.jpg";
            }
            
            // Đảm bảo URL có định dạng đúng
            var imageUrl = item.ImageUrl;
            if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
            {
                imageUrl = "/" + imageUrl;
            }
            
            return imageUrl;
        }
        
        /// <summary>
        /// Trả về URL hình ảnh cho BlindBoxItem theo ID của item
        /// </summary>
        /// <param name="blindBoxItemId">ID của BlindBoxItem cần lấy hình ảnh</param>
        /// <param name="defaultUrl">URL mặc định nếu không tìm thấy</param>
        /// <returns>URL hình ảnh đã định dạng</returns>
        private async Task<string> GetImageUrlByItemIdAsync(Guid blindBoxItemId, string defaultUrl = "/images/box-placeholder.jpg")
        {
            try
            {
                if (ServiceManager == null) return defaultUrl;
                
                using var blindBoxItemService = ServiceManager.BlindBoxItemService;
                var result = await blindBoxItemService.GetBlindBoxItemByIdAsync(blindBoxItemId, false);
                
                if (result.IsSuccess && result.Value != null && !string.IsNullOrEmpty(result.Value.ImageUrl))
                {
                    // Đảm bảo URL có định dạng đúng
                    var imageUrl = result.Value.ImageUrl;
                    if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://") && !imageUrl.StartsWith("/"))
                    {
                        imageUrl = "/" + imageUrl;
                    }
                    
                    return imageUrl;
                }
                
                return defaultUrl;
            }
            catch (Exception)
            {
                return defaultUrl;
            }
        }
    }
} 