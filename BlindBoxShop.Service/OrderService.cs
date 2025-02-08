using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class OrderService : BaseService, IOrderService
    {
        public OrderService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
        }
    }
}
