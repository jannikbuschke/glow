using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Glow.Authentication.Aad
{
    public static partial class AzureAdAuthenticationBuilderExtensions
    {
        public class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureAdOptions azureOptions;
            private readonly TokenService tokenService;
            private readonly ILogger<ConfigureAzureOptions> logger;
            private readonly IWebHostEnvironment env;

            public ConfigureAzureOptions(
                IOptions<AzureAdOptions> options,
                TokenService tokenService,
                ILogger<ConfigureAzureOptions> logger,
                IWebHostEnvironment env
            )
            {
                azureOptions = options.Value;
                this.tokenService = tokenService;
                this.logger = logger;
                this.env = env;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = azureOptions.ClientId;
                options.Authority = $"{azureOptions.Instance}{azureOptions.TenantId}/v2.0";
                options.UseTokenLifetime = true;
                options.CallbackPath = azureOptions.CallbackPath;
                options.RequireHttpsMetadata = env.IsProduction();
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.Prompt = "select_account";

                options.Scope.Add("openid");
                if (azureOptions.DefaultScopes != null && azureOptions.DefaultScopes.Length > 0)
                {

                    foreach (var scope in azureOptions.DefaultScopes)
                    {
                        options.Scope.Add(scope);
                    }
                }

                if (azureOptions.ValidIssuers != null && azureOptions.ValidIssuers.Length > 0)
                {
                    options.TokenValidationParameters.ValidIssuers = azureOptions.ValidIssuers;
                }

                options.Events = new OpenIdConnectEvents
                {
                    OnUserInformationReceived = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users then add your logic here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        logger.LogError("OnAuthenticationFailed", context.Exception);
                        context.Response.Redirect("/Home/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        System.Security.Claims.ClaimsPrincipal user = context.HttpContext.User;

                        context.ProtocolMessage.LoginHint = user.GetLoginHint();
                        context.ProtocolMessage.DomainHint = user.GetDomainHint();

                        tokenService.RemoveAccount(user);
                        return Task.FromResult(true);
                    },
                    OnRedirectToIdentityProvider = context =>
                    {
                        System.Collections.Generic.IDictionary<string, object> properties = context.Properties.Parameters;
                        if (properties.ContainsKey("scopes"))
                        {
                            var scopes = properties["scopes"] as string[];
                            if (scopes.Length > 0)
                            {
                                var encoded = string.Join(" ", scopes);
                                context.ProtocolMessage.Scope = context.ProtocolMessage.Scope + " " + encoded;
                            }
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = async (context) =>
                    {
                        // As AcquireTokenByAuthorizationCode is asynchronous we want to tell ASP.NET core
                        // that we are handing the code even if it's not done yet, so that it does 
                        // not concurrently call the Token endpoint.
                        context.HandleCodeRedemption();

                        var code = context.ProtocolMessage.Code;

                        Microsoft.Identity.Client.AuthenticationResult result = await tokenService.GetAccessTokenByAuthorizationCodeAsync(context.Principal, code);

                        // Do not share the access token with ASP.NET Core otherwise ASP.NET will cache it
                        // and will not send the OAuth 2.0 request in case a further call to
                        // AcquireTokenByAuthorizationCode in the future for incremental consent 
                        // (getting a code requesting more scopes)
                        // Share the ID Token so that the identity of the user is known in the application (in 
                        // HttpContext.User)
                        context.HandleCodeRedemption(null, result.IdToken);
                    }
                };
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
