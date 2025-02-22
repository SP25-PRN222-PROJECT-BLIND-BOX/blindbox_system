using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class BlindBoxService : BaseService, IBlindBoxService
    {
        private readonly IBlindBoxRepository _blindBoxRepository;
        public BlindBoxService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _blindBoxRepository = repositoryManager.BlindBox;
        }

        public void Dispose()
        {
            _blindBoxRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
