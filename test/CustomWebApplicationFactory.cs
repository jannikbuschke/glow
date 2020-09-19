using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Glow.Test
{
    public class CustomWebApplicationFactory<Startup>
      : WebApplicationFactory<Startup> where Startup : class
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
        }
    }
}
