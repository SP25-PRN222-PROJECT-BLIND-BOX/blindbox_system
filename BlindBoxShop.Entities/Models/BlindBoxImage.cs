namespace BlindBoxShop.Entities.Models
{
    public class BlindBoxImage : BaseEntity
    {
        public Guid BlindBoxId { get; set; }

        public string ImageUrl { get; set; } = null!;


        public virtual BlindBox? BlindBox { get; set; }
    }

}
