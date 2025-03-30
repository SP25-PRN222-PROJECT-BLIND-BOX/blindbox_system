using BlindBoxShop.Shared.DataTransferObject.BlindBoxItems;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBox
{
    public class BlindBoxForCreate : BlindBoxForManipulation
    {
        // Additional create-specific properties can be added here
        public IEnumerable<BlindBoxItemDtoForCreation> BlindBoxItems { get; set; }
    }
}