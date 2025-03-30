using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    internal sealed class BlindBoxImageConfiguration : ConfigurationBase<BlindBoxImage>
    {
        protected override void ModelCreating(EntityTypeBuilder<BlindBoxImage> entity)
        {
            base.ModelCreating(entity);
        }

        protected override void SeedData(EntityTypeBuilder<BlindBoxImage> entity)
        {
            entity.HasData(
                // Naruto Uzumaki - Main Image
                new BlindBoxImage
                {
                    Id = Guid.Parse("ba4e485f-6ce5-4c6d-950c-10f3c70a7b3a"),
                    BlindBoxId = Guid.Parse("7594c261-b8d9-43a0-a2ea-095214afc2a9"),
                    ImageUrl = "https://i.imgur.com/7H4fftM.png",
                    CreatedAt = DateTime.Now
                },

                // Goku Super Saiyan - Main Image
                new BlindBoxImage
                {
                    Id = Guid.Parse("c5e97f5e-c2c7-4c25-8e6e-3887d76be0e9"),
                    BlindBoxId = Guid.Parse("8109eb24-4086-42a3-9d20-8e07a321b905"),
                    ImageUrl = "https://i.imgur.com/L5xfBcB.png",
                    CreatedAt = DateTime.Now
                },

                // Mario - Main Image
                new BlindBoxImage
                {
                    Id = Guid.Parse("d8b1e04f-7e07-4253-93d1-3ab8c5cb5a7d"),
                    BlindBoxId = Guid.Parse("56792b87-5156-4959-82e3-25a12b66b267"),
                    ImageUrl = "https://i.imgur.com/CQJnDLt.png",
                    CreatedAt = DateTime.Now
                },

                // Kratos - Main Image
                new BlindBoxImage
                {
                    Id = Guid.Parse("f3f40eda-3d18-4698-b70a-b9f3d5aa9769"),
                    BlindBoxId = Guid.Parse("5d0d8d83-a8a6-410d-97b0-73bd5fb5d213"),
                    ImageUrl = "https://i.imgur.com/Mk40l4L.png",
                    CreatedAt = DateTime.Now
                },

                // Iron Man - Main Image
                new BlindBoxImage
                {
                    Id = Guid.Parse("1a0eb1ee-dce0-4f32-b5c9-e4066f89e74c"),
                    BlindBoxId = Guid.Parse("3db50dc1-b3aa-4088-b083-d8823235120b"),
                    ImageUrl = "https://i.imgur.com/X1ekcDk.png",
                    CreatedAt = DateTime.Now
                },

                // Darth Vader - Main Image
                new BlindBoxImage
                {
                    Id = Guid.Parse("93e49ce5-46a9-457f-be54-8b45e14dc6aa"),
                    BlindBoxId = Guid.Parse("6b34d818-8e04-4d63-9c40-2aeb68a60a90"),
                    ImageUrl = "https://i.imgur.com/Gr83jxH.png",
                    CreatedAt = DateTime.Now
                },

                // Mèo con dễ thương - Main Image
                new BlindBoxImage
                {
                    Id = Guid.Parse("60a7be19-36e6-47c6-9f13-2d36937ea5e5"),
                    BlindBoxId = Guid.Parse("1c3f7db4-557f-46c9-8d59-25c557e2cbb2"),
                    ImageUrl = "https://i.imgur.com/4fOQeNR.png",
                    CreatedAt = DateTime.Now
                },

                // Chó Shiba Inu - Main Image
                new BlindBoxImage
                {
                    Id = Guid.Parse("8c5ca06a-831e-4b6c-a4ca-436fd6aa4bee"),
                    BlindBoxId = Guid.Parse("2ba2b8a7-3fcf-4dd6-a3de-51a897ba7c27"),
                    ImageUrl = "https://i.imgur.com/Dt0vpyi.png",
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
} 