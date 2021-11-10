using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Glow.Core.EfTicketStore
{
    public interface ITicketStoreDbContext
    {
        DbSet<DbAuthenticationTicket> AuthenticationTickets { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public abstract class TicketStoreDbContext : DbContext,
                                                 ITicketStoreDbContext
    {
        public TicketStoreDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DbAuthenticationTicket> AuthenticationTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbAuthenticationTicket>().ToTable("__AuthenticationTicket");
            base.OnModelCreating(modelBuilder);
        }
    }

    public class SqlServerTicketStoreDbContext : TicketStoreDbContext
    {
        public SqlServerTicketStoreDbContext(DbContextOptions<SqlServerTicketStoreDbContext> options) : base(options)
        {
        }
    }

    internal class SqlServerTicketStoreDbContextFactory : IDesignTimeDbContextFactory<SqlServerTicketStoreDbContext>
    {
        public SqlServerTicketStoreDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<SqlServerTicketStoreDbContext>();
            options.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ticket-store;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            return new SqlServerTicketStoreDbContext(options.Options);
        }
    }
}