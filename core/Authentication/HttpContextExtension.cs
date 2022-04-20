using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Glow.Authentication.Aad
{
    public static class HttpContextExtension
    {
        public static string GetUserObjectId(this HttpContext httpContext)
        {
            ClaimsPrincipal user = httpContext.User;
            return user.GetObjectId();
        }

        public static string GetUpn(this HttpContext httpContext)
        {
            ClaimsPrincipal user = httpContext.User;
            return user.Upn();
        }
    }
}