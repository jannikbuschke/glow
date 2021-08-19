using System;
using System.Threading.Tasks;
using Glow.Core.Authentication;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Glow.Core.Authentication
{
    // glow/profile uses IGraphTokenService to get some scopes
    // in azdo apps we use this to mock it (workaround)
    public class MockedGraphTokenService : IGraphTokenService
    {
        public Task<string> AccessTokenForApp()
        {
            return Task.FromResult("");
        }

        public Task<string> AccessTokenForCurrentUser(string[] scope)
        {
            return Task.FromResult("");
        }

        public Task<string> AccessTokenForServiceUser()
        {
            return Task.FromResult("");
        }

        public Task<GraphServiceClient> GetClientForUser(string[] scopes, bool useBetaEndpoint = false)
        {
            return null;
        }

        public Task ThrowIfCurrentUserNotConsentedToScope(string scope)
        {
            return Task.CompletedTask;
        }

        public Task<AuthenticationResult> TokenForCurrentUser(string[] scope)
        {
            return Task.FromResult(
                new AuthenticationResult("", false, "", DateTimeOffset.Now, DateTimeOffset.Now, "",
                null, "", null, Guid.Empty, null));
        }
    }
}
