using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace JannikB.Glue.AspNetCore.Tests
{
    public class AdditionalClaims
    {
        public IEnumerable<Claim> Claims { get; set; }
    }

    public static class ServiceExtensions
    {
        public static void AddTestAuthentication(this IServiceCollection services)
        {
            AddTestAuthentication(services, "test", "test", new List<Claim>());
        }

        public static void AddTestAuthentication(
            this IServiceCollection services,
            string userId,
            string userName
        )
        {
            AddTestAuthentication(services, userId, userName, new List<Claim>());
        }

        public static void AddTestAuthentication(
          this IServiceCollection services,
          string userId,
          string userName,
          IEnumerable<Claim> additionalClaims
        )
        {
            IEnumerable<Claim> claims = additionalClaims.Concat(new[] {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
            });
            AddTestAuthentication(services, claims);
        }

        public static void AddTestAuthentication(
            this IServiceCollection services,
            IEnumerable<Claim> additionalClaims
        )
        {
            services.AddSingleton(new AdditionalClaims
            {
                Claims = additionalClaims
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestAuthentication";
                options.DefaultChallengeScheme = "TestAuthentication";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("TestAuthentication", null);
        }
    }

    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IEnumerable<Claim> claims;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            AdditionalClaims additionalClaims,
            IHttpContextAccessor httpContextAccessor
        )
        : base(options, logger, encoder, clock)
        {
            claims = additionalClaims.Claims;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            StringValues userIds = httpContextAccessor.HttpContext.Request.Headers["x-userid"];
            var userId = userIds.FirstOrDefault();
            IEnumerable<Claim> newClaims = claims.Select(v => userId != null && v.Type == ClaimTypes.NameIdentifier ? new Claim(ClaimTypes.NameIdentifier, userId) : v);

            var identity = new ClaimsIdentity(newClaims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
