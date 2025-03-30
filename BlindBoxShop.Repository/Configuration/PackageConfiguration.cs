using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Enum;

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
                },
                 new Package
                 {
                     Id = Guid.Parse("9a47fcb2-4910-4589-baea-6f8698c9ceab"),
                     Name = "Hộp Premium",
                     Barcode = "PKG-STD-002",
                     Type = PackageType.Standard,
                     TotalBlindBox = 20,
                     CurrentTotalBlindBox = 20,
                     CreatedAt = DateTime.Now
                 },
                  new Package
                  {
                      Id = Guid.Parse("bc37c5bb-dc22-4bd1-85b1-9c3fe13f5c40"),
                      Name = "Hộp Deluxe",
                      Barcode = "PKG-STD-003",
                      Type = PackageType.Standard,
                      TotalBlindBox = 20,
                      CurrentTotalBlindBox = 20,
                      CreatedAt = DateTime.Now
                  },
                  new Package
                  {
                      Id = Guid.Parse("68f2ac54-5118-4711-9f22-97167b5b5a9a"),
                      Name = "Hộp Giới hạn",
                      Barcode = "PKG-STD-004",
                      Type = PackageType.Standard,
                      TotalBlindBox = 20,
                      CurrentTotalBlindBox = 20,
                      CreatedAt = DateTime.Now
                  }
            );
        }
    }
}
