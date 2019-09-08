using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JannikB.Glue.AspNetCore
{
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    public class AuthenticationSettingsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthenticationSettingsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public ActionResult<AuthenticationSettings> GetSettings()
        {
            AuthenticationSettings settings = new AuthenticationSettings();
            configuration.Bind("Authentication", settings);
            return settings;
        }
    }
}
