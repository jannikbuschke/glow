using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core.EfTicketStore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEfTicketStore(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<SqlServerTicketStoreDbContext>(options);
            services.AddScoped<ITicketStoreDbContext, SqlServerTicketStoreDbContext>();
            services.AddSingleton<ITicketStore, EfTicketStore>();
        }
    }
}
