using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class BlindBoxPriceHistoryService : BaseService, IBlindBoxPriceHistoryService
    {
        private readonly IBlindBoxPriceHistoryRepository _blindBoxPriceHistoryRepository;
        public BlindBoxPriceHistoryService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _blindBoxPriceHistoryRepository = repositoryManager.BlindBoxPriceHistory;
        }

        public void Dispose()
        {
            _blindBoxPriceHistoryRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
