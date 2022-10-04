using System.Text;
using Glow.TypeScript;

namespace Glow.Core.Typescript
{
    public static class RenderTypes
    {
        public static void ToDisk(TypeCollection types, string path, TsGenerationOptions? options)
        {
            Console.WriteLine("");
            Console.WriteLine("Render to path " + path);

            if (!path.EndsWith("/"))
            {
                throw new ArgumentException("path must end with /");
            }

            Directory.CreateDirectory(path);

            IEnumerable<Module> modules = types.Modules;

            foreach (Module v in modules)
            {
                RenderModule(v, path, options?.ApiOptions);
            }
        }

        public static void RenderModule(Module module, string path, ApiOptions? options)
        {
            var content = RenderModule(module, options);
            if (content != null)
            {
                File.WriteAllText(path + module.Namespace + ".ts", content);
            }
        }

        public static string RenderModule(Module module, ApiOptions? options)
        {
            Console.WriteLine("Render " + module.Namespace);

            if (module.Namespace == null)
            {
                Console.WriteLine($"Namespace of module '{string.Join(", ", module.TsTypes.Select(v => v.Id))}' is null");
                return null;
            }

            if (module.Namespace.StartsWith("System"))
            {
                return null;
            }

            IEnumerable<TsEnum> enumerables = module.TsEnums;
            List<TsType> tsTypes = module.TsTypes;
            if (enumerables.Count() == 0 && tsTypes.Count() == 0)
            {
                return @"export type NeedToExportSomething = {}";
            }

            var builder = new StringBuilder();

            IEnumerable<IGrouping<string, Dependency>> dependencies = module.GetDependenciesGroupedByNamespace();

            if (options?.ApiFileFirstLines != null)
            {
                foreach (var line in options.ApiFileFirstLines)
                {
                    builder.AppendLine(line);
                }
            }

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
                if (tsType.GetType() == typeof(TsDiscriminatedUnion))
                {
                    continue;
                }

                if (!tsType.IsCollection)
                {
                    RenderTsType(tsType, builder);
                }
            }

            foreach (TsType tsType in tsTypes)
            {
                if (tsType.Name.Contains("CircularStatus"))
                {
                }

                if (tsType.GetType() == typeof(TsDiscriminatedUnion))
                {
                    if (!tsType.IsCollection)
                    {
                        RenderTsType(tsType, builder);
                    }
                }
            }

            return builder.ToString();
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
            if (type.GetType() == typeof(TsDiscriminatedUnion))
            {
                RenderTsDiscriminatedUnion(type as TsDiscriminatedUnion, builder);
                return;
            }

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
                RenderProperties(type.Properties, builder, 0, 1, type.Name);
                builder.AppendLine("}");
            }

            builder.AppendLine("");
        }

        private static void RenderTsDiscriminatedUnion(TsDiscriminatedUnion type, StringBuilder builder)
        {
            foreach (DuCase v in type.Cases)
            {
                var caseTypeName = $"{type.Name}_Case_{v.Name}";
                // Console.WriteLine("Handle DU " + type.Namespace + " " + type.Name);
                if (v.Fields.Length > 1)
                {
                    // Console.WriteLine("Field length > 1, append");

                    builder.AppendLine(
                        @$"export type {caseTypeName} = {{
  Case: ""{v.CaseName}"",
  Fields: {{ {string.Join(", ", v.Fields.Select((v, i) => $"Item{i}: {v.Name}"))} }}
}}");

                    builder.AppendLine(
                        @$"export const default{caseTypeName}: {caseTypeName} = {{
  Case: ""{v.CaseName}"",
  Fields: {{ {string.Join(", ", v.Fields.Select((v, i) => $"Item{i}: {v.DefaultValue}"))} }}
}}");
                }
                else if (v.IsNull)
                {
                    // Console.WriteLine("v.IsNull");

                    builder.AppendLine(@$"export type {v.Name} = null");
                }
                else if (v.Fields.Length == 0)
                {
                    // Console.WriteLine("Field length = 0, append");

                    builder.AppendLine(
                        @$"export type {caseTypeName} = {{
  Case: ""{v.CaseName}""
}}");
                }
                else if (v.Fields.Length == 1)
                {
                    // Console.WriteLine("Field length = 1, append");

                    TsType field = v.Fields.First();
                    builder.AppendLine(
                        @$"export type {caseTypeName} = {{
  Case: ""{v.CaseName}"",
  Fields: {field.Name}
}}");
                    builder.AppendLine(
                        @$"export const default{caseTypeName}: {caseTypeName} = {{
  Case: ""{v.CaseName}"",
  Fields: {field.DefaultValue}
}}");
                }
                else
                {
                    // Console.WriteLine("SKIP°!!!");
                }

                builder.AppendLine();


//                 if (!type.IsGeneric)
//                 {
//                     var fields = v.Fields.FirstOrDefault()?.DefaultValue;
//                     builder.AppendLine(
//                         @$"export const default{caseTypeName}: {caseTypeName} = {{
//   case: ""{v.CaseName}"",
// {(fields != null ? $"  fields: {fields}" : "")}
// }}"
//                     );
//                 }
            }

            builder.AppendLine();
            if (type.IsGeneric)
            {
                var duRoot = $"export type {type.NameWithGenericArguments} = " + string.Join(" | ", type.Cases.Select(v => v.Name));

                builder.AppendLine(duRoot);
            }
            else
            {
                var duRoot = $"export type {type.Name} = " + string.Join(" | ", type.Cases.Select(v => $"{type.Name}_Case_{v.Name}"));

                builder.AppendLine(duRoot);
            }

            builder.AppendLine();

            if (type.DefaultValue != null)
            {
                builder.AppendLine("");
                builder.AppendLine($"export const default{type.Name} = {type.DefaultValue}");
                builder.AppendLine("");
            }

            // builder.AppendLine($@"export const default{type.Name}: {type.Name} = default{type.Cases.First().Name}");
        }

        private static void RenderProperties(List<Property> properties, StringBuilder builder, int depth, int maxDepth, string typeName)
        {
            if (properties == null)
            {

                builder.Append(" // skipped rendering properties (null)");
                builder.AppendLine("");
                Console.WriteLine($"skip rendering properties (is null). This should usually not happen. Type = {typeName}");
                return;
            }

            foreach (Property property in properties)
            {
                TsType tsType = property.TsType.IsT0 ? property.TsType.AsT0 : null;
                if (tsType?.IsCollection == true)
                {
                    builder.AppendLine($"  {property.PropertyName}: [],");
                }
                else if (tsType?.IsPrimitive != true)
                {
                    if (tsType?.GetType() == typeof(TsDiscriminatedUnion) && tsType?.DefaultValue != null)
                    {
                        builder.AppendLine($"  {property.PropertyName}: {property.DefaultValue},");
                    }
                    else
                    {
                        builder.AppendLine($"  {property.PropertyName}: {{}} as any,");
                    }
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

                        RenderProperties(property.TsType.AsT0.Properties, builder, depth + 1, maxDepth, typeName);

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