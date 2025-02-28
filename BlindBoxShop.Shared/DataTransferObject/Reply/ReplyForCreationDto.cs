namespace BlindBoxShop.Shared.DataTransferObject.Reply
{
    public class ReplyForCreationDto
    {
        public Guid UserId { get; set; }
        public Guid CustomerReviewsId { get; set; }
        public string Reply { get; set; } = null!;
    }
}
