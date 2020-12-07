using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Glow.Core.EfMsalTokenStore
{

    public interface IMsalTokenDbContext
    {
        DbSet<MsalToken> MsalTokens { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }

    public abstract class MsalTokenDbContext : DbContext, IMsalTokenDbContext
    {
        public MsalTokenDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<MsalToken> MsalTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MsalToken>().ToTable("__MsalToken");
            base.OnModelCreating(modelBuilder);
        }
    }

    public class SqlServerMsalTokenDbContext : MsalTokenDbContext
    {
        public SqlServerMsalTokenDbContext(DbContextOptions<SqlServerMsalTokenDbContext> options) : base(options)
        {
        }
    }

    internal class SqlServerTicketStoreDbContextFactory : IDesignTimeDbContextFactory<SqlServerMsalTokenDbContext>
    {
        public SqlServerMsalTokenDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<SqlServerMsalTokenDbContext>();
            options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ticket-store;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            return new SqlServerMsalTokenDbContext(options.Options);
        }
    }
}
