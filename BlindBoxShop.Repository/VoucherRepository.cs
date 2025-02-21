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

        // Create a voucher
        public async Task CreateVoucherAsync(Voucher voucher)
        {
            await CreateAsync(voucher);
        }

        // Delete a voucher
        public void DeleteVoucher(Voucher voucher)
        {
            Delete(voucher);
        }

        // Get a single voucher by ID
        public async Task<Voucher?> GetVoucherAsync(Guid id, bool trackChanges)
        {
            return await FindByCondition(v => v.Id == id, trackChanges).SingleOrDefaultAsync();
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
