using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Glow.Ts;
using Glow.TypeScript;
using Microsoft.CodeAnalysis.Options;
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

        public static Dependency FromTsType(TsType value)
        {
            return new()
            {
                TsType = value,
                Id = value.Id,
                Name = value.Name,
                Namespace = value.Namespace,
                IsPrimitive = value.IsPrimitive
            };
        }
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
                try
                {
                    OneOf<TsType, TsEnum> result = CreateOrGet(type);
                }
                catch (CodeGenerationNotSupported e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Could not generate ts type for " + type.FullName);
                    throw;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not generate ts type for " + type.FullName);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }

            var collection = new TypeCollection { Types = tsTypes, Enums = tsEnums };
            if (update != null)
            {
                //var all = collection.All().Select(v => v.AsT0).Where(v => v != null).ToList();
                // var meeting = collection.All().Where(v => v.IsT0 && v.AsT0?.Name?.StartsWith("Meeting") == true)
                //     .ToList();
                foreach (OneOf<TsType, TsEnum> v in collection.All())
                {
                    update(v);
                }
            }

            return collection;
        }

        private OneOf<TsType, TsEnum> CreateOrGet(Type type, bool skipDependencies = false)
        {
            if (type.Namespace.Contains("System.Text.Json.Serialization")&&type.Name=="Skippable`1"
            // type == typeof(Skippable<>) || type.GetGenericTypeDefinition() == typeof(Skippable<>)||type.Name.Contains("UpdateMeetingItemInvite")
                )
            {
                return TsType.Any();
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(FSharpOption<>))
            {
                var args = type.GetGenericArguments().First();
                var tsType = CreateOrGet(args);
                if (tsType.IsT0)
                {
                    var t0 = tsType.AsT0;
                    if (t0.IsPrimitive)
                    {
                        t0.DefaultValue = "null";
                        t0.Name = t0.Name.EndsWith("null") ? t0.Name : t0.Name + " | null";
                    }
                    else
                    {
                        t0.IsNullable = true;
                        t0.DefaultValue = "null";
                    }
                }

                return tsType;
            }

            if (type == typeof(object))
            {
                return TsType.Any();
            }
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

            TsType AddAsEnumerable(Type type)
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

            if (FSharpType.IsUnion(type, null))
            {

                var cases = FSharpType.GetUnionCases(type, FSharpOption<BindingFlags>.Some(BindingFlags.Public));

                if (cases.Length == 1)
                {
                    var case0 = cases.First();
                    var name = cases.First().Name;
                    var fields = case0.GetFields();
                    if (fields.Length == 1)
                    {
                        // single case DU will be inlined
                        var fieldType = CreateOrGet(fields.First().PropertyType);
                        if (fieldType.IsT0 && fieldType.AsT0.IsPrimitive)
                        {
                            return fieldType;
                        }
                        else
                        {
                            throw new CodeGenerationNotSupported("Single case DU with non-primitive field not yet supported Name = " + type.FullName);
                        }

                    }
                    else
                    {
                        throw new CodeGenerationNotSupported("Single case DU with multiple fields not yet supported Name= "  + type.FullName);
                    }
                }
                else if(!type.IsGenericType)
                {
                    var fullName = type.FullName;
                    if (tsTypes.ContainsKey(fullName))
                    {
                        return tsTypes[fullName];
                    }
                    var tsCases = cases.Select(v =>
                    {
                        var fields = v.GetFields()
                            .Select(v => CreateOrGet(v.PropertyType))
                            .Select(v => v.AsT0)
                            .ToArray();
                        return new DuCase() { CaseName = v.Name, Name = v.Name, Fields = fields };
                    }).ToList();
                    var du = new TsDiscriminatedUnion()
                    {
                        Name = type.Name,
                        FullName = fullName,
                        Properties = tsCases.SelectMany(v=>v.Fields)
                            .Select(v=> OneOf<TsType, TsEnum>.FromT0(v))
                            .Select(v=>new Property(){TsType=v})
                            .ToList(),
                        DefaultValue = $"null as any as {type.Name}",
                        Namespace = type.Namespace,
                        Nullable = false,
                        Cases = tsCases
                    };
                    tsTypes.Add(du.FullName,du);
                    return du;
                }

                if (type.IsEnumerable())
                {
                    return AddAsEnumerable(type);
                }

                var isGenericType = type.IsGenericType;

                if (isGenericType)
                {
                    var genericTypeDefinition = type.GetGenericTypeDefinition();

                    if (genericTypeDefinition == typeof(FSharpOption<>))
                    {
                        //as FSharpOption
                        var args = type.GetGenericArguments().First();
                        var tsType = CreateOrGet(args);
                        // var asNullable = AsNullable(args);
                        if (tsType.IsT0 && tsType.AsT0.IsPrimitive)
                        {
                            var t0 = tsType.AsT0;
                            if (t0.IsPrimitive)
                            {
                                t0.DefaultValue = "null";
                                t0.Name = t0.Name.EndsWith("null") ? t0.Name : t0.Name + " | null";
                            }
                        }

                        return tsType;

                        // return asNullable;

                    }
                }

                var id = type.FullName;
                if (tsTypes.ContainsKey(id))
                {
                    return tsTypes[id];
                }

                TsType GetAsGenericTsDiscriminatedUnion(Type type, string duTypeName, List<string> genericArguments)
                {
                    var genericInfos = $"<{string.Join(",", genericArguments)}>";
                    // generic stuff
                    var cases = FSharpType.GetUnionCases(type, FSharpOption<BindingFlags>.None);
                    var duCases = cases.Select(v => new DuCase()
                    {
                        Name = @$"{duTypeName}_Case_{v.Name}{genericInfos}",
                        CaseName = v.Name,
                        IsNull = v.Name == "None",
                        Fields = v.GetFields()
                            .Select(v => CreateOrGet(v.PropertyType))
                            .Select(v => v.AsT0)
                            .ToArray()
                    });

                    var tsType = new TsDiscriminatedUnion()
                    {
                        Id = type.FullName,
                        IsGeneric = true,
                        Name = $"{duTypeName}",
                        NameWithGenericArguments = $"{duTypeName}{genericInfos}",
                        FullName = type.FullName,
                        Type = type,
                        Cases = duCases,
                        Namespace = type.Namespace,
                        Properties = new List<Property>(),
                        DefaultValue = duCases.Any(v => v.IsNull) ? "null" : null // @"{ Case: ""None"" }",
                    };
                    return tsType;
                }

                var genericArguments = type.GetGenericArguments();
                var duTypeName = type.Name;
                if (genericArguments.Length > 0)
                {
                    var genericTsType = CreateOrGet(genericArguments.First());
                    var genericTsTypeName = CreateOrGet(genericArguments.First()).Match(v => v.Name, v => v.Name).Replace(" | null", "");
                    duTypeName = duTypeName.Replace("`1", $"_{genericTsTypeName}");
                }

                if (!isGenericType)
                {
                    //not yet supported
                    return TsType.Any();
                }
                if (isGenericType)
                {
                    var genericTypeDefinition = type.GetGenericTypeDefinition();

                    if (type.IsGenericType && genericTypeDefinition == typeof(FSharpOption<>))
                    {
                        //is generic
                        var tsType = GetAsGenericTsDiscriminatedUnion(genericTypeDefinition, type.Name.Replace("`1", ""), new List<string>(new[] { "T" }));

                        var args = type.GetGenericArguments();
                        var argumentType = args.First();
                        var argumentTsType = CreateOrGet(argumentType);

                        if (argumentTsType.IsT1)
                        {
                            Console.WriteLine("Discriminated union with generic enum argument is not yet supported");
                            return TsType.Any();
                        }

                        tsType.GenericArgumentsTsTypes.Add(argumentTsType.AsT0);

                        tsTypes.TryAdd(tsType.Id, tsType);
                        return tsType;
                    }
                    else
                    {
                        var tsType = GetAsGenericTsDiscriminatedUnion(type, duTypeName, new List<string>());
                        tsTypes.Add(tsType.Id, tsType);
                        return tsType;
                    }
                }
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
                    return AddAsEnumerable(type);
                }

                var id = type.GetTypeId();
                if (!tsTypes.ContainsKey(id))
                {
                    if (IsNullable(type))
                    {
                        OneOf<TsType, TsEnum> nullable = AsNullable(type);

                        nullable.Switch(tsType =>
                        {
                            tsType.Id = id;
                            tsTypes.TryAdd(id, tsType);
                            PopuplateProperties(tsType);
                        }, v1 =>
                        {
                            tsEnums.TryAdd(v1.Id, v1);
                        });
                        return nullable;
                    }
                    else
                    {
                        // if (type.IsStruct())
                        // {
                        //     return TsType.Any();
                        // }

                        if (type.Namespace == null)
                        {
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

        private TsType AsEnumerable(Type type)
        {
            Type elementType = type.GetCollectionElementType();

            OneOf<TsType, TsEnum> elementTsType =
                elementType == null ? TsType.Any() : CreateOrGet(elementType, skipDependencies: false);

            var elementTypeNamespace = elementTsType.Match(v1 => v1.Namespace, v2 => v2.Namespace);
            var id = type.GetTypeId();

            var name = elementTsType.IsT0 && elementTsType.AsT0.IsGeneric && elementTsType.AsT0 is TsDiscriminatedUnion du
                ? du.NameWithGenericArguments.Replace("<T>", $"<{du.GenericArgumentsTsTypes.First().Name}>")
                : elementTsType.Match(v1 => v1.Name, v2 => v2.Name);

            List<TsType> genericArgumentTypes = elementTsType.IsT0 ? new[] { elementTsType.AsT0 }.ToList() : new();
            return new TsType
            {
                Id = id,
                Name = name + "[]",
                Namespace = elementTypeNamespace,
                DefaultValue = "[]",
                GenericArgumentsTsTypes = genericArgumentTypes,
                Properties = new List<Property>(),
                IsCollection = true,
                IsGeneric = true,
                IsPrimitive = elementTsType.Match(v1 => v1.IsPrimitive, v2 => true),
                AdditionalDependencies = new List<Dependency>()
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
                throw new CodeGenerationNotSupported("Could not get primitive for " + type.FullName + " / element type = " +
                                                     elementType.FullName + " " + e.Message);
                // throw new Exception("Could not get primitive for " + type.FullName + " / element type = " +
                //                     elementType.FullName + " " + e.Message);
            }
        }

        private TsType GetPrimitiveAsNullable(Type type)
        {
            if (!GetTypeExtension.primitives.ContainsKey(type))
            {
                throw new CodeGenerationNotSupported($"Type '{type.FullName}' not a primitive");
                // throw new Exception($"Type '{type.FullName}' not a primitive");
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
            try
            {
                type.Properties = type.PropertyInfos?
                    .DistinctBy(v => v.Name)
                    .Select(v =>
                    {
                        OneOf<TsType, TsEnum> tsType = CreateOrGet(v.PropertyType, skipDependencies: true);

                        var defaultValue = tsType.Match(v1 => v1.DefaultValue, v2 =>
                        {
                            return "default" + v2.Name;
                        });
                        var typeName = tsType.Match(v1 =>
                        {
                            if (v1.Name == "any") { return "any"; }

                            return v1.Name;
                        }, v2 => v2.Name);
                        var isNullable = tsType.Match(v1 => v1.IsNullable /*v1.DefaultValue == "null"*/, v2 => v2.IsNullable);
                        return new Property
                        {
                            PropertyName = v.Name.CamelCase(),
                            DefaultValue = defaultValue,
                            TsType = tsType,
                            IsNullable = isNullable,
                            TypeName = tsType.IsT0 && tsType.AsT0.Namespace == "System" ? "any" : typeName,
                            IsCyclic = tsType.IsT0 && visited.Contains(tsType.AsT0),
                        };
                    }).ToList();
            }
            catch (Exception e)
            {
                throw new CodeGenerationNotSupported(e.Message);
            }
        }

        private bool IsNullable(Type type)
        {
            return type.Name.StartsWith("Nullable");
        }

        private OneOf<TsType, TsEnum> AsNullable(Type type)
        {
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
                var t = new TsType
                {
                    FullName = type.FullName,
                    IsPrimitive = type.Name.StartsWith("Nullable") || type.Name.StartsWith("FSharpOption"),
                    Name = name,
                    Namespace = type.Namespace,
                    DefaultValue = null,
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