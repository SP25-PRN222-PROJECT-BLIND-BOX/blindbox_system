using BlindBoxShop.Entities.Models;
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
    }
}
