using BlindBoxShop.Shared.DataTransferObject.Order;
using BlindBoxShop.Shared.DataTransferObject.Roles;

namespace BlindBoxShop.Shared.DataTransferObject.Users
{
    public class UserDto
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
    }

    public class UserDtoWithRelation : UserDto
    {
        public virtual ICollection<RolesDto>? Roles { get; set; }

        public virtual ICollection<OrderDto>? Orders { get; set; }

        //public virtual ICollection<CustomerReviewsDto>? CustomerReviews { get; set; }

        //public virtual ICollection<ReplyReviews>? ReplyReviews { get; set; }
    }
}
