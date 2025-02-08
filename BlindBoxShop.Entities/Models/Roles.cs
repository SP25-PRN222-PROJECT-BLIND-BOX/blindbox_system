using Microsoft.AspNetCore.Identity;

namespace BlindBoxShop.Entities.Models
{
    public class Roles : IdentityRole<Guid>
    {
        public virtual ICollection<User>? Users { get; set; }
    }
}
