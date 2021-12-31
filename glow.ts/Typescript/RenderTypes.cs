using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneOf;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Glow.Core.Typescript
{
    public static class RenderTypes
    {
        public static void ToDisk(TypeCollection types, string path)
        {
            if (!path.EndsWith("/"))
            {
                throw new ArgumentException("path must end with /");
            }

            Directory.CreateDirectory(path);

            IEnumerable<Module> modules = types.Modules;

            foreach (Module v in modules)
            {
                RenderModule(v, path);
            }
        }

        private static void RenderModule(Module module, string path)
        {
            if (module.Namespace.StartsWith("System"))
            {
                return;
            }
            IEnumerable<TsEnum> enumerables = module.TsEnums;
            var tsTypes = module.TsTypes;
            if (enumerables.Count() == 0 && tsTypes.Count() == 0)
            {
                File.WriteAllText(path + module.Namespace + ".ts", @"export type NeedToExportSomething = {}");

                return;
            }

            var builder = new StringBuilder();

            IEnumerable<IGrouping<string, Dependency>> dependencies = module.GetDependenciesGroupedByNamespace();

            builder.AppendLine(@"/* eslint-disable prettier/prettier */");

            foreach (IGrouping<string, Dependency> group in dependencies)
            {
                builder.AppendLine(
                    $"import {{ {string.Join(", ", group.Select(v => v.Name.Replace("[]", "")))} }} from \"./{group.Key}\"");
                builder.AppendLine(
                    $"import {{ {string.Join(", ", group.Where(v => v != null).Select(v => "default" + v.Name.Replace("[]", "")))} }} from \"./{group.Key}\"");
            }

            if (dependencies.Count() != 0)
            {
                builder.AppendLine("");
            }

            foreach (TsEnum v in enumerables)
            {
                RenderTsEnum(v, builder);
            }

            foreach (TsType tsType in tsTypes)
            {
                if (!tsType.IsCollection)
                {
                    RenderTsType(tsType, builder);
                }
            }

            File.WriteAllText(path + module.Namespace + ".ts", builder.ToString());
        }

        private static void RenderTsEnum(TsEnum type, StringBuilder builder)
        {
            var name = type.Name;
            builder.AppendLine($"export type {name} = {string.Join(" | ", type.Values.Select(v => $@"""{v}"""))}");
            builder.AppendLine($@"export const default{name} = ""{type.DefaultValue ?? "NULL"}""");
            builder.AppendLine($@"export const {name}Values: {{ [key in {name}]: {name} }} = {{");
            foreach (var v in type.Values)
            {
                builder.AppendLine($@"  {v}: ""{v}"",");
            }

            builder.AppendLine("}");
            builder.AppendLine($@"export const {name}ValuesArray: {name}[] = Object.keys({name}Values) as {name}[]");

            builder.AppendLine("");
        }

        private static void RenderTsType(TsType type, StringBuilder builder)
        {
            var name = type.Name;
            builder.AppendLine($"export interface {name} {{");
            if (type.Properties != null)
            {
                foreach (Property v in type.Properties)
                {
                    builder.AppendLine($"  {v.PropertyName}: {v.TypeName}{(v.IsNullable ? " | null" : "")}");
                }
            }

            builder.AppendLine("}");
            builder.AppendLine("");

            if (type.DefaultValue != null)
            {
                builder.AppendLine($"export const default{name}: {name} = {{");
                RenderProperties(type.Properties, builder, 0, 1);
                builder.AppendLine("}");
            }

            builder.AppendLine("");
        }

        private static void RenderProperties(List<Property> properties, StringBuilder builder, int depth, int maxDepth)
        {
            foreach (Property property in properties)
            {
                TsType tsType = property.TsType.IsT0 ? property.TsType.AsT0 : null;
                if (tsType?.IsCollection == true)
                {
                    builder.AppendLine($"  {property.PropertyName}: [],");
                }
                else if (tsType?.IsPrimitive != true)
                {
                    builder.AppendLine($"  {property.PropertyName}: {{}} as any,");
                }
                else if (tsType != null && !tsType.IsPrimitive && tsType.HasCyclicDependency && !tsType.IsCollection)
                {
                    if (depth >= maxDepth)
                    {
                        builder.Append("".PadRight(depth));

                        builder.AppendLine($"  {property.PropertyName}: null as any,");
                    }
                    else
                    {
                        builder.Append("".PadRight(depth));

                        builder.AppendLine($"  {property.PropertyName}: {{");

                        RenderProperties(property.TsType.AsT0.Properties, builder, depth + 1, maxDepth);

                        builder.Append("  ".PadRight(depth));

                        builder.AppendLine("},");
                    }
                }
                else
                {
                    builder.Append("".PadRight(depth));

                    builder.AppendLine($"  {property.PropertyName}: {property.DefaultValue ?? "null"},");
                }
            }
        }
    }
}