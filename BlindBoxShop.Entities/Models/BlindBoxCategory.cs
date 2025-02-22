namespace BlindBoxShop.Entities.Models
{
    public class BlindBoxCategory : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public virtual ICollection<BlindBox>? BlindBoxes { get; set; }
    }

}
