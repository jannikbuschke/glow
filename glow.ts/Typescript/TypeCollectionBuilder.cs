using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Glow.Core.Authentication;
using Glow.Core.Linq;
using Glow.Core.Utils;
using Glow.TypeScript;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Reflection;
using OneOf;

namespace Glow.Core.Typescript
{
    public record Dependency
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public string Namespace { get; init; }
        public bool IsPrimitive { get; init; }
        public TsType TsType { get; set; }
    }

    public class TypeCollectionBuilder
    {
        private readonly IList<Type> types = new List<Type>();
        private readonly Dictionary<string, TsType> tsTypes = new Dictionary<string, TsType>();
        private readonly Dictionary<string, TsEnum> tsEnums = new Dictionary<string, TsEnum>();

        public int Count
        {
            get { return types.Count; }
        }

        public void Add<T>()
        {
            types.Add(typeof(T));
        }

        public void Add(Type type)
        {
            types.Add(type);
        }

        public void AddRange(IEnumerable<Type> types)
        {
            foreach (Type v in types)
            {
                Add(v);
            }
        }

        private List<TsType> visited = new List<TsType>();

        public TypeCollection Generate(Action<OneOf<TsType, TsEnum>> update)
        {
            foreach (Type type in types)
            {
                visited.Clear();
                OneOf<TsType, TsEnum> result = CreateOrGet(type);
            }

            var collection = new TypeCollection { Types = tsTypes, Enums = tsEnums };
            if (update != null)
            {
                //var all = collection.All().Select(v => v.AsT0).Where(v => v != null).ToList();
                var meeting = collection.All().Where(v => v.IsT0 && v.AsT0?.Name?.StartsWith("Meeting") == true)
                    .ToList();
                foreach (OneOf<TsType, TsEnum> v in collection.All())
                {
                    update(v);
                }
            }

            return collection;
        }

        private OneOf<TsType, TsEnum> CreateOrGet(Type type, bool skipDependencies = false)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                Type[] argTypes = type.GetGenericArguments();
                return new TsType
                {
                    Id = type.GetTypeId(),
                    Namespace = type.Namespace,
                    Name = "{ key: any, value: any }",
                    DefaultValue = "{ key: null, value: null }",
                    IsPrimitive = true,
                    IsCollection = false,
                    Properties = new List<Property>(),
                    Type = type
                };
            }

            if (type.Name.Contains("Workspace"))
            {
            }

            if (FSharpType.IsUnion(type, FSharpOption<BindingFlags>.None))
            {
                var id = type.FullName;
                if (tsTypes.ContainsKey(id))
                {
                    return tsTypes[id];
                }
                var cases = FSharpType.GetUnionCases(type, FSharpOption<BindingFlags>.None);

                var genericArguments = type.GetGenericArguments();
                var genericTypeArguments = type.GenericTypeArguments;
                var duTypeName = type.Name;
                if (genericArguments.Length > 0)
                {
                    var genericTsType = CreateOrGet(genericArguments.First());
                    var genericTsTypeName = CreateOrGet(genericArguments.First()).Match(v => v.Name, v => v.Name).Replace(" | null","");
                    duTypeName = duTypeName.Replace("`1", $"_{genericTsTypeName}");
                }

                var duCases = cases.Select(v => new DuCase()
                {
                    Name = @$"{duTypeName}_Case_{v.Name}",
                    CaseName = v.Name,
                    Fields = v.GetFields()
                        .Select(v => CreateOrGet(v.PropertyType))
                        .Select(v=>v.AsT0)
                        .ToArray()
                });

                var tsType = new TsDiscriminatedUnion()
                {
                    Id = type.FullName,
                    Name = duTypeName,
                    FullName = type.FullName,
                    Type = type,
                    Cases = duCases,
                    Namespace = type.Namespace,
                    Properties = new List<Property>(),
                    // DefaultValue = @"{ Case: ""None"" }",
                };

                tsTypes.Add(tsType.Id, tsType);
                return tsType;
            }

            if (type.IsEnum)
            {
                TsEnum e = AsEnum(type);
                tsEnums.TryAdd(e.Id, e);
                return e;
            }
            else
            {
                if (type.IsPrimitive())
                {
                    TsType primitiveTsType = GetPrimitive(type);
                    if (primitiveTsType != null)
                    {
                        return primitiveTsType;
                    }
                }

                if (type.IsDictionary())
                {
                    Tuple<string, string> dictTuple = AsDictionary(type);
                    TsType tsType = TupleAsPrimitive(dictTuple, type);
                    return tsType;
                }

                if (type.IsEnumerable())
                {
                    Type elementType = type.GetCollectionElementType();

                    if (elementType.IsPrimitive())
                    {
                        TsType primitiveTsType = GetPrimitiveCollection(type);
                        return primitiveTsType;
                    }

                    if (type.FullName == null)
                    {
                        Console.WriteLine("Cannot generate type for " + type.Name + " ( element type = " +
                                          elementType.Name + ") as Fullname is null (not yet supported)");
                        return TsType.Any();
                    }

                    if (type.FullName.Contains("Newtonsoft"))
                    {
                        return TsType.Any();
                    }

                    TsType enumerable = AsEnumerable(type);
                    tsTypes.TryAdd(enumerable.Id, enumerable);
                    return enumerable;
                }

                var id = type.GetTypeId();
                if (!tsTypes.ContainsKey(id))
                {
                    // if (IsFsharOption(type))
                    {
                        // return AsFsharpOption(type);
                    }

                    if (IsNullable(type))
                    {
                        OneOf<TsType, TsEnum> nullable = AsNullable(type);

                        nullable.Switch(tsType =>
                        {
                            tsType.Id = id;
                            tsTypes.Add(id, tsType);
                            PopuplateProperties(tsType);
                        }, v1 =>
                        {
                            tsEnums.TryAdd(v1.Id, v1);
                        });
                        return nullable;
                    }
                    else
                    {
                        if (type.Namespace == null)
                        {
                            // type.Namespace = type.ReflectedType.FullName;
                            var reflectedType = type.ReflectedType;
                            return TsType.Any();
                        }
                        else if (type.Namespace.StartsWith("System"))
                        {
                            return TsType.Any();
                        }

                        TsType tsType = Create(type, skipDependencies);

                        tsType.Id = id;
                        if (type.IsGenericParameter)
                        {
                            return tsType;
                        }

                        tsTypes.Add(id, tsType);
                        PopuplateProperties(tsType);
                    }
                }

                return tsTypes[id];
            }
        }

        // private OneOf<TsType, TsEnum> AsFsharpOption(Type type)
        // {
        //     Type[] genericTypeArguments = type.GenericTypeArguments;
        //     Type genericArgument = genericTypeArguments.First();
        //     if (!genericArgument.IsEnum && !genericArgument.IsPrimitive())
        //     {
        //         return CreateOrGet(genericArgument);
        //     }
        //     else if (genericArgument.IsEnum)
        //     {
        //         throw new Exception("enum FS Option currently not supported");
        //     }
        //     else if (genericArgument.IsPrimitive())
        //     {
        //         return GetPrimitiveAsNullable(genericArgument);
        //     }
        //     else
        //     {
        //         throw new Exception("enum/primitive FS Option currently not supported");
        //     }
        // }

        private TsType AsEnumerable(Type type)
        {
            Type elementType = type.GetCollectionElementType();

            OneOf<TsType, TsEnum> elementTsType =
                elementType == null ? TsType.Any() : CreateOrGet(elementType, skipDependencies: false);

            var name = elementTsType.Match(v1 => v1.Name, v2 => v2.Name);
            var nameSpace = elementTsType.Match(v1 => v1.Namespace, v2 => v2.Namespace);
            var id = type.GetTypeId();

            return new TsType
            {
                Id = id, //argsTsType.Namespace + "." + argsTsType.Name + "[]",
                Name = name + "[]",
                Namespace = nameSpace,
                DefaultValue = "[]",
                Properties = new List<Property>(),
                IsCollection = true,
                IsPrimitive = elementTsType.Match(v1 => v1.IsPrimitive, v2 => true)
            };
        }

        private Tuple<string, string> AsDictionary(Type type)
        {
            Type keyType = type.GenericTypeArguments[0];
            TsType keyTsType = GetPrimitive(type.GenericTypeArguments[0]);

            var keyName = keyTsType.Type == typeof(string)
                ? "string"
                : keyTsType.Name;

            Type valueType = type.GenericTypeArguments[1];
            OneOf<TsType, TsEnum> valueTsType = CreateOrGet(valueType);

            var name = valueTsType.Match(v1 => v1.Name, v2 => v2.Name);

            return new Tuple<string, string>
            (
                $"{{ [key: {keyName}]: {name} }}",
                "{}"
            );
        }

        private TsType TupleAsPrimitive(Tuple<string, string> value, Type type)
        {
            (var name, var defaultValue) = value;
            return new TsType
            {
                Name = name,
                DefaultValue = defaultValue,
                IsPrimitive = true,
                Properties = new List<Property>(),
                Type = type,
            };
        }

        private TsEnum AsEnum(Type t)
        {
            IEnumerable<string> values = GetEnumValues(t);
            return new TsEnum
            {
                Name = t.Name,
                FullName = t.FullName,
                DefaultValue = values.First(),
                Values = values,
                Namespace = t.Namespace
            };
        }

        private static IEnumerable<string> GetEnumValues(Type t)
        {
            Array values = Enum.GetValues(t);
            foreach (object val in values)
            {
                yield return Enum.GetName(t, val);
            }
        }

        private TsType GetPrimitiveCollection(Type type)
        {
            Type elementType = type.GetCollectionElementType();
            try
            {
                if (IsNullable(elementType))
                {
                    var t = GetPrimitiveAsNullable(elementType);
                    return t;
                }
                else
                {
                    Tuple<string, string> primitiveTuple = GetTypeExtension.primitiveCollection[elementType];
                    (var name, var defaultValue) = primitiveTuple;
                    TsType t = TupleAsPrimitive(primitiveTuple, type);
                    return t;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not get primitive for " + type.FullName + " / element type = " +
                                    elementType.FullName + " " + e.Message);
            }
        }

        private TsType GetPrimitiveAsNullable(Type type)
        {
            if (!GetTypeExtension.primitives.ContainsKey(type))
            {
                throw new Exception($"Type '{type.FullName}' not a primitive");
            }

            (var name, var defaultValue) = GetTypeExtension.primitives[type];
            return new TsType
            {
                Name = name.EndsWith("null") ? name : $"{name} | null",
                DefaultValue = "null",
                IsPrimitive = true,
                Properties = new List<Property>(),
                Type = type,
            };
        }

        private TsType GetPrimitive(Type type)
        {
            if (GetTypeExtension.primitives.ContainsKey(type))
            {
                (var name, var defaultValue) = GetTypeExtension.primitives[type];
                return new TsType
                {
                    Name = name,
                    DefaultValue = defaultValue,
                    IsPrimitive = true,
                    Properties = new List<Property>(),
                    Type = type,
                };
            }

            if (type.IsDictionary())
            {
                (var name, var defaultValue) = GetTypeExtension.primitives[type];
                return new TsType
                {
                    Name = name,
                    DefaultValue = defaultValue,
                    IsPrimitive = true,
                    Properties = new List<Property>(),
                    Type = type
                };
            }

            return null;
        }

        private void PopuplateProperties(TsType type)
        {
            type.Properties = type.PropertyInfos?
                .DistinctBy(v => v.Name)
                .Select(v =>
                {
                    // problem, current type does not yet exist on dependency
                    OneOf<TsType, TsEnum> tsType = CreateOrGet(v.PropertyType, skipDependencies: true);

                    var defaultValue = tsType.Match(v1 => v1.DefaultValue, v2 =>
                    {
                        return "default" + v2.Name;
                    });
                    var typeName = tsType.Match(v1 =>
                    {
                        if (v1.Name == "any") { return "any"; }

                        return v1.IsPrimitive ? v1.Name : v1.Name;
                    }, v2 => v2.Name);
                    var isNullable = tsType.Match(v1 => false, v2 => v2.IsNullable);
                    return new Property
                    {
                        PropertyName = v.Name.CamelCase(),
                        DefaultValue = defaultValue,
                        TsType = tsType,
                        IsNullable = isNullable,
                        TypeName = typeName,
                        IsCyclic = tsType.IsT0 && visited.Contains(tsType.AsT0), // true, // if already visited
                    };
                }).ToList();
        }

        private bool IsFsharOption(Type type)
        {
            return type.Name.StartsWith("FSharpOption");
        }

        private bool IsNullable(Type type)
        {
            return type.Name.StartsWith("Nullable");
        }

        private OneOf<TsType, TsEnum> AsNullable(Type type)
        {
            Type[] genericArguments = type.GetGenericArguments();
            Type[] genericTypeArguments = type.GenericTypeArguments;

            Type genericArgument = genericTypeArguments.First();

            if (genericArgument.IsEnum)
            {
                TsEnum t = CreateOrGet(genericArgument).AsT1;
                t.Name = t.Name;
                t.IsNullable = true;

                return t;
            }
            else
            {
                var name = genericArgument.Name + " | null";
                //var name = genericArgument.Namespace + "." + genericArgument.Name + " | null";
                var t = new TsType
                {
                    FullName = type.FullName,
                    IsPrimitive = type.Name.StartsWith("Nullable") || type.Name.StartsWith("FSharpOption"),
                    Name = name,
                    Namespace = type.Namespace,
                    DefaultValue = null,
                    //DefaultValue = genericArguments.Length != 0 ? null : type.Namespace + ".default" + name,
                    Type = type,
                    PropertyInfos = null,
                };
                return t;
            }
        }

        private TsType Create(Type type, bool skipDependencies = false)
        {
            if (type.IsGenericParameter)
            {
                return new TsType
                {
                    FullName = type.FullName ?? type.Name ?? "T",
                    DefaultValue = "{}",
                    IsPrimitive = false,
                    Name = type.Name ?? "T",
                    Type = type,
                    Properties = new List<Property>(),
                    PropertyInfos = Array.Empty<PropertyInfo>()
                };
            }

            Type[] genericTypeArguments = type.GenericTypeArguments;
            Type[] genericArguments = type.GetGenericArguments();

            var name = genericTypeArguments.Length != 0
                ? type.Name.StartsWith("Nullable")
                    ? genericTypeArguments.First().Name + " | null"
                    : type.Name.Replace(".", "").Replace("`", "") +
                      string.Join("", genericTypeArguments.Select(v => v.Name))
                : genericArguments.Length != 0
                    ? Regex.Replace(type.Name, "`.*$",
                        "<" + string.Join(", ", genericArguments.Select(v => v.Name)) + ">")
                    : type.Name;


            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var value = new TsType
            {
                FullName = type.FullName,
                IsPrimitive = type.Name.StartsWith("Nullable"),
                Name = name,
                Namespace = type.Namespace,
                DefaultValue = genericArguments.Length != 0 ? null : "default" + name,
                Type = type,
                PropertyInfos = props,
            };
            return value;
        }
    }
}