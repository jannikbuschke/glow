using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JannikB.Glue.AspNetCore
{
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    [Authorize]
    public class GlobalSettingsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public GlobalSettingsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public ActionResult<GlobalSettings> GetSettings()
        {
            GlobalSettings settings = new GlobalSettings();
            configuration.Bind("Global", settings);
            return settings;
        }
    }
}
