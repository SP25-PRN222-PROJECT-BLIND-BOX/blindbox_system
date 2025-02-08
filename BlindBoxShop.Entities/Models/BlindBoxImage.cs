namespace BlindBoxShop.Entities.Models
{
    public class BlindBoxImage : BaseEntity, IBaseEntity
    {
        public Guid BlindBoxId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public virtual BlindBox? BlindBox { get; set; }
    }

}
