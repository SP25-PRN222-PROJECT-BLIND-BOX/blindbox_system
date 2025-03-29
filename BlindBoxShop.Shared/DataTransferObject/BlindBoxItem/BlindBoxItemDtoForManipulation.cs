using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBoxItems
{
    public class BlindBoxItemDtoForManipulation
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public BlindBoxRarity Rarity { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsSecret { get; set; }
    }
}
