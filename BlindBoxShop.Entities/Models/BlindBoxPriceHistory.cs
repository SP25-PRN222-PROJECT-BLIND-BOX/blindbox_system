using System.ComponentModel.DataAnnotations.Schema;

namespace BlindBoxShop.Entities.Models
{
    public class BlindBoxPriceHistory : BaseEntity, IBaseEntity
    {
        public Guid? BlindBoxId { get; set; }

        public decimal DefaultPrice { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual BlindBox? BlindBox { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }

}
