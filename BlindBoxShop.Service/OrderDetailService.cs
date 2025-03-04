using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using BlindBoxShop.Shared.DataTransferObject.OrderDetail;
using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Service
{
    public class OrderDetailService : BaseService, IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper)
        {
            _orderDetailRepository = repositoryManager.OrderDetail;
        }

        public async Task<Result<IEnumerable<OrderDetailDto>>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges)
        {
            var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId, trackChanges);

            var orderDetailsDto = _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails);

            return Result<IEnumerable<OrderDetailDto>>.Success(orderDetailsDto);
        }

        public void Dispose()
        {
            _orderDetailRepository.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
