using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Glow.Authentication.Aad
{
    [Route("api/sign-in")]
    [ApiVersion("1.0")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = configuration["OpenIdConnect:RedirectUri"];
            return Challenge(
                new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("accept-code")]
        public async Task<ActionResult<object>> SignIn(string code)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "user.Email"),
                new Claim("FullName","user.FullName"),
                new Claim(ClaimTypes.Role, "Administrator"),
            };
            //var client = configuration["Authentication:ClientId"];
            //var tenant = configuration["Authentication:TenantId"];
            //var app = new PublicClientApplicationBuilder()
            //    .Create(client)
            //    .WithTenantId(tenant)
            //    .WithClientSecret("EpY=?d8Y_QGkhU_NeEc1Y4shjgxbqIe0")
            //    .Build();
            //var c = app.AcquireTokenByAuthorizationCode(new string[] { "User.Read" }, code);
            //var result = await c.ExecuteAsync();

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok();
        }

        [Authorize]
        [HttpGet("test")]
        public object Test()
        {
            ClaimsPrincipal user = User;
            return user.Claims.Select(v => new { v.Type, v.Value }).ToList();
        }
    }
}
