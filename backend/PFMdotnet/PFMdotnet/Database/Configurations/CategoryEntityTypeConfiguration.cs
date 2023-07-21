using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFMdotnet.Database.Entities;

namespace PFMdotnet.Database.Configurations
{
    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<CategoryEntity>
    {

        public CategoryEntityTypeConfiguration()
        {
            
        }

        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.ToTable("categories");

            // PK
            builder.HasKey(x => x.Code);

            // Definiotion of columns
            builder.Property(x => x.Code).IsRequired().HasMaxLength(64);
            builder.Property(x => x.ParentCode).IsRequired(false).HasMaxLength(64);
            builder.Property(x => x.Name).HasMaxLength(128);
        }
    }
}
