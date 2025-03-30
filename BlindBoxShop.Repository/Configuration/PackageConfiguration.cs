using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    internal class PackageConfiguration : ConfigurationBase<Package>
    {
        protected override void ModelCreating(EntityTypeBuilder<Package> entity)
        {
            base.ModelCreating(entity);
            entity.Property(e => e.Type).HasConversion(typeof(string));

            entity.HasIndex(e => e.Barcode).IsUnique();
        }

        protected override void SeedData(EntityTypeBuilder<Package> entity)
        {
            entity.HasData(
                new Package
                {
                    Id = Guid.Parse("d2e1d185-02c9-479c-ae8b-0031a447389f"),
                    Name = "Hộp Tiêu Chuẩn",
                    Barcode = "PKG-STD-001",
                    Type = PackageType.Standard,
                    TotalBlindBox = 20,
                    CurrentTotalBlindBox = 20,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
