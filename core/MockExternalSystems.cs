using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Glow.Core
{
    public class MockExternalSystems
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;

        public MockExternalSystems(IWebHostEnvironment env, IConfiguration configuration)
        {
            this.env = env;
            this.configuration = configuration;
        }

        public bool MockExternSystems(bool allowInNonDevelopmentEnvironment = false)
        {
            return (allowInNonDevelopmentEnvironment || env.IsDevelopment()) && configuration.GetValue<bool>("MockExternalSystems");
        }
    }
}
