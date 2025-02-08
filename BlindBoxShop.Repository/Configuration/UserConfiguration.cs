using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BlindBoxShop.Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace BlindBoxShop.Repository.Configuration
{

    internal sealed class UserConfiguration : ConfigurationBase<User>
    {
        protected override void ModelCreating(EntityTypeBuilder<User> entity)
        {
            entity.HasMany(u => u.Roles)
                    .WithMany(r => r.Users)
                    .UsingEntity<IdentityUserRole<Guid>>(
                        j => j.HasOne<Roles>().WithMany().HasForeignKey(ur => ur.RoleId),
                        j => j.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId),
                        j =>
                        {
                            j.HasKey(ur => new { ur.UserId, ur.RoleId });
                            j.ToTable("UserRoles");
                        });
        }
    }
}
