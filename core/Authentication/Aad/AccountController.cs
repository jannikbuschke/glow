using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Serilog;

namespace Glow.Authentication.Aad
{
    [Route("[controller]/[action]")]
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly TokenService tokenService;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<AccountController> logger;
        private readonly IAuthorizationService authorizationService;

        public AccountController(
            TokenService tokenService,
            IHttpClientFactory clientFactory,
            ILogger<AccountController> logger,
            IAuthorizationService authorizationService
        )
        {
            this.tokenService = tokenService;
            this.clientFactory = clientFactory;
            this.logger = logger;
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        public IActionResult SignIn(string[] scopes, string redirectUrl)
        {
            var url = string.IsNullOrEmpty(redirectUrl) ? "/" : redirectUrl;// Url.Action(nameof(AccountController.SignIn), "Home");
            var para = new AuthenticationProperties
            {
                RedirectUri = url,
            };
            if (scopes != null)
            {
                para.Parameters.Add("scopes", scopes);
            }

            return Challenge(para, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                return SignOut(
                    new AuthenticationProperties { RedirectUri = "/" },
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    OpenIdConnectDefaults.AuthenticationScheme);
            }

            return RedirectToAction("/");
        }

        private const string ClientId = "d0b12d68-8c72-486c-88cd-99d7ac7de745";
        private const string ClientSecret = "Mr9AcL3cn.G8~XfZS-OUo-wW9V0N2hJ61_";

        [HttpPost("teams-signin/{idToken}")]
        public async System.Threading.Tasks.Task<IActionResult> SigninWithIdtoken(string idToken)
        {
            //// validate idToken

            Log.Logger.Information("token: " + idToken);

            JwtSecurityToken tokenS = ParseAccessToken(idToken);// await MsalOnBehalf(idToken);
            //JwtSecurityToken tokenS = await LowlevelOnBehalf(idToken);

            var identity = new ClaimsIdentity(tokenS.Claims, CookieAuthenticationDefaults.AuthenticationScheme)
            { };
            //identity.AddClaim(new Claim("preferred_username", "jannik.buschke@gertrud.digital"));
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(120),
                IsPersistent = true,
            });

            return Ok(tokenS.Claims);
        }

        private async Task<JwtSecurityToken> MsalOnBehalf(string token)
        {
            var app = ConfidentialClientApplicationBuilder.Create(clientId: ClientId)
                .WithClientSecret(ClientSecret)
                .Build();

            var result = await app.AcquireTokenOnBehalfOf(
                new[] { "user.read" },
                new UserAssertion(jwtBearerToken: token)
            ).ExecuteAsync();

            var tokenS = ParseAccessToken(result.AccessToken);

            return tokenS;
        }

        private async Task<JwtSecurityToken> LowlevelOnBehalf(string idToken)
        {
            HttpClient client = clientFactory.CreateClient();

            HttpResponseMessage result = await client.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token",
                new FormUrlEncodedContent
                (
                    // gertruddemo/ms-teams-test-2
                    new List<KeyValuePair<string, string>>
                    {
                        { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                        { "client_id",  ClientId },
                        { "client_secret", ClientSecret },
                        { "scope", "user.read"},
                        { "requested_token_use", "on_behalf_of"},
                        { "assertion", idToken}
                    }
            ));

            var content = await result.Content.ReadAsStringAsync();
            Log.Logger.Information("on behalf result: " + content);

            RawToken data = JsonConvert.DeserializeObject<RawToken>(content);
            //logger.LogInformation("Response {status}, content: {content} ", result1.StatusCode, content);

            var tokenS = ParseAccessToken(data.AccessToken);

            return tokenS;
        }

        private JwtSecurityToken ParseAccessToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            Microsoft.IdentityModel.Tokens.SecurityToken jsonToken = handler.ReadToken(token);
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            return tokenS;
        }
    }

    public class RawToken
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("ext_expires_in")]
        public int ExtExpiresIn { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }

    public class SigninWidthIdToken
    {
        public string IdToken { get; set; }
    }
}
