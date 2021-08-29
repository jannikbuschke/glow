using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TemplateName.Test
{

    public class CustomWebApplicationFactory<TEntryPoint>
      : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        static CustomWebApplicationFactory()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (!string.IsNullOrEmpty(environment))
            {
                builder.UseEnvironment(environment);
            }
            //var cs = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TemplateName-test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //var options = new DbContextOptionsBuilder { };
            //options.UseSqlServer(cs, options =>
            //{
            //    options.MigrationsAssembly("TemplateName.Core.SqlServer");
            //});

            //using var dbContext = new DataContext(options.Options);
            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.Migrate();

            //dbContext.Configurations.Add(new Configuration
            //{
            //    Created = DateTime.UtcNow,
            //    Values = new System.Collections.Generic.Dictionary<string, string>
            //    {
            //        { "key1", "value1" },
            //        { "key2", "value2" },
            //        { "key3", "value3" },
            //        { "key4", "value4" },
            //        { "key5", "value5" },
            //        { "key6", "value6" },
            //    }
            //});
            //dbContext.SaveChanges();
        }
    }
}
