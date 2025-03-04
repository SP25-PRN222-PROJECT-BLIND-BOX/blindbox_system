namespace BlindBoxShop.Shared.DataTransferObject.Reply
{
    public class ReplyDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Reply { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
