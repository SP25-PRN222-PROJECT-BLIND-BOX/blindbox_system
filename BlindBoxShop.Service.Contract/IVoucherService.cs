using BlindBoxShop.Shared.DataTransferObject.Voucher;
using BlindBoxShop.Shared.Features;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service.Contract
{
    public interface IVoucherService : IDisposable
    {
        Task<Result<IEnumerable<VoucherDto>>> GetVouchersAsync(VoucherParameter voucherParameter, bool trackChanges);
        Task<Result<VoucherDto>> GetVoucherAsync(Guid id, bool trackChanges);
        Task<Result<VoucherDto>> CreateVoucherAsync(VoucherForCreate voucherForCreateDto);
        Task<Result> UpdateVoucherAsync(Guid id, VoucherForUpdate voucherForUpdateDto);
        Task<Result> DeleteVoucherAsync(Guid id);

    }
}
