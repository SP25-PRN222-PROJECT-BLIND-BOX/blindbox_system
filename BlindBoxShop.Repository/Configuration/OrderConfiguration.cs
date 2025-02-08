using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    internal sealed class OrderConfiguration : ConfigurationBase<Order>
    {
        protected override void ModelCreating(EntityTypeBuilder<Order> entity)
        {
            base.ModelCreating(entity);
            entity.HasIndex(e => new { e.VoucherId, e.UserId }).IsUnique();

            entity.HasIndex(e => e.UserId);

            entity.Property(e => e.PaymentMethod).HasConversion(typeof(string));

            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Voucher)
                .WithMany(e => e.Orders)
                .HasForeignKey(e => e.VoucherId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
