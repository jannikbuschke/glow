using System;
using EfConfigurationProvider.Core;
using Glow.TokenCache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Glue.AzdoAuthentication
{
    public enum DatabaseProvider
    {
        SqlServer = 1
    }

    public static class StartupExtension
    {
        public static AuthenticationBuilder AddAzdo(
            this AuthenticationBuilder builder,
            DatabaseProvider dbProvider,
            Action<AzdoConfig> configure,
            Action<DbContextOptionsBuilder> configureDb)
        {
            IServiceCollection services = builder.Services;

            services.AddSingleton<AzdoAuthenticationService>();
            services.AddSingleton<AzdoClients>();
            services.AddSingleton<ActiveUsersCache>();
            var config = new AzdoConfig();
            configure(config);
            services.AddSingleton(config);
            builder.AddCookie(AzdoDefaults.CookieAuthenticationScheme);

            switch (dbProvider)
            {
                case DatabaseProvider.SqlServer:
                    services.AddDbContext<SqlServerTokenDataContext>(configureDb);
                    services.AddScoped<ITokenDataContext, SqlServerTokenDataContext>();

                    services.AddDbContext<SqlServerConfigurationDataContext>(configureDb);
                    services.AddScoped<IConfigurationDataContext, SqlServerConfigurationDataContext>();

                    break;
                default:
                    throw new Exception("Unsupported Database Provider");
            }

            return builder;
        }
    }
}
