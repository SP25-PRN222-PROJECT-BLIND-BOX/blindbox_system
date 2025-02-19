namespace BlindBoxShop.Shared.DataTransferObject.User
{
    public record BlindBoxCategoryDto
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
