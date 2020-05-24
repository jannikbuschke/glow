using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EfConfigurationProvider.Core
{
    public class ConfigurationProvider : Microsoft.Extensions.Configuration.ConfigurationProvider, IConfigurationProvider
    {
        public static ConfigurationProvider Value { private set; get; }
        private Configuration Configuration { set; get; }

        public ConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
            ConfigurationProvider.Value = this;

            var builder = new DbContextOptionsBuilder<SqlServerConfigurationDataContext>();

            OptionsAction(builder);

            using var dbContext = new SqlServerConfigurationDataContext(builder.Options);
            dbContext.Database.Migrate();
        }

        private Action<DbContextOptionsBuilder> OptionsAction { get; }

        public void Reload()
        {
            Load();
            OnReload();
        }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<SqlServerConfigurationDataContext>();

            OptionsAction(builder);

            using var dbContext = new SqlServerConfigurationDataContext(builder.Options);
            Configuration = dbContext
                .GlowConfigurations
                .OrderByDescending(v => v.Created)
                .FirstOrDefault() ?? new Configuration { Values = new Dictionary<string, string>() };
            Data = Configuration.Values;
        }

        public Configuration GetConfiguration()
        {
            return Configuration;
        }
        public IDictionary<string, string> GetData()
        {
            return Data;
        }
    }
}
