using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class VoucherService : BaseService, IVoucherService
    {
        public VoucherService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
        }
    }
}
