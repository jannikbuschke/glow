using System;
using Glow.Authentication.Aad;
using Glow.Azure.AzureKeyVault;
using Glow.Core.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Glow.MsGraph.Mails;

namespace Glow.Core
{
    public static class ServiceExtensions
    {
        [Obsolete]
        public static IServiceCollection AddMailService(this IServiceCollection services)
        {
            services.AddScoped<MailService>();
            services.AddAzureKeyvaultClientProvider();
            return services;
        }

        public static IServiceCollection AddGlowAadIntegration(
            this IServiceCollection services
        )
        {
            services.AddScoped<IGraphTokenService, GraphTokenService>();

            services.AddSingleton<TokenService>();

            services.AddMemoryCache();

            return services;
        }
    }
}
