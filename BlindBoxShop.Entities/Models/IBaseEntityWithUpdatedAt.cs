namespace BlindBoxShop.Entities.Models
{
    public interface IBaseEntityWithUpdatedAt
    {
        public DateTime? UpdatedAt { get; set; }
    }

}
