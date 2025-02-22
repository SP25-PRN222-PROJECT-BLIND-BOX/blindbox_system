using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Features;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Repository.Contract
{
    public interface IVoucherRepository : IRepositoryBase<Voucher>
    {
        Task<PagedList<Voucher>> GetVouchersAsync(VoucherParameter voucherParameter, bool trackChanges);
    }
}
