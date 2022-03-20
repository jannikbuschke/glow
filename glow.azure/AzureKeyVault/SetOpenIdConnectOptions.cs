using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Security.KeyVault.Secrets;
using Glow.Core.Actions;
using Glow.Glue.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Glow.Azure.AzureKeyVault
{
    [Action(Route = "api/glow/set-openid-connect-options", AllowAnonymous = true)]
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
            if (configuration.AllowConfiguration())
            {
                try
                {
                    Pageable<SecretProperties> properties = client.GetPropertiesOfSecrets();
                    var secrets = properties.Select(v => v.Name).ToList();
                    logger.LogInformation("Secrets= {@secrets}", secrets);

                    async Task Set(string name, string value)
                    {
                        if (!secrets.Contains(name))
                        {
                            logger.LogInformation("Set " + name);
                            await client.SetSecretAsync(name, value);
                        }
                        else
                        {
                            logger.LogInformation("Skip setting " + name + " (already set)");
                        }
                    }

                    await Set(TenantId, request.TenantId);
                    await Set(ClientId, request.ClientId);
                    await Set(ClientSecret, request.ClientSecret);
                    //
                    // Response<KeyVaultSecret> currentTenantId = await client.GetSecretAsync(TenantId);
                    // Response<KeyVaultSecret> currentclientId = await client.GetSecretAsync(ClientId);
                    // Response<KeyVaultSecret> currentclientSecret = await client.GetSecretAsync(ClientSecret);
                    //
                    // if (string.IsNullOrEmpty(currentTenantId?.Value?.Value))
                    // {
                    //     logger.LogInformation("OpenIdConnect: Set tenant id");
                    //
                    //     await client.SetSecretAsync(TenantId, request.TenantId);
                    // }
                    //
                    // if (string.IsNullOrEmpty(currentclientId?.Value?.Value))
                    // {
                    //     logger.LogInformation("OpenIdConnect: Set client id");
                    //
                    //     await client.SetSecretAsync(ClientId, request.ClientId);
                    // }
                    //
                    // if (string.IsNullOrEmpty(currentclientSecret?.Value?.Value))
                    // {
                    //     logger.LogInformation("OpenIdConnect: Set client secret");
                    //
                    //     await client.SetSecretAsync(ClientSecret, request.ClientSecret);
                    // }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Could not get or update client secrets");
                    throw new BadRequestException(e.Message);
                }
            }
            else
            {
                logger.LogInformation("Skip setting openid connect values (AllowConfiguration = false)");
            }

            return Unit.Value;
        }
    }
}