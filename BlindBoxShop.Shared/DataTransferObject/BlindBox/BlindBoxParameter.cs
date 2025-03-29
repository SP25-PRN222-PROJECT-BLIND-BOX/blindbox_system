using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBox
{
    public class BlindBoxParameter : RequestParameters
    {
        public string? SearchByName { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? PackageId { get; set; }
        public int? Rarity { get; set; }
        public int? Status { get; set; }
        public bool BestSeller { get; set; }


        public BlindBoxParameter()
        {
            OrderBy = "CreatedAt desc";
        }
    }
}