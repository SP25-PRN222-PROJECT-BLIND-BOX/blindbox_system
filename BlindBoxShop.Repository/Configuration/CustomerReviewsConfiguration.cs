using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    internal sealed class CustomerReviewsConfiguration : ConfigurationBase<CustomerReviews>
    {
        protected override void ModelCreating(EntityTypeBuilder<CustomerReviews> entity)
        {
            base.ModelCreating(entity);
            entity.HasIndex(e => new { e.UserId, e.BlindBoxId }).IsUnique();

            entity.HasIndex(e => e.UserId);

            entity.HasIndex(e => e.BlindBoxId);
        }
    }
}
