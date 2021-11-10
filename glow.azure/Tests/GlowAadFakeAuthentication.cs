using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Glow.Authentication.Aad;
using Glow.Core.Actions;
using Glow.Core.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Glow.TestAutomation
{
    public class FakeUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class FakeUsers
    {
        public IEnumerable<FakeUser> Values { get; set; }
    }

    [Action(Route = "api/glow/test-automation/get-available-fake-users", Policy = DefaultPolicies.AuthenticatedUser)]
    public class GetAvailableFakeUsers : IRequest<FakeUsers>
    {
    }

    public class GetAvailableFakeUsersHandler : IRequestHandler<GetAvailableFakeUsers, FakeUsers>
    {
        private readonly IOptions<AadFakeAuthenticationOptions> options;

        public GetAvailableFakeUsersHandler(IOptions<AadFakeAuthenticationOptions> options)
        {
            this.options = options;
        }

        public Task<FakeUsers> Handle(GetAvailableFakeUsers request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new FakeUsers() { Values = options.Value.Users });
        }
    }

    public class AadFakeAuthenticationOptions
    {
        public string DefaultUserName { get; set; }
        public IEnumerable<FakeUser> Users { get; set; }
    }

    public class FakeUserOptions : IConfigureOptions<AadFakeAuthenticationOptions>
    {
        private readonly IConfiguration configuration;

        public FakeUserOptions(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Configure(AadFakeAuthenticationOptions options)
        {
            configuration.GetSection("AadFakeAuthentication").Bind(options);
        }
    }

    public static class AddAadFakeAuthenticationServiceExtensions
    {
        public static void AddAadFakeAuthentication(
            this IServiceCollection services
        )
        {
            services.ConfigureOptions<FakeUserOptions>();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = AadFakeAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = AadFakeAuthenticationDefaults.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, GlowAadFakeAuthenticationHandler>(
                    AadFakeAuthenticationDefaults.AuthenticationScheme, null);
        }
    }

    public static class AadFakeAuthenticationDefaults
    {
        public const string AuthenticationScheme = "AadFakeAuthentication";
    }

    public class GlowAadFakeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IEnumerable<Claim> claims;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private readonly IOptions<AadFakeAuthenticationOptions> fakeAuthOptions;
        private readonly IGraphTokenService tokenService;

        public GlowAadFakeAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IOptions<AadFakeAuthenticationOptions> fakeAuthOptions,
            IGraphTokenService tokenService
        )
            : base(options, logger, encoder, clock)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.fakeAuthOptions = fakeAuthOptions;
            this.tokenService = tokenService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var accessToken = await tokenService.AccessTokenForCurrentUser(new[] { "profile" });

            JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(token.Claims, Scheme.Name));

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}