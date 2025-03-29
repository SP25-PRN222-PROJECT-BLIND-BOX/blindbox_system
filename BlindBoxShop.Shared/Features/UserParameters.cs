using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Shared.Features
{
    public class UserParameters : RequestParameters
    {
        public UserRole? Role { get; set; }
    }
}
