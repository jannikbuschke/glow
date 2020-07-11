using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Glow.Configurations
{
    public class EfConfigurationProvider : IConfigurationProvider
    {
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

        public EfConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;

            var builder = new DbContextOptionsBuilder<SqlServerConfigurationDataContext>();

            OptionsAction(builder);

            using var dbContext = new SqlServerConfigurationDataContext(builder.Options);
            dbContext.Database.Migrate();
        }
        public Action<DbContextOptionsBuilder> OptionsAction { get; }

        protected IDictionary<string, string> Data
        {
            get
            {
                var builder = new DbContextOptionsBuilder<SqlServerConfigurationDataContext>();

                OptionsAction(builder);

                using var dbContext = new SqlServerConfigurationDataContext(builder.Options);

                var data = dbContext.GlowConfigurations
                    .OrderByDescending(v => v.Version)
                    .ToList();

                var configurations = data
                    .GroupBy(v => v.Id)
                    .SelectMany(v => v.First().Values)
                    .ToDictionary(v => v.Key, v => v.Value);
                var cfg = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> item in configurations)
                {
                    cfg[item.Key] = item.Value;
                }
                return cfg;
            }
        }

        public virtual bool TryGet(string key, out string value)
        {
            return Data.TryGetValue(key, out value);
        }

        public virtual void Set(string key, string value)
        {
            Data[key] = value;
        }

        public virtual void Load()
        {
            // no-op
        }

        public virtual IEnumerable<string> GetChildKeys(
            IEnumerable<string> earlierKeys,
            string parentPath)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + ConfigurationPath.KeyDelimiter;

            return Data
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(kv => Segment(kv.Key, prefix.Length))
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        private static string Segment(string key, int prefixLength)
        {
            var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
        }

        public IChangeToken GetReloadToken()
        {
            return _reloadToken;
        }

        protected void OnReload()
        {
            ConfigurationReloadToken previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }
    }
}
