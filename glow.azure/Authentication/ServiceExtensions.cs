using System;
using System.Reflection;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Extensions.ExpressionMapping;
using Glow.Authentication.Aad;
using Glow.Clocks;
using Glow.Configurations;
using Glow.Core.Actions;
using Glow.Core.Authentication;
using Glow.Files;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Glow.MsGraph.Mails;
using Glow.Core.AzureKeyVault;
using Glow.Core.EfMsalTokenStore;
using Glow.Core.EfTicketStore;

namespace Glow.Core
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMailService(this IServiceCollection services)
        {
            services.AddScoped<MailService>();
            services.AddAzureKeyvaultClientProvider();
            return services;
        }

        public static IServiceCollection AddGlowAadIntegration(
            this IServiceCollection services,
            IWebHostEnvironment env,
            IConfiguration configuration
        )
        {
            services.AddScoped<IGraphTokenService, GraphTokenService>();

            services.AddSingleton<TokenService>();
            if (env.IsDevelopment())
            {
                var connectionString = configuration.ConnectionString();
                services.AddEfMsalTokenCache(options =>
                {
                    options.UseSqlServer(connectionString);
                });

                services.AddEfTicketStore(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            }
            else
            {
                services.AddSingleton<ITokenCacheProvider, TokenCacheProvider>();
                services.AddSingleton<ITicketStore, InmemoryTicketStore>();
            }

            services.AddMemoryCache();

            return services;
        }
    }
}
