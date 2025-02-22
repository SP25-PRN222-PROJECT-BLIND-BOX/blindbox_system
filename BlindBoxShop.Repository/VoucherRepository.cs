using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Repository.Extensions;
using BlindBoxShop.Shared.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlindBoxShop.Repository
{
    public class VoucherRepository : RepositoryBase<Voucher>, IVoucherRepository
    {
        public VoucherRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<PagedList<Voucher>> GetVouchersAsync(VoucherParameter voucherParameter, bool trackChanges)
        {
            var vouchers = await FindAll(trackChanges)
                    .SearchById(voucherParameter.SearchById)
                    .Sort(voucherParameter.OrderBy)
                    .Skip((voucherParameter.PageNumber - 1) * voucherParameter.PageSize)
                    .Take(voucherParameter.PageSize)
                    .ToListAsync();

            var count = await FindAll(trackChanges)
                .SearchById(voucherParameter.SearchById)
                .CountAsync();


            return new PagedList<Voucher>(
                vouchers,
                count,
                voucherParameter.PageNumber,
                voucherParameter.PageSize);
        }
    }
}
