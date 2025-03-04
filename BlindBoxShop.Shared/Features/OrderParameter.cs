namespace BlindBoxShop.Shared.Features
{
    public class OrderParameter : RequestParameters
    {
        public string? SearchById { get; set; }
        public string? SearchByStatus { get; set; }
        public DateTime? SearchByDate { get; set; }
        public OrderParameter()
        {
            OrderBy = "CreatedAt";
        }
    }
}
