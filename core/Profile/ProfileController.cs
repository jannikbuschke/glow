using System.Linq;
using Glow.Authentication.Aad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Glow.Core.Profiles
{
    public class Profile
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string IdentityName { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    public class HasConsented
    {
        public bool Value { get; set; }
    }

    [Route("glow/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> logger;

        public ProfileController(ILogger<ProfileController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public ActionResult<Profile> Get()
        {
            var isAuthenticated = User?.Identity.IsAuthenticated ?? false;

            return new Profile
            {
                DisplayName = User.Name(),
                Email = User.Email(),
                Id = User.NameIdentifier(),
                IdentityName = User?.Identity.Name,
                IsAuthenticated = isAuthenticated
            };
        }

        [HttpGet("claims")]
        [Authorize]
        public object Claims()
        {
            var claims = User.Claims.Select(c => new { c.Value, c.Type }).ToList();
            logger.LogInformation("Claims {@claims}", claims);

            return claims;
        }
    }
}
