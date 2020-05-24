using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;

namespace Glue.AzdoAuthentication
{
    public class AzdoClients
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ActiveUsersCache activeUsers;

        public AzdoClients(IHttpContextAccessor httpContextAccessor, ActiveUsersCache activeUsers)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.activeUsers = activeUsers;
        }

        public async Task<T> GetClient<T>() where T : VssHttpClientBase
        {
            ClaimsPrincipal user = httpContextAccessor.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                Claim id = user.Claims.Single(v => v.Type == ClaimTypes.NameIdentifier);
                ActiveUser activeUser = await activeUsers.Get(id.Value);
                var credentials = new VssOAuthAccessTokenCredential(activeUser.AccessToken);
                var connection = new VssConnection(new Uri("https://dev.azure.com/jannikb"), credentials);
                return connection.GetClient<T>();
            }
            else
            {
                throw new Exception("User is no authenticated");
            }
        }
    }
}
