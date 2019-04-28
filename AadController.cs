using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace JannikB.Glue.AspNetCore
{
    [ApiVersion("1.0")]
    [Route("api/Aad")]
    public class AadController : ControllerBase
    {
        private readonly IConfiguration config;

        public AadController(IConfiguration config)
        {
            this.config = config;
        }

        [HttpGet("Settings")]
        public object GetSettings()
        {
            return new
            {
                AppId = config["Aad:AppId"],
                Authority = config["Aad:Authority"]
            };
        }

        [Authorize]
        [HttpGet("claims")]
        public object Get()
        {
            return User.Claims;
        }
    }
}
