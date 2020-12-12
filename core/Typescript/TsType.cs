using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Glow.TypeScript;

namespace Glow.Core.Typescript
{
    public class TypeCollection
    {
        private readonly IList<Type> types = new List<Type>();
        private readonly Dictionary<string, TsType> tsTypes = new Dictionary<string, TsType>();

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

        public Dictionary<string, TsType> Generate()
        {
            foreach (Type type in types)
            {
                TsType result = CreateOrGet(type);
            }
            return tsTypes;
        }

        private TsType CreateOrGet(Type type)
        {
            if (IsPrimitive(type))
            {
                TsType valueType = GetPrimitive(type);
                if (valueType != null)
                {
                    return valueType;
                }
            }

            //if (IsDictionary(type))
            //{
            //    return AsDictionary(type);
            //}

            if (IsEnumerableType(type))
            {
                return AsEnumerable(type);
            }

            if (!tsTypes.ContainsKey(type.FullName))
            {
                tsTypes.Add(type.FullName, Create(type));
            }
            return tsTypes[type.FullName];
        }

        private TsType AsEnumerable(Type type)
        {
            Type[] args = type.GenericTypeArguments;
            TsType argTsType = CreateOrGet(args.First());
            return new TsType
            {
                Name = argTsType.Name+"[]",
                DefaultValue = "[]",
            };
        }

        private Tuple<string, string> AsDictionary(Type type)
        {
            TsType keyTsType = GetPrimitive(type.GenericTypeArguments[0]);
            
            return new Tuple<string, string>
            (
                $"{{ [key: {keyTsType.Name}]: {CreateOrGet(type).Name} }}",
                "{}"
            );
        }

        private bool IsDictionary(Type t)
        {
            var isDict = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
            return isDict;
        }

        private bool IsEnumerableType(Type type)
        {
            return (type.GetInterface(nameof(IEnumerable)) != null);
        }

        private bool IsPrimitive(Type type)
        {
            return primitives.ContainsKey(type);
        }

        private TsType GetPrimitive(Type type)
        {
            if (primitives.ContainsKey(type))
            {
                (var name, var defaultValue) = primitives[type];
                return new TsType
                {
                    Name = name,
                    DefaultValue = defaultValue,
                    IsPrimitive = true
                };
            }
            if (IsDictionary(type))
            {
                (var name, var defaultValue) = primitives[type];
                return new TsType
                {
                    Name = name,
                    DefaultValue = defaultValue,
                    IsPrimitive = true
                };
            }
            return null;
        }

        private readonly Dictionary<Type, Tuple<string, string>> primitives = new Dictionary<Type, Tuple<string, string>>
        {
            { typeof(string), new Tuple<string, string>("string | null", "null") },
            { typeof(double), new Tuple<string, string>("number", "0") },
            { typeof(double?), new Tuple<string, string>("number | null", "null") },
            { typeof(float), new Tuple<string, string>("number", "0") },
            { typeof(float?), new Tuple<string, string>("number | null", "null") },
            { typeof(int), new Tuple<string, string>("number","0") },
            { typeof(int?), new Tuple<string, string>("number | null", "null") },
            { typeof(decimal), new Tuple<string, string>("number", "0") },
            { typeof(decimal?), new Tuple<string, string>("number | null", "null") },
            { typeof(DateTime), new Tuple<string, string>("string", @"""1/1/0001 12:00:00 AM""") },
            { typeof(DateTime?), new Tuple<string, string>("string | null", "null") },
            { typeof(Guid), new Tuple<string, string>("string", @"""00000000-0000-0000-0000-000000000000""") },
            { typeof(Guid?),new Tuple<string, string>( "string | null", "null") },
            { typeof(bool),new Tuple<string, string>( "boolean", "false") },
            { typeof(bool?), new Tuple<string, string>("boolean | null", "null") },
            { typeof(Dictionary<string, string>), new Tuple<string, string>("{ [key: string]: string }", "{}") },
            { typeof(Dictionary<string, int>), new Tuple<string, string>("{ [key: string]: number }", "{}") },
            { typeof(Dictionary<string, object>), new Tuple<string, string>("{ [key: string]: any }", "{}") },
            { typeof(object), new Tuple<string, string>("any", "null") }
        };

        private TsType Create(Type type)
        {
            var name = type.GenericTypeArguments.Length != 0
                ? type.Name.Replace(".","").Replace("`","") + string.Join("", type.GenericTypeArguments.Select(v => v.Name))
                : type.Name;

            var value = new TsType
            {
                FullName = type.FullName,
                Name = name,
                Namespace = type.Namespace,
                DefaultValue = "default"+name,
                Type = type,
                Properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(v => v.Name.CamelCase(), v => CreateOrGet(v.PropertyType))
            };
            return value;

            //type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ForEach(v =>
            //{
            //    builder.AppendLine($"  {v.Name.CamelCase()}: {v.PropertyType.ToTsType()}");
            //});

            //foreach (PropertyInfo v in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //{
            //    //var value = v.PropertyType.DefaultValue();// v.PropertyType.IsValueType ? Activator.CreateInstance(v.PropertyType) : "null";


            //    //builder.AppendLine($"  {v.Name.CamelCase()}: {value ?? "null"},");
            //}
        }
    }

    public class TsType
    {
        public string Id
        {
            get
            {
                return FullName;
            }
        }
        public bool IsPrimitive { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public Type Type { get; set; }

        public string DefaultValue { get; set; }

        public IEnumerable<TsType> Dependencies
        {
            get { return Properties.Values; }
        }

        public Dictionary<string, TsType> Properties { get; set; }
    }

    public static class Render
    {
        public static void ToDisk(Dictionary<string, TsType> types, string path)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"/* this file is autogenerated. Do not edit. */");
            builder.AppendLine("");

            foreach ((var key, TsType tsType) in types)
            {
                RenderTsType(tsType, builder);
            }

            //builder.AppendLine("export declare module Entities {");

            //builder.AppendLine(entities.ToString());

            //builder.AppendLine($"  export type All = {string.Join(" | ", allEntityNames)}");
            //builder.AppendLine("}");
            builder.Insert(0, "\r\n");

            //builder.Insert(0, "/* eslint-disable prettier/prettier */");
            var text = builder.ToString();
            File.WriteAllText(path, text);
        }

        private static void RenderTsType(TsType type, StringBuilder builder)
        {
            //var name = type.GenericTypeArguments.Length == 0
            //    ? type.Name.Replace(".", "")
            //    : type.GenericTypeArguments.Length == 1
            //        ? type.Name
            //            .Replace("`1", type.GenericTypeArguments[0].FullName)
            //            .Replace(".", "")
            //        : type.Name
            //            .Replace("`2", type.GenericTypeArguments[0].FullName + type.GenericTypeArguments[1].FullName)
            //            .Replace(".", "");
            //var fullName = type.FullName.Replace(".", "");

            var name = type.Name;
            builder.AppendLine($"export interface {name} {{");
            foreach (KeyValuePair<string, TsType> v in type.Properties)
            {
                builder.AppendLine($"  {v.Key}: {v.Value.Name}");
            }

            builder.AppendLine("}");
            builder.AppendLine("");

            builder.AppendLine($"export const default{name}: {name} = {{");
            foreach ((var propertyName, TsType tsType) in type.Properties)
            {
                builder.AppendLine($"  {propertyName}: {tsType.DefaultValue ?? "null"},");
            }
            builder.AppendLine("}");
            builder.AppendLine("");
        }
    }
}
