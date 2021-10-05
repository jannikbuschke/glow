using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Security.KeyVault.Secrets;
using Glow.Core.Actions;
using Glow.Core.AzureKeyVault;
using Glow.Core.StringExtensions;
using Glow.Glue.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Glow.Core.OpenIdConnect
{
    [Action(Route = "api/glow/set-openid-connect-options", AllowAnoymous = true)]
    public class SetOpenIdConnectOptions : IRequest
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class SetOpenIdConnectOptionsHandler : IRequestHandler<SetOpenIdConnectOptions, Unit>
    {
        private readonly AzureKeyvaultClientProvider azureKeyvaultClientProvider;
        private readonly IConfiguration configuration;
        private readonly ILogger<SetOpenIdConnectOptionsHandler> logger;

        private const string TenantId = "OpenIdConnect--TenantId";
        private const string ClientId = "OpenIdConnect--ClientId";
        private const string ClientSecret = "OpenIdConnect--ClientSecret";

        public SetOpenIdConnectOptionsHandler(AzureKeyvaultClientProvider azureKeyvaultClientProvider, IConfiguration configuration,
            ILogger<SetOpenIdConnectOptionsHandler> logger)
        {
            this.azureKeyvaultClientProvider = azureKeyvaultClientProvider;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<Unit> Handle(SetOpenIdConnectOptions request, CancellationToken cancellationToken)
        {
            SecretClient client = azureKeyvaultClientProvider.GetSecretClient();
            if (configuration.GetValue<bool>("AllowConfiguration") == true)
            {
                try
                {
                    Response<KeyVaultSecret> currentTenantId = await client.GetSecretAsync(TenantId);
                    Response<KeyVaultSecret> currentclientId = await client.GetSecretAsync(ClientId);
                    Response<KeyVaultSecret> currentclientSecret = await client.GetSecretAsync(ClientSecret);

                    if (string.IsNullOrEmpty(currentTenantId?.Value?.Value))
                    {
                        await client.SetSecretAsync(TenantId, request.TenantId);
                    }

                    if (string.IsNullOrEmpty(currentclientId?.Value?.Value))
                    {
                        await client.SetSecretAsync(ClientId, request.ClientId);
                    }

                    if (string.IsNullOrEmpty(currentclientSecret?.Value?.Value))
                    {
                        await client.SetSecretAsync(ClientSecret, request.ClientSecret);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Could not get or update client secrets");
                    throw new BadRequestException(e.Message);
                }
            }

            return Unit.Value;
        }
    }
}