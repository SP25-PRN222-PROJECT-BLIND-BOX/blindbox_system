namespace BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory
{
    public record BlindBoxCategoryDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
