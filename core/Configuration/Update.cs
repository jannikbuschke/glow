using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Glow.Configurations
{
    public class Update : IRequest
    {
        public string ConfigurationId { get; set; }
        public ConfigurationValue[] Values { get; set; }
    }

    public class UpdateHandler : IRequestHandler<Update>
    {
        private readonly Configurations partialConfigurations;

        public UpdateHandler(Configurations partialConfigurations)
        {
            this.partialConfigurations = partialConfigurations;
        }

        public async Task<Unit> Handle(Update request, CancellationToken cancellationToken)
        {
            Meta partialConfiguration = partialConfigurations.Get().Single(v => v.Id == request.ConfigurationId);

            var builder = new DbContextOptionsBuilder<ConfigurationDataContext>();
            StartupExtensions.optionsAction(builder);
            using var ctx = new ConfigurationDataContext(builder.Options);

            ConfigurationVersion current = await ctx
                .GlowConfigurations
                .OrderByDescending(v => v.Version)
                .FirstOrDefaultAsync() ?? new ConfigurationVersion
                {
                    Version = 0,
                    Values = new Dictionary<string, string>()
                };

            var values = new Dictionary<string, string>(current.Values);
            foreach (ConfigurationValue value in request.Values)
            {
                values[partialConfiguration.SectionId + ":" + value.Name] = value.Value;
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
