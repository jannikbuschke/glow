using System.Text;
using Glow.Core.Actions;
using Glow.Ts.Events;
using Glow.TypeScript;
using OneOf;

namespace Glow.Core.Typescript
{
    public static class RenderSubscrptions
    {
        public static void Render(TypeCollection types, TsGenerationOptions options)
        {
            if (!options.GenerateSubscriptions)
            {
                Console.WriteLine("Skip generate subscriptions for " + options.Path);
                return;
            }

            var path = options.GetPath();
            var events = options.Assemblies.GetEvents();

            var glowPath = options.Assemblies.FirstOrDefault()?.FullName.StartsWith("Glow.Core") == true
                ? ".."
                : "glow-core";
            var useSubmitPath = options.Assemblies.FirstOrDefault()?.FullName.StartsWith("Glow.Core") == true
                ? ".."
                : "glow-core";

            var imports = new StringBuilder();

            if(options.ApiOptions!=null)
            {
                foreach (var v in options.ApiOptions?.ApiFileFirstLines)
                {
                    imports.AppendLine(v);
                }
            }

            imports.AppendLine($"// Assembly: {options.Assemblies.FirstOrDefault()?.FullName}");

            imports.AppendLine(@"import * as React from ""react""");

            imports.AppendLine(@"import { Form } from ""formik-antd""");
            imports.AppendLine(@"import mitt, { Handler, WildcardHandler } from ""mitt""");

            imports.AppendLine(@$"import {{ useNotification, useWildcardNotification }} from ""{glowPath}/lib/notifications/type-notifications""");
            // imports.AppendLine(@"import * as emitt from "mitt");

            var modules = types.Modules;
            foreach (var v in modules)
            {
                if (v.Namespace == null)
                {
                    foreach (var t in v.TsTypes)
                    {
                        Console.WriteLine(t.Name);
                    }
                    Console.WriteLine($"Namespace is null {{types count = {v.TsTypes.Count}, enums count = {v.TsEnums.Count()}}}");

                    // throw new Exception("Namespace is null " + options.Path);
                    continue;
                }

                if (!v.Namespace.StartsWith("System.") && v.Namespace != "System")
                {
                    imports.AppendLine(
                        $"import * as {v.Namespace.Replace(".", "_")} from \"./{v.Namespace}\"");
                }
            }

            var tsE = events.Select(e => types.Modules.SelectMany(v => v.TsTypes).FirstOrDefault(v => v.Type == e));

            imports.AppendLine("");

            imports.AppendLine("export type Events = {");
            foreach (var VARIABLE in tsE)
            {
                imports.AppendLine($"  '{VARIABLE.FullName}': {VARIABLE.Namespace.Replace(".", "_")}.{VARIABLE.Name},");
            }
            imports.AppendLine("}");

            imports.AppendLine(@"
// export const emitter = mitt<Events>();

type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
}

export type EventTable = TagWithKey<""url"", Events>

export function useSubscription<EventName extends keyof EventTable>(
    name: EventName,
    callback: Handler<Events[EventName]>,
    deps?: any[],
    ) {
    const ctx = useNotification<EventName, Events>(name, callback, deps)
    return ctx
}

export function useSubscriptions(
  callback: WildcardHandler<Events>,
  deps?: any[],
) {
  const ctx = useWildcardNotification<Events>(callback, deps)
  return ctx
}


");

            File.WriteAllText(path + "subscriptions.tsx", imports.ToString());

            Console.WriteLine("Rendered supscriptions " + options.Path);
        }
    }
}
