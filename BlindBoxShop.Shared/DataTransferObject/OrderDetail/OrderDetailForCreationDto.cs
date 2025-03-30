using System;

namespace BlindBoxShop.Shared.DataTransferObject.OrderDetail
{
    public class OrderDetailForCreationDto
    {
        public Guid OrderId { get; set; }
        public Guid BlindBoxId { get; set; }
        public Guid? BlindBoxItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
} 