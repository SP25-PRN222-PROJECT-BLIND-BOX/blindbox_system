using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;

namespace BlindBoxShop.Service
{
    public class OrderDetailService : BaseService, IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        public OrderDetailService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _orderDetailRepository = repositoryManager.OrderDetail;
        }

        public void Dispose()
        {
            _orderDetailRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
