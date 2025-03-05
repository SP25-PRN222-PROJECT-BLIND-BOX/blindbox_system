using BlindBoxShop.Shared.DataTransferObject.Reply;

namespace BlindBoxShop.Shared.DataTransferObject.Review
{
    public class ReviewWithReplyDto
    {
        public ReviewDto Review { get; set; } = null!;
        public ReplyDto? Reply { get; set; }
    }
}
