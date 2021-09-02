using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Configurations
{
    [Route("api/configuration-schemas")]
    [ApiController]
    [Authorize, AllowAnonymous]
    public class SchemaController : ControllerBase
    {
        private readonly Configurations partialConfigurations;

        public SchemaController(
            Configurations partialConfigurations
        )
        {
            this.partialConfigurations = partialConfigurations;
        }

        [HttpGet]
        public IEnumerable<IConfigurationMeta> Get()
        {
            return partialConfigurations.Get();
        }
    }
}