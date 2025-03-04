namespace BlindBoxShop.Shared.Features
{
    public class ReviewParameter : RequestParameters
    {
        public string? SearchById { get; set; }
        public string? SearchByContent { get; set; }
        public string? SearchByUsername { get; set; }

        public ReviewParameter()
        {
            OrderBy = "CreatedAt";
        }
    }
}
