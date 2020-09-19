using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;

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

        [HttpGet("teams-signin/{idToken}")]
        [HttpPost("teams-signin/{idToken}")]
        public async System.Threading.Tasks.Task<IActionResult> SigninWithIdtoken(string idToken)
        {
            HttpClient client = clientFactory.CreateClient();



            HttpResponseMessage result1 = await client.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token",
                new FormUrlEncodedContent
                (
                    //new List<KeyValuePair<string, string>>
                    //{
                    //    { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                    //    { "client_id", "26d5c3cb-545e-4047-a51b-7ffa0340ad12"},
                    //    { "client_secret", "9ZqiP1coKnQj~5wGnWfDMulvINBG_52.~7"},
                    //    { "scope", "user.read"},
                    //    { "requested_token_use", "on_behalf_of"},
                    //    { "assertion", request.IdToken}
                    //},
                    // gertruddemo/ms-teams-test-2
                    new List<KeyValuePair<string, string>>
                    {
                        { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                        { "client_id", "d0b12d68-8c72-486c-88cd-99d7ac7de745"},
                        { "client_secret", "Mr9AcL3cn.G8~XfZS-OUo-wW9V0N2hJ61_"},
                        { "scope", "user.read"},
                        { "requested_token_use", "on_behalf_of"},
                        { "assertion", idToken}
                    }
                    // gertrud phat demo
                    //new List<KeyValuePair<string, string>>
                    //{
                    //    { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                    //    { "client_id", "6e069957-7b5b-4be5-a00c-8915678b3c83"},
                    //    { "client_secret", "bLl1n8ber_0zuB~s1~.pHbc03djF.r_RmE"},
                    //    { "scope", "user.read"},
                    //    { "requested_token_use", "on_behalf_of"},
                    //    { "assertion", request.IdToken}
                    //}
            ));

            var content = await result1.Content.ReadAsStringAsync();
            RawToken data = JsonConvert.DeserializeObject<RawToken>(content);
            //logger.LogInformation("Response {status}, content: {content} ", result1.StatusCode, content);

            var stream = data.AccessToken;
            var handler = new JwtSecurityTokenHandler();
            Microsoft.IdentityModel.Tokens.SecurityToken jsonToken = handler.ReadToken(stream);
            var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

            var identity = new ClaimsIdentity(tokenS.Claims, CookieAuthenticationDefaults.AuthenticationScheme)
            {
                
            };
            identity.AddClaim(new Claim("preferred_username", "jannik.buschke@gertrud.digital"));
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(120),
                IsPersistent = true,
            });


            //var r = await tokenService.GetAccessTokenByIdToken(principal, idToken);

            //tokenService.AddAccountToCacheFromJwt(new string[] { "user.read" }, idToken, principal, null);
            //await tokenService.AddToCache(principal);

            //var principal = handler.ValidateToken(stream, new Microsoft.IdentityModel.Tokens.TokenValidationParameters {

            //}, out Microsoft.IdentityModel.Tokens.SecurityToken foo);


            //var p = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("oid", "1"), new Claim("tid", "1") }));

            //Microsoft.Identity.Client.AuthenticationResult result = await svc.GetAccessTokenByIdToken(p, request.IdToken);
            return Ok(Unit.Value);
        }

        //[HttpGet]
        //public IActionResult SignedOut()
        //{
        //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //}
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
