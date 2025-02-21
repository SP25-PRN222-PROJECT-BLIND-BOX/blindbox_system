using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Features;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Repository.Contract
{
    public interface IVoucherRepository : IRepositoryBase<Voucher>
    {
        Task CreateVoucherAsync(Voucher voucher);
        void DeleteVoucher(Voucher voucher);
        Task<Voucher?> GetVoucherAsync(Guid id, bool trackChanges);
        Task<PagedList<Voucher>> GetVouchersAsync(VoucherParameter voucherParameter, bool trackChanges);
    }
}
