using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    internal sealed class BlindBoxPriceHistoryConfiguration : ConfigurationBase<BlindBoxPriceHistory>
    {
        protected override void ModelCreating(EntityTypeBuilder<BlindBoxPriceHistory> entity)
        {
            base.ModelCreating(entity);
            entity.Property(e => e.DefaultPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        }
    }
}
