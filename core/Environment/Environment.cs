using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Glow.Core.Environment
{
    public class GlowEnvironment
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;

        public GlowEnvironment(IWebHostEnvironment env, IConfiguration configuration)
        {
            this.env = env;
            this.configuration = configuration;
        }

        public bool MockExternSystems
        {
            get
            {
                return env.IsDevelopment() && configuration.GetValue<bool>("MockExternalSystems");
            }
        }

    }
}
