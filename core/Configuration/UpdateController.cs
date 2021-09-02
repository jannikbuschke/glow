using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Glow.Configurations
{
    public class UpdateController<T> : ControllerBase where T : class, new()
    {
        private readonly IMediator mediator;
        private readonly IConfiguration configuration;
        private readonly Configurations partialConfigurations;
        private readonly IWebHostEnvironment environment;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfigurationRoot efConfiguration;

        public UpdateController(
            IMediator mediator,
            IConfiguration configuration,
            Configurations partialConfigurations,
            IWebHostEnvironment environment,
            IServiceProvider serviceProvider
        )
        {
            this.mediator = mediator;
            this.configuration = configuration;
            this.partialConfigurations = partialConfigurations;

            this.environment = environment;
            this.serviceProvider = serviceProvider;

            // TODO remove duplication
            var cs = configuration.GetValue<string>("ConnectionString");
            efConfiguration = new ConfigurationBuilder()
                .AddEFConfiguration(options => options.UseSqlServer(cs, configure =>
                {
                    configure.MigrationsAssembly(GetType().Assembly.FullName);
                })).Build();
        }

        [HttpGet]
        public ActionResult<T> Get()
        {
            var path = Request.Path.Value;
            var position = path.LastIndexOf("/") + 1;
            var configurationId = path[position..];

            Meta configurationMeta = partialConfigurations.Get().Single(v => v.Id == configurationId);
            T options = Activator.CreateInstance<T>();
            efConfiguration.GetSection(configurationMeta.SectionId).Bind(options);
            return options;
        }

        [HttpGet("{name}")]
        public ActionResult<T> Get(string name)
        {
            var path = Request.Path.Value.Replace("/" + name, "");
            var position = path.LastIndexOf("/") + 1;
            var configurationId = path[position..];

            Meta configurationMeta = partialConfigurations.Get().Single(v => v.Id == configurationId);
            T options = Activator.CreateInstance<T>();
            efConfiguration.GetSection(configurationMeta.SectionId + ":" + name).Bind(options);
            return options;
        }

        [HttpGet("from-options")]
        public ActionResult<T> GetFromOptions([FromServices] IOptionsSnapshot<T> options)
        {
            if (environment.IsDevelopment())
            {
                return options.Value;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UpdateRaw<T> value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = value.ToConfigurationUpdate();
            await mediator.Send(request);
            MethodInfo? m = typeof(T).GetMethod("OnSuccess");
            if (m != null)
            {
                m.Invoke(value.Value, new object[] { serviceProvider });
            }

            return Ok();
        }
    }
}