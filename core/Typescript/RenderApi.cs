using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper.Internal;
using Glow.Core.Actions;
using Glow.Core.Linq;
using Glow.TypeScript;

namespace Glow.Core.Typescript
{
    public static class RenderApi
    {
        public static void Render(TypeCollection types, TsGenerationOptions options)
        {
            if (!options.GenerateApi)
            {
                Console.WriteLine("Skip generate api for " + options.Path);
                return;
            }

            var path = options.GetPath();
            IEnumerable<ActionMeta> actions = options.Assemblies.GetActions();

            var imports = new StringBuilder();
            imports.AppendLine(@"import { QueryOptions } from ""react-query"";");
            imports.AppendLine(@"import { useApi, ApiResult } from ""glow-react"";");
            imports.AppendLine(@"import { useAction, useSubmit, UseSubmit } from ""glow-react/es/Forms/use-submit"";");

            var modules = types.Modules;
            foreach (var v in modules)
            {
                if (v.Namespace == null)
                {
                    foreach (var t in v.TsTypes)
                    {
                        Console.WriteLine(t.Name);
                    }
                    throw new Exception("Namespace is null " + options.Path);
                    continue;
                }
                imports.AppendLine(
                    $"import * as {v.Namespace.Replace(".", "_")} from \"./{v.Namespace}\"");
            }

            IEnumerable<Dependency> dependencies = types
                .Modules
                .SelectMany(v => v.GetDependencies())
                .DistinctBy(v => v.Id);

            imports.AppendLine("");

            var queryInputs = new StringBuilder();
            var queryOutputs = new StringBuilder();
            var actionInputs = new StringBuilder();
            var actionOutputs = new StringBuilder();

            queryInputs.AppendLine("type QueryInputs = {");
            queryOutputs.AppendLine("type QueryOutputs = {");
            actionInputs.AppendLine("type Actions = {");
            actionOutputs.AppendLine("type Outputs = {");

            string GetTypeName(Type type)
            {
                if (!types.Exists(type))
                {
                    Console.WriteLine("Type does not exist " + type + " using any");
                    return "any";
                }

                var tsType = types.Find(type);
                var name = tsType.Match(v1 => v1.Name, v2 => v2.Name);
                var nameSpace = tsType.Match(v1 => v1.Namespace, v2 => v2.Namespace).Replace(".", "_");
                // Console.WriteLine("name=" + name);
                return nameSpace + "." + name;
            }

            foreach (var v in actions)
            {
                // Console.WriteLine(v.ActionAttribute.Route);
                var outputTypeName = GetTypeName(v.Output);
                var inputTypeName = GetTypeName(v.Input);

                if (v.Input.Name.StartsWith("Get"))
                {
                    queryOutputs.AppendLine($@"  ""/{v.ActionAttribute.Route}"": {outputTypeName},");
                    queryInputs.AppendLine($@"  ""/{v.ActionAttribute.Route}"": {inputTypeName},");
                }
                else
                {
                    actionOutputs.AppendLine($@"  ""/{v.ActionAttribute.Route}"": {outputTypeName},");
                    actionInputs.AppendLine($@"  ""/{v.ActionAttribute.Route}"": {inputTypeName},");
                }
            }
            queryInputs.AppendLine("}");
            queryOutputs.AppendLine("}");
            actionOutputs.AppendLine("}");
            actionInputs.AppendLine("}");

            actionInputs.AppendLine(@"
type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
};

type ActionTable = TagWithKey<""url"", Actions>;

type Unionize<T> = T[keyof T];

type MyActions = Unionize<ActionTable>;

export function useTypedAction<ActionName extends keyof ActionTable>(key: ActionName): UseSubmit<Actions[ActionName], Outputs[ActionName]>{
  const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
  return s
}

type QueryTable = TagWithKey<""url"", QueryInputs>;

export function useTypedQuery<ActionName extends keyof QueryTable>(key: ActionName, {
    placeholder,
    input,
    queryOptions
  }: {
    placeholder: QueryOutputs[ActionName],
    input:  QueryInputs[ActionName]
    queryOptions?: QueryOptions<QueryOutputs[ActionName]>
  }): ApiResult<QueryOutputs[ActionName]> {

  const { data, ...rest} = useApi({
    url: key,
    method:""POST"",
    payload: input,
    // todo: find defaultPlaceholder
    placeholder: placeholder,
    queryOptions: queryOptions
  })

  const result = data as QueryOutputs[ActionName]

  return { data: result, ...rest} as any
}



");

            File.WriteAllText(path + "api.ts", imports.ToString());
            File.AppendAllText(path + "api.ts", queryInputs.ToString());
            File.AppendAllText(path + "api.ts", queryOutputs.ToString());
            File.AppendAllText(path + "api.ts", actionOutputs.ToString());
            File.AppendAllText(path + "api.ts", actionInputs.ToString());

            Console.WriteLine("Rendered api " + options.Path);
        }
    }
}