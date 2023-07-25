using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFMdotnet.Database.Entities;

namespace PFMdotnet.Database.Configurations
{
    public class TransactionTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {

        public TransactionTypeConfiguration()
        {
            
        }

        public void Configure(EntityTypeBuilder<Transaction> builder)
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

            builder.HasOne(x => x.Category).WithMany(x => x.Transactions).HasForeignKey(x => x.CatCode);
        }
    }
}
