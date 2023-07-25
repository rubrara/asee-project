using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFMdotnet.Database.Entities;

namespace PFMdotnet.Database.Configurations
{
    public class TransactionSplitTypeConfiguration : IEntityTypeConfiguration<TransactionSplit>
    {

        public TransactionSplitTypeConfiguration()
        {

        }

        public void Configure(EntityTypeBuilder<TransactionSplit> builder)
        {
            builder.ToTable("splits");

            // PK
            builder.HasKey(x => x.Id);

            // Definiotion of columns
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.CatCode).IsRequired().HasMaxLength(64);

            builder.HasOne(x => x.Transaction).WithMany(x => x.Splits).HasForeignKey(x => x.TransactionId);
        }
    }
}
