using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Security.KeyVault.Secrets;
using Glow.Core.Actions;
using Glow.Glue.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public string Instance { get; set; }
    }

    public class SetOpenIdConnectOptionsHandler : IRequestHandler<SetOpenIdConnectOptions, Unit>
    {
        private readonly AzureKeyvaultClientProvider azureKeyvaultClientProvider;
        private readonly IConfiguration configuration;
        private readonly ILogger<SetOpenIdConnectOptionsHandler> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        private const string TenantId = "OpenIdConnect--TenantId";
        private const string ClientId = "OpenIdConnect--ClientId";
        private const string ClientSecret = "OpenIdConnect--ClientSecret";
        private const string Instance = "OpenIdConnect--Instance";

        public SetOpenIdConnectOptionsHandler(
            AzureKeyvaultClientProvider azureKeyvaultClientProvider, IConfiguration configuration,
            ILogger<SetOpenIdConnectOptionsHandler> logger,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            this.azureKeyvaultClientProvider = azureKeyvaultClientProvider;
            this.configuration = configuration;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(SetOpenIdConnectOptions request, CancellationToken cancellationToken)
        {
            SecretClient client = azureKeyvaultClientProvider.GetSecretClient();
            if (configuration.AllowConfiguration())
            {
                try
                {
                    // Pageable<SecretProperties> properties = client.GetPropertiesOfSecrets();
                    // var secrets = properties.Select(v => v.Name).ToList();

                    async Task Set(string name, string value)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            logger.LogInformation("Set " + name);
                            await client.SetSecretAsync(name, value);
                        }
                        else
                        {
                            logger.LogInformation($"Skip setting '{name}' (no value provided)");
                        }
                    }

                    await Set(TenantId, request.TenantId);
                    await Set(ClientId, request.ClientId);
                    await Set(ClientSecret, request.ClientSecret);
                    await Set(Instance, request.Instance);
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
                throw new BadRequestException("Not possible, editing is not enabled");
            }

            return Unit.Value;
        }
    }
}