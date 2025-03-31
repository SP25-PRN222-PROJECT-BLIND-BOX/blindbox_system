using BlindBoxShop.Shared.Enum;
using BlindBoxShop.Shared.Features;

using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBox
{
    public class BlindBoxParameter : RequestParameters
    {
        public string? SearchByName { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? PackageId { get; set; }
        [EnumDataType(typeof(BlindBoxRarity))]
        public BlindBoxRarity? Rarity { get; set; }
        [EnumDataType(typeof(BlindBoxStatus))]
        public BlindBoxStatus? Status { get; set; }
        public bool BestSeller { get; set; }


        public BlindBoxParameter()
        {
            OrderBy = "CreatedAt desc";
        }
    }
}