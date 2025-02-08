using AutoMapper;
using BlindBoxShop.Repository.Contract;

namespace BlindBoxShop.Service
{
    public abstract class BaseService
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;

        protected BaseService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
    }
}
