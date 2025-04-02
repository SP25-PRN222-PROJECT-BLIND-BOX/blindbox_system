using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Shared.DataTransferObject.Package
{
    public class PackageParameter : RequestParameters
    {
        public string? SearchByName { get; set; }
        public int? Type { get; set; }

        public PackageParameter()
        {
            OrderBy = "CreatedAt desc";
        }
    }
}
