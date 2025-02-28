namespace BlindBoxShop.Shared.Features
{
    public class CustomerReviewParameter : RequestParameters
    {
        public string? SearchById { get; set; }
        public string? SearchByContent { get; set; }
        public string? SearchByUsername { get; set; }

        public CustomerReviewParameter()
        {
            OrderBy = "CreatedAt";
        }
    }
}
