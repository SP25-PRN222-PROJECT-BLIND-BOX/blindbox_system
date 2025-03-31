using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Repository.Extensions
{
    public static class UserRepositoryExtensions
    {
        public static IQueryable<User> FilterByRole(this IQueryable<User> users, UserRole? userRole)
        {
            if (userRole == null)
            {
                return users;
            }
            return users.Where(u => u.Roles != null && u.Roles.Any(r => r.Name!.Equals(userRole.ToString())));
        }
    }
}
