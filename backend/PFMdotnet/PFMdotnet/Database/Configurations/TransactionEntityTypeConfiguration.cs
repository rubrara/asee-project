using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFMdotnet.Database.Entities;

namespace PFMdotnet.Database.Configurations
{
    public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<TransactionEntity>
    {

        public TransactionEntityTypeConfiguration()
        {
            
        }

        public void Configure(EntityTypeBuilder<TransactionEntity> builder)
        {
            builder.ToTable("transactions");

            // PK
            builder.HasKey(x => x.Id);

            // Definiotion of columns
            builder.Property(x => x.Id).IsRequired().HasMaxLength(64);
            builder.Property(x => x.BeneficiaryName).HasMaxLength(128);
            builder.Property(x => x.Date);
            builder.Property(x => x.Direction).HasConversion<string>().IsRequired();
            builder.Property(x => x.Amount);
            builder.Property(x => x.Description).HasMaxLength(256);
            builder.Property(x => x.Currency).HasConversion<string>().IsRequired();
            builder.Property(x => x.Mcc);
            builder.Property(x => x.Kind).HasConversion<string>().IsRequired();
        }
    }
}
