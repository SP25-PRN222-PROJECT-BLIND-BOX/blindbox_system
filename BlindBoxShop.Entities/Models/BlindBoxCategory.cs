namespace BlindBoxShop.Entities.Models
{
    public class BlindBoxCategory : BaseEntity, IBaseEntity
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<BlindBox>? BlindBoxes { get; set; }
    }

}
