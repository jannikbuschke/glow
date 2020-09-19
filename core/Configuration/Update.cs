using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Glow.Configurations
{
    public class ConfigurationUpdate : IRequest
    {
        public string ConfigurationId { get; set; }
        public Dictionary<string, object> Values { get; set; }
    }

    public class ConfigurationUpdateHandler : IRequestHandler<ConfigurationUpdate>
    {
        private readonly Configurations partialConfigurations;

        public ConfigurationUpdateHandler(Configurations partialConfigurations)
        {
            this.partialConfigurations = partialConfigurations;
        }

        public async Task<Unit> Handle(ConfigurationUpdate request, CancellationToken cancellationToken)
        {
            Meta partialConfiguration = partialConfigurations.Get().Single(v => v.Id == request.ConfigurationId);

            var builder = new DbContextOptionsBuilder<ConfigurationDataContext>();
            StartupExtensions.optionsAction(builder);
            using var ctx = new ConfigurationDataContext(builder.Options);

            ConfigurationVersion current = await ctx
                .GlowConfigurations
                .Where(v => v.Id == request.ConfigurationId)
                .OrderByDescending(v => v.Version)
                .FirstOrDefaultAsync() ?? new ConfigurationVersion
                {
                    Version = 0,
                    Values = new Dictionary<string, object>()
                };

            var values = new Dictionary<string, object>(current.Values);
            foreach (KeyValuePair<string, object> value in request.Values)
            {
                values[partialConfiguration.SectionId + ":" + value.Key] = value.Value;
            }

            ctx.GlowConfigurations.Add(new ConfigurationVersion
            {
                Id = request.ConfigurationId,
                Version = current.Version + 1,
                Values = values,
                Created = DateTime.UtcNow,
                Message = "",
                User = null
            });

            await ctx.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
