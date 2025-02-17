using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    internal sealed class ReplyReviewsConfiguration : ConfigurationBase<ReplyReviews>
    {
        protected override void ModelCreating(EntityTypeBuilder<ReplyReviews> entity)
        {
            base.ModelCreating(entity);
            entity.HasIndex(e => new { e.UserId, e.CustomerReviewsId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(e => e.ReplyReviews)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.CustomerReviews)
                .WithOne(e => e.ReplyReviews)
                 .HasForeignKey<ReplyReviews>(e => e.CustomerReviewsId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
