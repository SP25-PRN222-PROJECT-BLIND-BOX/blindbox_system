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
            entity.Property(e => e.DefaultProbability).HasColumnType("decimal(5,2)");
            entity.Property(e => e.Probability).HasColumnType("decimal(5,2)");
        }

        protected override void SeedData(EntityTypeBuilder<BlindBoxPriceHistory> entity)
        {
            entity.HasData(
                // Naruto Uzumaki
                new BlindBoxPriceHistory
                {
                    Id = Guid.Parse("f211a1f3-e4c7-4aa5-b319-48b013f92e07"),
                    BlindBoxId = Guid.Parse("7594c261-b8d9-43a0-a2ea-095214afc2a9"),
                    DefaultPrice = 150000m,
                    Price = 150000m,
                    DefaultProbability = 5.00m,
                    Probability = 5.00m,
                    CreatedAt = DateTime.Now
                },

                // Goku Super Saiyan
                new BlindBoxPriceHistory
                {
                    Id = Guid.Parse("8f72c4b6-78bf-4967-b5dc-c2a45a9ac8c7"),
                    BlindBoxId = Guid.Parse("8109eb24-4086-42a3-9d20-8e07a321b905"),
                    DefaultPrice = 350000m,
                    Price = 350000m,
                    DefaultProbability = 3.00m,
                    Probability = 3.00m,
                    CreatedAt = DateTime.Now
                },

                // Mario
                new BlindBoxPriceHistory
                {
                    Id = Guid.Parse("6c09ae3a-9156-4de3-9508-4ffb2c5c196e"),
                    BlindBoxId = Guid.Parse("56792b87-5156-4959-82e3-25a12b66b267"),
                    DefaultPrice = 120000m,
                    Price = 120000m,
                    DefaultProbability = 10.00m,
                    Probability = 10.00m,
                    CreatedAt = DateTime.Now
                },

                // Kratos
                new BlindBoxPriceHistory
                {
                    Id = Guid.Parse("4b6fd56e-c089-4222-88e8-4cda5e37a853"),
                    BlindBoxId = Guid.Parse("5d0d8d83-a8a6-410d-97b0-73bd5fb5d213"),
                    DefaultPrice = 280000m,
                    Price = 280000m,
                    DefaultProbability = 2.50m,
                    Probability = 2.50m,
                    CreatedAt = DateTime.Now
                },

                // Iron Man
                new BlindBoxPriceHistory
                {
                    Id = Guid.Parse("9d781c58-cb88-4c03-ad79-ec159f4c91c6"),
                    BlindBoxId = Guid.Parse("3db50dc1-b3aa-4088-b083-d8823235120b"),
                    DefaultPrice = 320000m,
                    Price = 320000m,
                    DefaultProbability = 2.00m,
                    Probability = 2.00m,
                    CreatedAt = DateTime.Now
                },

                // Darth Vader
                new BlindBoxPriceHistory
                {
                    Id = Guid.Parse("5218b0c5-0891-477d-9ae5-d6e35e7c2c13"),
                    BlindBoxId = Guid.Parse("6b34d818-8e04-4d63-9c40-2aeb68a60a90"),
                    DefaultPrice = 500000m,
                    Price = 500000m,
                    DefaultProbability = 1.00m,
                    Probability = 1.00m,
                    CreatedAt = DateTime.Now
                },

                // Mèo con dễ thương
                new BlindBoxPriceHistory
                {
                    Id = Guid.Parse("3a0e9a5b-1b1e-4c27-8c59-267a174c7b64"),
                    BlindBoxId = Guid.Parse("1c3f7db4-557f-46c9-8d59-25c557e2cbb2"),
                    DefaultPrice = 100000m,
                    Price = 100000m,
                    DefaultProbability = 15.00m,
                    Probability = 15.00m,
                    CreatedAt = DateTime.Now
                },

                // Chó Shiba Inu
                new BlindBoxPriceHistory
                {
                    Id = Guid.Parse("c8c1a4c6-46fb-4058-b9ca-bd5c0302a276"),
                    BlindBoxId = Guid.Parse("2ba2b8a7-3fcf-4dd6-a3de-51a897ba7c27"),
                    DefaultPrice = 250000m,
                    Price = 250000m,
                    DefaultProbability = 5.00m,
                    Probability = 5.00m,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
