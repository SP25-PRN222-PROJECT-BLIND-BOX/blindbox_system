using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BlindBoxShop.Repository.Configuration
{
    public abstract class ConfigurationBase<T> : IEntityTypeConfiguration<T> where T : class
    {
        public void Configure(EntityTypeBuilder<T> entity)
        {
            SeedData(entity);
            ModelCreating(entity);
        }

        protected virtual void SeedData(EntityTypeBuilder<T> entity) { }

        protected virtual void ModelCreating(EntityTypeBuilder<T> entity) { }
    }
}
