using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    internal sealed class BlindBoxCategoryConfiguration : ConfigurationBase<BlindBoxCategory>
    {
        protected override void ModelCreating(EntityTypeBuilder<BlindBoxCategory> entity)
        {
            base.ModelCreating(entity);
            entity.Property(e => e.Description).HasColumnType("nvarchar(500)");
            entity.Property(e => e.Name).HasColumnType("nvarchar(100)");
            entity.HasIndex(e => e.Name).IsUnique();
        }

        protected override void SeedData(EntityTypeBuilder<BlindBoxCategory> entity)
        {
            entity.HasData(
                new BlindBoxCategory
                {
                    Id = Guid.Parse("c8c3ec17-0a76-49d0-b274-994d15848f39"),
                    Name = "Anime",
                    Description = "Bộ sưu tập BlindBox dựa trên các nhân vật anime nổi tiếng.",
                    CreatedAt = DateTime.Now
                },
                new BlindBoxCategory
                {
                    Id = Guid.Parse("b2a7a3e8-d1a1-4f80-9c5a-09532cd8be17"),
                    Name = "Game",
                    Description = "Các nhân vật được yêu thích từ thế giới game.",
                    CreatedAt = DateTime.Now
                },
                new BlindBoxCategory
                {
                    Id = Guid.Parse("92d3e29f-9f4f-4bd0-b8bf-d9cde105c04a"),
                    Name = "Phim",
                    Description = "Nhân vật từ các bộ phim nổi tiếng của Hollywood và thế giới.",
                    CreatedAt = DateTime.Now
                },
                new BlindBoxCategory
                {
                    Id = Guid.Parse("6c9aa2b5-8cef-4621-b526-d94b08c17e46"),
                    Name = "Thú cưng",
                    Description = "Các mô hình động vật và thú cưng đáng yêu.",
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
