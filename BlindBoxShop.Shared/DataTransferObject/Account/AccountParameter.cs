using BlindBoxShop.Shared.Features;

namespace BlindBoxShop.Shared.DataTransferObject.Account
{
    public class AccountParameter : RequestParameters
    {
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }

        public AccountParameter()
        {
            OrderBy = "CreatedAt desc";
        }
    }
} 