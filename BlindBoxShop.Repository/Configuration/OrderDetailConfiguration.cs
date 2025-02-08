using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    internal sealed class OrderDetailConfiguration : ConfigurationBase<OrderDetail>
    {
        protected override void ModelCreating(EntityTypeBuilder<OrderDetail> entity)
        {
            base.ModelCreating(entity);

            entity.HasIndex(e => new { e.OrderId, e.BlindBoxPriceHistoryId }).IsUnique();

            entity.HasIndex(e => e.OrderId);
        }
    }
}
