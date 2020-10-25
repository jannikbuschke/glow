using Glow.Sample;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit;

namespace Glow.Test
{
    [CollectionDefinition("integration-tests")]
    public class CollectionFixture : ICollectionFixture<IntegrationTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class IntegrationTestFixture
    {
        public WebApplicationFactory<Startup> Factory { get; }

        public IntegrationTestFixture()
        {
            var factory = new CustomWebApplicationFactory<Startup>();
            Log.Logger = new LoggerConfiguration()
                 //.WriteTo.TestOutput(output)
                 .WriteTo.Console()
                 .WriteTo.Seq("http://localhost:5341")
                 .Enrich.FromLogContext()
                 .CreateLogger();

            Factory = factory.WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    //UserDto testUser = TestUsers.TestUser();
                    //services.AddTestAuthentication(testUser.Id, testUser.DisplayName, testUser.Email);
                })
            );
            DataContext ctx = Factory.Services.GetRequiredService<DataContext>();
            ctx.Database.EnsureDeleted();
            ctx.Database.Migrate();
        }
    }
}
