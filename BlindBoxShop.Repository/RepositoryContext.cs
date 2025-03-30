using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Configuration;
using BlindBoxShop.Shared.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Emit;

namespace BlindBoxShop.Repository
{
    public class RepositoryContext : IdentityDbContext<User, Roles, Guid>
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName!.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new BlindBoxCategoryConfiguration());
            builder.ApplyConfiguration(new BlindBoxConfiguration());
            builder.ApplyConfiguration(new BlindBoxImageConfiguration());
            builder.ApplyConfiguration(new BlindBoxPriceHistoryConfiguration());
            builder.ApplyConfiguration(new BlindBoxItemConfiguration());
            builder.ApplyConfiguration(new CustomerReviewsConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new OrderDetailConfiguration());
            builder.ApplyConfiguration(new PackageConfiguration());
            builder.ApplyConfiguration(new ReplyReviewsConfiguration());
        }

        public virtual DbSet<BlindBox> BlindBoxes { get; set; }
        public virtual DbSet<BlindBoxCategory> BlindBoxCategories { get; set; }
        public virtual DbSet<BlindBoxImage> BlindBoxImages { get; set; }
        public virtual DbSet<BlindBoxPriceHistory> BlindBoxPriceHistories { get; set; }
        public virtual DbSet<BlindBoxItem> BlindBoxItems { get; set; }
        public virtual DbSet<CustomerReviews> CustomerReviews { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<ReplyReviews> ReplyReviews { get; set; }


        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    HandleTimestampsForAddedEntities(entry);
                }

                if (entry.State == EntityState.Modified)
                {
                    HandleTimestampsForModifiedEntities(entry);
                }
            }

            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    HandleTimestampsForAddedEntities(entry);
                }

                if (entry.State == EntityState.Modified)
                {
                    HandleTimestampsForModifiedEntities(entry);
                }
            }

            return await base.SaveChangesAsync();
        }

        private void HandleTimestampsForAddedEntities(EntityEntry entry)
        {
            if (entry.Entity is IBaseEntity baseEntity)
            {
                baseEntity.CreatedAt = DateTime.UtcNow.SEAsiaStandardTime();
            }

            if (entry.Entity is IBaseEntityWithUpdatedAt baseEntityWithUpdatedAt)
            {
                baseEntityWithUpdatedAt.UpdatedAt = DateTime.UtcNow.SEAsiaStandardTime();
            }
        }

        private void HandleTimestampsForModifiedEntities(EntityEntry entry)
        {
            if (entry.Entity is IBaseEntityWithUpdatedAt baseEntityWithUpdatedAt)
            {
                baseEntityWithUpdatedAt.UpdatedAt = DateTime.UtcNow.SEAsiaStandardTime();
            }
        }
    }

}
