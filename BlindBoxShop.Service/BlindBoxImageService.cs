using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class BlindBoxImageService : BaseService, IBlindBoxImageService
    {
        private readonly IBlindBoxImageRepository _blindBoxImageRepository;

        public BlindBoxImageService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _blindBoxImageRepository = repositoryManager.BlindBoxImage;
        }

        public void Dispose()
        {
            _blindBoxImageRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
