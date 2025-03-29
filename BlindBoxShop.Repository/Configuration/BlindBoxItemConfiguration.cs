using BlindBoxShop.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindBoxShop.Repository.Configuration
{
    public class BlindBoxItemConfiguration : IEntityTypeConfiguration<BlindBoxItem>
    {
        public void Configure(EntityTypeBuilder<BlindBoxItem> builder)
        {
            builder.ToTable("BlindBoxItems");
            
            builder.HasKey(b => b.Id);
            
            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(b => b.Description)
                .HasMaxLength(1000);
                
            builder.Property(b => b.ImageUrl)
                .HasMaxLength(1000);
                
            builder.Property(b => b.BlindBoxId)
                .IsRequired();
                
            builder.Property(b => b.Rarity)
                .IsRequired();
                
            builder.Property(b => b.IsSecret)
                .HasDefaultValue(false);
                
            builder.HasOne(b => b.BlindBox)
                .WithMany(bb => bb.BlindBoxItems)
                .HasForeignKey(b => b.BlindBoxId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 