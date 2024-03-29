using System;
using System.Collections.Generic;
using System.Linq;
using Glow.Core.Linq;
using OneOf;

namespace Glow.Core.Typescript
{
    public class Module
    {
        public Module(string @namespace, IEnumerable<OneOf<TsType, TsEnum>> types)
        {
            this.Namespace = @namespace;
            this.Types = types;
            TsEnums = types.Where(v => v.IsT1).Select(v => v.AsT1).ToList();
            var tsTypes = types
                .Where(v => v.IsT0)
                .Select(v => v.AsT0)
                .Where(v => !v.IsPrimitive)
                .DistinctBy(v => v.Id)
                .ToList();

            // very slow for big collections (<100 might be okay)
            // especially slow with debugger attached
            IList<TsType> sorted = tsTypes.TopologicalSort(
                v => v.Properties?.Where(v => v.TsType.IsT0 && !v.TsType.AsT0.IsPrimitive)
                    .Select(v => v.TsType.AsT0));
            sorted = sorted.Where(v => !v.IsCollection).ToList();

            TsTypes = new List<TsType>();
            var missing = tsTypes.Where(v => !sorted.Contains(v) && !v.IsPrimitive).ToList();
            TsTypes.AddRange(sorted.Where(v => v.Namespace == this.Namespace));
            TsTypes.AddRange(missing.Where(v => v.Namespace == this.Namespace));
        }

        public string Namespace { get; set; }
        private IEnumerable<OneOf<TsType, TsEnum>> Types { get; set; }

        public IEnumerable<TsEnum> TsEnums { get; private set; }
        public List<TsType> TsTypes { get; private set; }

        public IEnumerable<IGrouping<string, Dependency>> GetDependenciesGroupedByNamespace()
        {
            IEnumerable<IGrouping<string, Dependency>> result = GetDependencies()
                .GroupBy(v => v.Namespace);
            return result;
        }

        public IEnumerable<Dependency> GetDependencies()
        {
            IEnumerable<Dependency> directDependencies = this.Types
                .Where(v => v.IsT0)
                .Select(v => v.AsT0)
                .Where(v => v.Properties != null)
                .SelectMany(v => v.Properties)
                .Select(v => v.TsType.Match(
                    v => new Dependency
                    {
                        Id = v.Id?.Replace("[]", ""),
                        Namespace = v.Namespace,
                        Name = v.Name?.Replace("[]", ""),
                        IsPrimitive = v.IsPrimitive,
                        TsType = v
                    },
                    v => new Dependency
                    {
                        Id = v.Id.Replace("[]", ""),
                        Namespace = v.Namespace,
                        Name = v.Name.Replace("[]", ""),
                        IsPrimitive = false
                    }))
                .Where(v => !v.IsPrimitive)
                .Where(v => v.Namespace != this.Namespace && v.Name != "any");

            IEnumerable<Dependency> subDependencies = this.TsTypes
                .SelectMany(v => v.Properties)
                .Where(v => v.TsType.IsT0)
                .Select(v => v.TsType.AsT0)
                .Where(v => v.HasCyclicDependency)
                .SelectMany(v => v.Properties)
                .Select(v => v.TsType.Match(
                    v => new Dependency
                    {
                        Id = v.Id?.Replace("[]", ""),
                        Namespace = v.Namespace,
                        Name = v.Name?.Replace("[]", ""),
                        IsPrimitive = v.IsPrimitive,
                        TsType = v
                    },
                    v => new Dependency { Id = v.Id, Namespace = v.Namespace, Name = v.Name, IsPrimitive = false }))
                .Where(v => !v.IsPrimitive)
                .Where(v => v.Namespace != this.Namespace && v.Name != "any");

            var all = new List<Dependency>();
            all.AddRange(directDependencies);
            all.AddRange(subDependencies);

            IEnumerable<Dependency> result = all
                .Where(v => !v.Namespace.StartsWith("System.Collections"))
                .DistinctBy(v => v.Id)
                .DistinctBy(v => v.Namespace + v.Name);
            return result;
        }
    }
}