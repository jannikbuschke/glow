using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Glow.Core.StringExtensions;
using Glow.Glue.AspNetCore;
using Microsoft.Extensions.Configuration;

namespace Glow.Azure.AzureKeyVault
{
    /// <summary>
    // Provides an api client for Azure keyVault
    /// </summary>
    public class AzureKeyvaultClientProvider
    {
        private readonly IConfiguration configuration;

        public AzureKeyvaultClientProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public SecretClient GetSecretClient()
        {
            var kvUri = configuration.GetKeyvaultDns();
            if (kvUri.IsNullOrEmpty())
            {
                throw new BadRequestException("No keyvault configured");
            }

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            return client;
        }
    }
}