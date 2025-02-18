using Microsoft.AspNetCore.Identity;

namespace BlindBoxShop.Entities.Models
{
    public class User : IdentityUser<Guid>, IBaseEntity, IBaseEntityWithUpdatedAt
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Address { get; set; }

        public string? Provinces { get; set; }

        public string? District { get; set; }

        public string? Wards { get; set; }

        public string? Image { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Roles>? Roles { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }

        public virtual ICollection<CustomerReviews>? CustomerReviews { get; set; }

        public virtual ICollection<ReplyReviews>? ReplyReviews { get; set; }

    }
}
