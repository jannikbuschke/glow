using System.Security.Claims;
using System.Threading.Tasks;
using Glow.Authentication.Aad;
using JannikB.Glue.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;

namespace Glow.Core.Authentication
{
    public interface IGraphTokenService
    {
        Task<string> TokenForCurrentUser(string scope);
        Task<string> TokenForApp();
    }

    public class GraphTokenService : IGraphTokenService
    {
        private readonly IHttpContextAccessor accessor;
        private readonly TokenService tokenService;

        public GraphTokenService(IHttpContextAccessor accessor, TokenService tokenService)
        {
            this.accessor = accessor;
            this.tokenService = tokenService;
        }

        public Task<string> TokenForApp()
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> TokenForCurrentUser(string scope)
        {
            ClaimsPrincipal user = accessor.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new ForbiddenException("Not authenticated");
            }
            try
            {
                AuthenticationResult result = await tokenService.GetAccessTokenAsync(user, new[] { scope });
                return result.AccessToken;
            }
            catch (MsalUiRequiredException e)
            {
                if (e.Classification == UiRequiredExceptionClassification.ConsentRequired)
                {
                    var message = "We could not yet fullfill you request. Please first allow the app to act on your behalf, then try again.";
                    throw new MissingConsentException(message, scope);
                }
                throw e;
            }
        }
    }
}
