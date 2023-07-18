using Microsoft.EntityFrameworkCore;
using PFMdotnet.Database.Configurations;
using PFMdotnet.Database.Entities;
using System.Reflection;

namespace PFMdotnet.Database
{
    public class TransactionDbContext : DbContext
    {
        public DbSet<TransactionEntity> Transactions { get; set; }

        public TransactionDbContext(DbContextOptions options) : base(options) 
        {

        }

        public TransactionDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            modelBuilder.ApplyConfiguration(
                new TransactionEntityTypeConfiguration()
                );
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
