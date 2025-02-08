using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class BlindBoxPriceHistoryService : BaseService, IBlindBoxPriceHistoryService
    {
        public BlindBoxPriceHistoryService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
        }
    }
}
