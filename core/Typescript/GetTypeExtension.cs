using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Glow.Core.Typescript
{
    public static class GetTypeExtension
    {
        public static string GetTypeId(this Type type)
        {
            return type.FullName ?? (type.IsGenericParameter ? "T" : null);
        }

        public static bool IsDictionary(this Type t)
        {
            var isDict = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
            return isDict;
        }

        public static bool IsEnumerableType(this Type type)
        {
            return (type.GetInterface(nameof(IEnumerable)) != null);
        }

        public static bool IsPrimitive(this Type type)
        {
            return primitives.ContainsKey(type);
        }

        // public static IEnumerable<Type> BaseClasses(this Type type)
        // {
        //     if (type.BaseType == null)
        //     {
        //         return Enumerable.Empty<Type>();
        //     }
        //     else
        //     {
        //         var b = Enumerable.Repeat(type.BaseType, 1);
        //         return b.Concat(type.BaseClasses());
        //     }
        // }

        public static Type GetCollectionElementType(this Type type)
        {
            var name = type.FullName;
            Type[] args = type.GenericTypeArguments;

            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (args.Length == 0)
            {
                var collectionInterface = (type.GetInterface(typeof(IList<>).FullName) ??
                                           type.GetInterface(typeof(ICollection<>).FullName) ??
                                           type.GetInterface(typeof(IEnumerable<>).FullName)
                                           ?? (type.BaseType == typeof(List<>) ? type.BaseType : null)
                    );

                if (collectionInterface == null)
                {
                    var isList = type.IsSubclassOf(typeof(List<>));
                    var isCollection = type.IsSubclassOf(typeof(Collection<>));
                    var iss = type.GetInterfaces();
                    Console.WriteLine("Could not find Elementtype for " + type.FullName + ". Interfaces are:");
                    foreach (var v in iss)
                    {
                        Console.WriteLine(v.FullName);
                    }
                }

                var elementType = collectionInterface?.GenericTypeArguments?.FirstOrDefault();
                return elementType;
            }
            else
            {
                return args[0];
            }
        }

        public static readonly Dictionary<Type, Tuple<string, string>> primitiveCollection =
            new Dictionary<Type, Tuple<string, string>>
            {
                {typeof(string), new Tuple<string, string>("(string | null)[]", "[]")},
                {typeof(int), new Tuple<string, string>("number[]", "[]")},
                {typeof(double), new Tuple<string, string>("number[]", "[]")},
                {typeof(bool), new Tuple<string, string>("boolean[]", "[]")},
            };

        public static readonly Dictionary<Type, Tuple<string, string>> primitives =
            new Dictionary<Type, Tuple<string, string>>
            {
                {typeof(string), new Tuple<string, string>("string | null", "null")},
                {typeof(double), new Tuple<string, string>("number", "0")},
                {typeof(double?), new Tuple<string, string>("number | null", "null")},
                {typeof(float), new Tuple<string, string>("number", "0")},
                {typeof(float?), new Tuple<string, string>("number | null", "null")},
                {typeof(int), new Tuple<string, string>("number", "0")},
                {typeof(int?), new Tuple<string, string>("number | null", "null")},
                {typeof(short), new Tuple<string, string>("number", "0")},
                {typeof(short?), new Tuple<string, string>("number | null", "null")},
                {typeof(long), new Tuple<string, string>("number", "0")},
                {typeof(long?), new Tuple<string, string>("number | null", "null")},
                {typeof(decimal), new Tuple<string, string>("number", "0")},
                {typeof(decimal?), new Tuple<string, string>("number | null", "null")},
                {typeof(DateTime), new Tuple<string, string>("string", @"""1/1/0001 12:00:00 AM""")},
                {typeof(DateTime?), new Tuple<string, string>("string | null", "null")},
                {typeof(DateTimeOffset), new Tuple<string, string>("string", @"""00:00:00""")},
                {typeof(DateTimeOffset?), new Tuple<string, string>("string | null", "null")},
                {typeof(Guid), new Tuple<string, string>("string", @"""00000000-0000-0000-0000-000000000000""")},
                {typeof(Guid?), new Tuple<string, string>("string | null", "null")},
                {typeof(bool), new Tuple<string, string>("boolean", "false")},
                {typeof(bool?), new Tuple<string, string>("boolean | null", "null")},
                {typeof(Dictionary<string, string>), new Tuple<string, string>("{ [key: string]: string }", "{}")},
                {typeof(Dictionary<string, decimal>), new Tuple<string, string>("{ [key: string]: number }", "{}")},
                {typeof(Dictionary<string, int>), new Tuple<string, string>("{ [key: string]: number }", "{}")},
                {typeof(Dictionary<string, object>), new Tuple<string, string>("{ [key: string]: any }", "{}")},
                {typeof(IDictionary<string, object>), new Tuple<string, string>("{ [key: string]: any }", "{}")},
                {typeof(object), new Tuple<string, string>("any", "null")},
                {typeof(byte[]), new Tuple<string, string>("string | null", "null")},
                {typeof(string[]), new Tuple<string, string>("(string | null)[]", "[]")},
                {typeof(List<string>), new Tuple<string, string>("(string | null)[]", "[]")},
                {typeof(IEnumerable<string>), new Tuple<string, string>("(string | null)[]", "[]")},
                {typeof(IEnumerable<Guid>), new Tuple<string, string>("string[]", "[]")},
                {typeof(IEnumerable<Guid?>), new Tuple<string, string>("(string | null)[]", "[]")},
                {typeof(IEnumerable<double>), new Tuple<string, string>("number[]", "[]")},
                {typeof(Collection<string>), new Tuple<string, string>("(string | null)[]", "[]")},
                {typeof(ICollection<string>), new Tuple<string, string>("(string | null)[]", "[]")},
            };
    }
}