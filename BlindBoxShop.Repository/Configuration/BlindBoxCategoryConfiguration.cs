using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    public sealed class BlindBoxCategoryConfiguration : ConfigurationBase<BlindBoxCategory>
    {
        protected override void ModelCreating(EntityTypeBuilder<BlindBoxCategory> entity)
        {
            base.ModelCreating(entity);
            entity.HasIndex(e => e.Name).IsUnique();

        }
    }
}
