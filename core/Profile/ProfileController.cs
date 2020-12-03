using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glow.Authentication.Aad;
using Glow.Core.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace Glow.Core.Profiles
{
    public class Profile
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string IdentityName { get; set; }
        public bool IsAuthenticated { get; set; }

        public string ObjectId { get; set; }
        public string UserId { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Claims { get; set; }
    }

    public class HasConsented
    {
        public bool Value { get; set; }
    }

    [Route("glow/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> logger;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
        private readonly IGraphTokenService graphTokenService;

        public ProfileController(
            ILogger<ProfileController> logger,
            IWebHostEnvironment env,
            IConfiguration configuration,
            IGraphTokenService graphTokenService
        )
        {
            this.logger = logger;
            this.env = env;
            this.configuration = configuration;
            this.graphTokenService = graphTokenService;
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public async Task<ActionResult<Profile>> Get()
        {
            var mockExternalSystems = env.IsDevelopment() && configuration.MockExternalSystems();
            var isAuthenticated = User?.Identity.IsAuthenticated ?? false;
            IEnumerable<KeyValuePair<string, string>> claims = env.IsDevelopment() ? User.Claims.Select(v => new KeyValuePair<string, string>(v.Type, v.Value)) : null;

            IEnumerable<string> scopes = mockExternalSystems ? new List<string>() : (await graphTokenService.TokenForCurrentUser(new[] { "profile" })).Scopes;

            return new Profile
            {
                DisplayName = User.Name(),
                Email = User.Email(),
                Id = User.NameIdentifier(),
                IdentityName = User?.Identity.Name,
                IsAuthenticated = isAuthenticated,

                Scopes = scopes,
                ObjectId = User.GetObjectId(),
                UserId = User.GetObjectId(),
                Claims = claims
            };
        }

        [HttpGet("scopes")]
        public async Task<IEnumerable<string>> GetScopes()
        {
            AuthenticationResult token = await graphTokenService.TokenForCurrentUser(new[] { "profile" });
            return token.Scopes;
        }

        [HttpGet("claims")]
        [Authorize]
        public object Claims()
        {
            var claims = User.Claims.Select(c => new { c.Value, c.Type }).ToList();
            logger.LogInformation("Claims {@claims}", claims);

            return claims;
        }

        [Authorize]
        [AllowAnonymous]
        [HttpGet("has-consent")]
        public async Task<HasConsented> HasConsent(string scope)
        {
            if (!User.Identity.IsAuthenticated) { return new HasConsented { Value = false }; }

            if (configuration.MockExternalSystems())
            {
                return new HasConsented { Value = true };
            }

            try
            {
                AuthenticationResult result = await graphTokenService.TokenForCurrentUser(new string[] { scope });
                return new HasConsented { Value = result.Scopes.Contains(scope.ToLower()) };
            }
            catch (System.ArgumentNullException)
            {
                return new HasConsented { Value = false };
            }
            catch (MsalUiRequiredException e)
            {
                if (e.ErrorCode == "invalid_grant")
                {
                    return new HasConsented { Value = false };
                }
                throw e;
            }
        }
    }

    [Route("graph/profile")]
    [Authorize]
    public class GraphProfileController
    {
        private readonly IGraphTokenService graphTokenService;

        public GraphProfileController(IGraphTokenService graphTokenService)
        {
            this.graphTokenService = graphTokenService;
        }

        [HttpGet]
        public async Task<Microsoft.Graph.User> Me()
        {
            Microsoft.Graph.GraphServiceClient client = await graphTokenService.GetClientForUser(new string[] { "profile" });
            Microsoft.Graph.User me = await client.Me.Request().GetAsync();
            return me;
        }
    }
}
