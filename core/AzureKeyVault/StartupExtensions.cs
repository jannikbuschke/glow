using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Glow.Core.StringExtensions;
using Glow.Glue.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Glow.Core.AzureKeyVault
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddAzureKeyvaultClientProvider(this IServiceCollection collection)
        {
            return collection.AddTransient<AzureKeyvaultClientProvider>();
        }
    }
}