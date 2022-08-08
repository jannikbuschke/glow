using System.Reflection;
using OneOf;

namespace Glow.Core.Typescript
{
    public abstract class BaseTsType
    {
        public virtual string Id { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public List<Dependency> AdditionalDependencies { get; set; } = new();

        public bool IsGeneric { get; set; }
        public bool IsCollection { get; set; }

        // public List<GenericArgument> GenericArguments { get; set; }
        public List<TsType> GenericArgumentsTsTypes { get; set; } = new();

        public string GetFullName()
        {
            if (string.IsNullOrEmpty(Namespace))
            {
                return Name;
            }
            if (IsGeneric && !IsCollection)
            {
                var fullGenericNames = string.Join(",", GenericArgumentsTsTypes.Select(v => v.GetFullName()));
                return $"{Namespace?.Replace(".", "_")}.{Name}<{fullGenericNames}>";
            }
            else if (IsCollection)
            {
                return $"Array<{string.Join(",", GenericArgumentsTsTypes.Select(v => v.GetFullName()))}>";
            }

            var template = $"<{GenericArgumentsTsTypes.FirstOrDefault()?.Name}>";
            var replacement = $"<{GenericArgumentsTsTypes.FirstOrDefault()?.GetFullName()}";
            var name = Name.Contains(template) ? Name.Replace(template, replacement) : Name;
            return $"{Namespace?.Replace(".", "_")}.{name}";
        }
    }

    public class TsDiscriminatedUnion : TsType
    {
        public bool Nullable { get; set; }
        public IEnumerable<DuCase> Cases { get; set; }
        public string NameWithGenericArguments { get; set; }
    }

    public class DuCase
    {
        public string Name { get; set; }
        public string CaseName { get; set; }
        public TsType TsType { get; set; }
        public TsType[] Fields { get; set; }
        public bool IsNull { get; set; }
    }

    public class GenericArgument
    {
        public string Name { get; set; }
        public TsType TsType { get; set; }
    }

    public class TsType : BaseTsType
    {
        private string id;
        public override string Id
        {
            get
            {
                if (id == null)
                {
                    return FullName;
                }
                return id;
            }
            set { this.id = value; }
        }
        public bool IsPrimitive { get; set; }
        public Type Type { get; set; }

        public string DefaultValue { get; set; }

        public List<Property> Properties { get; set; }
        public PropertyInfo[] PropertyInfos { get; set; }

        public List<Dependency> GetDependencies(int depth = 5)
        {
            if (depth == 0)
            {
                return new List<Dependency>();
            }

            var deps = new List<Dependency>();
            deps.AddRange(this.AdditionalDependencies);
            deps.AddRange(this.GenericArgumentsTsTypes.Select(v => Dependency.FromTsType(v)));

            var nextDepth = depth - 1;

            if (Properties != null)
            {
                deps.AddRange(this.Properties.SelectMany(v => v.TsType.Match(
                    v =>
                    {
                        if (v.IsGeneric && !v.IsCollection)
                        {
                            var deps = v.GenericArgumentsTsTypes.SelectMany(v => v.GetDependencies(nextDepth)).ToList();
                            deps.Add(Dependency.FromTsType(this));
                            return deps.ToList();
                        }

                        if (v.Name.Contains("Person"))
                        {
                        }

                        if (v.IsCollection && v.Name.Contains("Person"))
                            // if (v.IsCollection )
                        {
                            var argsAsDependencies = v.GenericArgumentsTsTypes.Select(v => Dependency.FromTsType(v)).ToList();

                            var deps = v.GenericArgumentsTsTypes.SelectMany(v => v.GetDependencies(nextDepth));
                            argsAsDependencies.AddRange(deps);
                            return argsAsDependencies.AsEnumerable();
                        }

                        return new[]
                        {
                            new Dependency
                            {
                                Id = v.Id?.Replace("[]", "").Replace("<T>", ""),
                                Namespace = v.Namespace,
                                Name = v.Name?.Replace("[]", "").Replace("<T>", ""),
                                IsPrimitive = v.IsPrimitive,
                                TsType = v
                            }
                        };
                    },
                    v =>
                    {
                        return new[]
                        {
                            new Dependency
                            {
                                Id = v.Id.Replace("[]", "").Replace("<T>", ""),
                                Namespace = v.Namespace,
                                Name = v.Name.Replace("[]", "").Replace("<T>", ""),
                                IsPrimitive = false
                            }
                        }.ToList();
                    })).ToList());
            }

            return deps;
        }

        public bool HasCyclicDependency { get; set; }


        public static TsType Any()
        {
            return new()
            {
                Id = "any",
                IsPrimitive = true,
                FullName = "any",
                Name = "any",
                DefaultValue = "null"
            };
        }
    }

    public class Property
    {
        public string PropertyName { get; set; }
        public string TypeName { get; set; }
        public string DefaultValue { get; set; }
        public OneOf<TsType, TsEnum> TsType { get; set; }
        public bool IsCyclic { get; set; }
        public bool IsNullable { get; set; }

        public override string ToString()
        {
            return PropertyName + ": " + TsType.Match(v => v.Name, v => v.Name);
        }
    }
}