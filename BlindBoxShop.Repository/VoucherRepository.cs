using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Repository
{
    public class VoucherRepository : RepositoryBase<Voucher>, IVoucherRepository
    {
        public VoucherRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

    }
}
