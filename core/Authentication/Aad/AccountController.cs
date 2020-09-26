using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Authentication.Aad
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
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

        //[HttpGet]
        //public IActionResult SignedOut()
        //{
        //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //}
    }
}