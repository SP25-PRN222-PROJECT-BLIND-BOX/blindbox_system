namespace BlindBoxShop.Shared.Features
{
    public class ReplyParameter : RequestParameters
    {
        public string? SearchById { get; set; }
        public string? SearchByReply { get; set; }
        public string? SearchByUsername { get; set; }

        public ReplyParameter()
        {
            OrderBy = "CreatedAt";
        }
    }
}
