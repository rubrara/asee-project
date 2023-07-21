using Microsoft.EntityFrameworkCore;
using PFMdotnet.Database.Configurations;
using PFMdotnet.Database.Entities;
using System.Reflection;

namespace PFMdotnet.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }

        public AppDbContext(DbContextOptions options) : base(options) 
        {

        }

        public AppDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            modelBuilder.ApplyConfiguration(new TransactionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
