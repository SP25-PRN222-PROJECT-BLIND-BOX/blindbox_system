using BlindBoxShop.Shared.DataTransferObject.OrderDetail;

namespace BlindBoxShop.Shared.DataTransferObject.Order
{
    public class OrderWithDetailsDto
    {
        public OrderDto Order { get; set; }
        public IEnumerable<OrderDetailDto> OrderDetails { get; set; }
    }
}
