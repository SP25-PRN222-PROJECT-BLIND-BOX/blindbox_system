using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class PackageService : BaseService, IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        public PackageService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _packageRepository = repositoryManager.Package;
        }

        public void Dispose()
        {
            _packageRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
