using BlindBoxShop.Entities.Models;
using BlindBoxShop.Shared.Enum;

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

            //entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

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
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.BlindBoxItems)
                .WithOne(e => e.BlindBox)
                .HasForeignKey(e => e.BlindBoxId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void SeedData(EntityTypeBuilder<BlindBox> entity)
        {
            // Anime category BlindBoxes
            entity.HasData(
                new BlindBox
                {
                    Id = Guid.Parse("7594c261-b8d9-43a0-a2ea-095214afc2a9"),
                    Name = "Naruto Uzumaki",
                    Description = "Mô hình BlindBox Naruto Uzumaki từ series anime/manga Naruto.",
                    BlindBoxCategoryId = Guid.Parse("c8c3ec17-0a76-49d0-b274-994d15848f39"), // Anime
                    PackageId = Guid.Parse("d2e1d185-02c9-479c-ae8b-0031a447389f"), // Hộp Tiêu Chuẩn
                    Rarity = BlindBoxRarity.Common,
                    Status = BlindBoxStatus.Available,
                    Probability = 15.0f,
                    TotalRatingStar = 4.5f,
                    CreatedAt = DateTime.Now
                },
                new BlindBox
                {
                    Id = Guid.Parse("8109eb24-4086-42a3-9d20-8e07a321b905"),
                    Name = "Goku Super Saiyan",
                    Description = "Mô hình BlindBox Goku trong trạng thái Super Saiyan từ Dragon Ball Z.",
                    BlindBoxCategoryId = Guid.Parse("c8c3ec17-0a76-49d0-b274-994d15848f39"), // Anime
                    PackageId = Guid.Parse("9a47fcb2-4910-4589-baea-6f8698c9ceab"), // Hộp Premium
                    Rarity = BlindBoxRarity.Rare,
                    Status = BlindBoxStatus.Available,
                    Probability = 5.0f,
                    TotalRatingStar = 5.0f,
                    CreatedAt = DateTime.Now
                },

                // Game category BlindBoxes
                new BlindBox
                {
                    Id = Guid.Parse("56792b87-5156-4959-82e3-25a12b66b267"),
                    Name = "Mario",
                    Description = "Mô hình BlindBox Mario từ series game Super Mario Bros của Nintendo.",
                    BlindBoxCategoryId = Guid.Parse("b2a7a3e8-d1a1-4f80-9c5a-09532cd8be17"), // Game
                    PackageId = Guid.Parse("d2e1d185-02c9-479c-ae8b-0031a447389f"), // Hộp Tiêu Chuẩn
                    Rarity = BlindBoxRarity.Common,
                    Status = BlindBoxStatus.Available,
                    Probability = 20.0f,
                    TotalRatingStar = 4.2f,
                    CreatedAt = DateTime.Now
                },
                new BlindBox
                {
                    Id = Guid.Parse("5d0d8d83-a8a6-410d-97b0-73bd5fb5d213"),
                    Name = "Kratos",
                    Description = "Mô hình BlindBox Kratos từ series game God of War.",
                    BlindBoxCategoryId = Guid.Parse("b2a7a3e8-d1a1-4f80-9c5a-09532cd8be17"), // Game
                    PackageId = Guid.Parse("bc37c5bb-dc22-4bd1-85b1-9c3fe13f5c40"), // Hộp Deluxe
                    Rarity = BlindBoxRarity.Uncommon,
                    Status = BlindBoxStatus.Available,
                    Probability = 10.0f,
                    TotalRatingStar = 4.8f,
                    CreatedAt = DateTime.Now
                },

                // Phim category BlindBoxes
                new BlindBox
                {
                    Id = Guid.Parse("3db50dc1-b3aa-4088-b083-d8823235120b"),
                    Name = "Iron Man",
                    Description = "Mô hình BlindBox Iron Man từ Marvel Cinematic Universe.",
                    BlindBoxCategoryId = Guid.Parse("92d3e29f-9f4f-4bd0-b8bf-d9cde105c04a"), // Phim
                    PackageId = Guid.Parse("9a47fcb2-4910-4589-baea-6f8698c9ceab"), // Hộp Premium
                    Rarity = BlindBoxRarity.Uncommon,
                    Status = BlindBoxStatus.Available,
                    Probability = 8.0f,
                    TotalRatingStar = 4.7f,
                    CreatedAt = DateTime.Now
                },
                new BlindBox
                {
                    Id = Guid.Parse("6b34d818-8e04-4d63-9c40-2aeb68a60a90"),
                    Name = "Darth Vader",
                    Description = "Mô hình BlindBox Darth Vader từ Star Wars.",
                    BlindBoxCategoryId = Guid.Parse("92d3e29f-9f4f-4bd0-b8bf-d9cde105c04a"), // Phim
                    PackageId = Guid.Parse("68f2ac54-5118-4711-9f22-97167b5b5a9a"), // Hộp Giới hạn
                    Rarity = BlindBoxRarity.Rare,
                    Status = BlindBoxStatus.Coming_Soon,
                    Probability = 3.0f,
                    TotalRatingStar = 0.0f,
                    CreatedAt = DateTime.Now
                },

                // Thú cưng category BlindBoxes
                new BlindBox
                {
                    Id = Guid.Parse("1c3f7db4-557f-46c9-8d59-25c557e2cbb2"),
                    Name = "Mèo con dễ thương",
                    Description = "Mô hình BlindBox mèo con đáng yêu với nhiều kiểu dáng khác nhau.",
                    BlindBoxCategoryId = Guid.Parse("6c9aa2b5-8cef-4621-b526-d94b08c17e46"), // Thú cưng
                    PackageId = Guid.Parse("d2e1d185-02c9-479c-ae8b-0031a447389f"), // Hộp Tiêu Chuẩn
                    Rarity = BlindBoxRarity.Common,
                    Status = BlindBoxStatus.Available,
                    Probability = 25.0f,
                    TotalRatingStar = 4.9f,
                    CreatedAt = DateTime.Now
                },
                new BlindBox
                {
                    Id = Guid.Parse("2ba2b8a7-3fcf-4dd6-a3de-51a897ba7c27"),
                    Name = "Chó Shiba Inu",
                    Description = "Mô hình BlindBox chó Shiba Inu nổi tiếng với nhiều tư thế vui nhộn.",
                    BlindBoxCategoryId = Guid.Parse("6c9aa2b5-8cef-4621-b526-d94b08c17e46"), // Thú cưng
                    PackageId = Guid.Parse("bc37c5bb-dc22-4bd1-85b1-9c3fe13f5c40"), // Hộp Deluxe
                    Rarity = BlindBoxRarity.Uncommon,
                    Status = BlindBoxStatus.Sold_Out,
                    Probability = 7.0f,
                    TotalRatingStar = 4.6f,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
