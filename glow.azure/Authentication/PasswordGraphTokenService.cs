using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Glow.Core.Authentication;

extern alias GraphBeta;

public class PasswordGraphTokenService : IGraphTokenService
{
    private readonly string upn;
    private readonly string password;
    private readonly string clientId;
    private readonly string clientSecret;
    private readonly string tenantId;
    private readonly IHttpClientFactory clientFactory;
    private readonly ILogger<PasswordGraphTokenService> logger;

    private static readonly System.Collections.Generic.Dictionary<string, AuthenticationResult> cache = new();

    public PasswordGraphTokenService(
        string upn, string password,
        string clientId, string clientSecret, string tenantId,
        IHttpClientFactory clientFactory,
        ILogger<PasswordGraphTokenService> logger
    )
    {
        this.upn = upn;
        this.password = password;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
        this.tenantId = tenantId;
        this.clientFactory = clientFactory;
        this.logger = logger;
    }

    public override Task<string> AccessTokenForApp()
    {
        throw new NotSupportedException();
    }

    public override async Task<string> AccessTokenForCurrentUser(string[] scope)
    {
        AuthenticationResult authResult = await TokenForCurrentUser(scope);
        return authResult.AccessToken;
    }

    public override Task<string> AccessTokenForServiceUser()
    {
        throw new NotSupportedException();
    }

    public override async Task<GraphServiceClient> GetClientForUser(string[] scopes, bool useBetaEndpoint = false)
    {
        var token = await AccessTokenForCurrentUser(scopes);
        return CreateClient(token, useBetaEndpoint);
    }

    public override async Task<GraphBeta::Microsoft.Graph.GraphServiceClient> GetBetaClientForUser(string[] scopes)
    {
        var token = await AccessTokenForCurrentUser(scopes);
        return CreateBetaClient(token);
    }

    public override Task ThrowIfCurrentUserNotConsentedToScope(string scope)
    {
        return Task.CompletedTask;
    }

    public override async Task<AuthenticationResult> TokenForCurrentUser(string[] scope)
    {
        var isExpired = cache.ContainsKey(upn) && cache[upn].ExpiresOn < DateTimeOffset.UtcNow.AddMinutes(60);
        if (!cache.ContainsKey(upn) || isExpired)
        {
            if (isExpired)
            {
                logger.LogInformation("Token expired, refresh");
            }

            try
            {
                logger.LogInformation("User {Upn} not yet cached", upn);

                HttpClient client = clientFactory.CreateClient();
                PasswordFlow.AccessTokenResponse result = await client.GetMsAccessTokentWithUsernamePassword(
                    clientSecret,
                    upn,
                    password,
                    clientId,
                    tenantId);

                cache[upn] = new AuthenticationResult(
                    accessToken: result.access_token,
                    isExtendedLifeTimeToken: false,
                    uniqueId: Guid.NewGuid().ToString(),
                    expiresOn: DateTimeOffset.UtcNow.AddSeconds(result.expires_in),
                    extendedExpiresOn: DateTimeOffset.UtcNow.AddSeconds(result.ext_expires_in),
                    tenantId: tenantId,
                    account: null,
                    idToken: null,
                    scopes: scope,
                    correlationId: Guid.NewGuid());
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not authenticate '{Upn}'",upn);
                return null;
            }
        }

        return cache[upn];
    }
}
