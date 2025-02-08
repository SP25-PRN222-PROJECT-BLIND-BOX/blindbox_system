using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    internal sealed class BlindBoxConfiguration : ConfigurationBase<BlindBox>
    {
        protected override void ModelCreating(EntityTypeBuilder<BlindBox> entity)
        {
            base.ModelCreating(entity);
            entity.HasIndex(e => e.PackageId);

            entity.Property(e => e.Rarity).HasConversion(typeof(string));

            entity.Property(e => e.Status).HasConversion(typeof(string));

            entity.Property(e => e.Description).HasColumnType("text");

            entity.HasOne(e => e.BlindBoxCategory)
                .WithMany(e => e.BlindBoxes)
                .HasForeignKey(e => e.BlindBoxCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Package)
                .WithMany(e => e.BlindBoxes)
                .HasForeignKey(e => e.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.CustomerReviews)
                .WithOne(e => e.BlindBox)
                .HasForeignKey(e => e.BlindBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.BlindBoxImages)
                .WithOne(e => e.BlindBox)
                .HasForeignKey(e => e.BlindBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.BlindBoxPriceHistories)
                .WithOne(e => e.BlindBox)
                .HasForeignKey(e => e.BlindBoxId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
