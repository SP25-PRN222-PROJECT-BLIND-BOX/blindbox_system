namespace BlindBoxShop.Shared.DataTransferObject.Package
{
    public class PackageManageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Barcode { get; set; } = null!;

        public int TotalBlindBox { get; set; }

        public int CurrentTotalBlindBox { get; set; }

        public BlindBoxShop.Shared.Enum.PackageType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
