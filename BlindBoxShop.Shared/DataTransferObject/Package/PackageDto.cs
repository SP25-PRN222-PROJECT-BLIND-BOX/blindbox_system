namespace BlindBoxShop.Shared.DataTransferObject.Package
{
    public class PackageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public BlindBoxShop.Shared.Enum.PackageType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int TotalBlindBox { get; set; }
        
        


    }
} 