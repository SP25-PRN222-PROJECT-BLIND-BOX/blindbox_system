using BlindBoxShop.Shared.DataTransferObject.Voucher;
using Microsoft.AspNetCore.Components;

namespace BlindBoxShop.Application.Pages.Employee.VoucherPage.Partials
{
    public partial class VoucherTable
    {
        [Parameter]
        public IEnumerable<VoucherDto>? Vouchers { get; set; }

        private bool isEditModalVisible = false;
        private Guid selectedVoucherId;

        private void OpenEditModal(Guid voucherId)
        {
            selectedVoucherId = voucherId;
            isEditModalVisible = true;
        }
    }
}
