using Glow.Sample.Files;
using Microsoft.EntityFrameworkCore;

namespace Glow.Sample
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<PortfolioFile> PortfolioFiles { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<MdxBundle.Entity> MdxBundle { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Portfolio>()
                .Property(v => v.RowVersion)
                .IsRowVersion();
        }
    }
}
