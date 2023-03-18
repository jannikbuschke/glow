using System;
using Glow.Authentication.Aad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core.EfMsalTokenStore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEfMsalTokenCache(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<SqlServerMsalTokenDbContext>(options);
            services.AddScoped<IMsalTokenDbContext, SqlServerMsalTokenDbContext>();
            services.AddSingleton<ITokenCacheProvider, EfTokenCacheProvider>();
        }
    }
}