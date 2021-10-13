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

            var glowPath = options.Assemblies.FirstOrDefault()?.FullName.StartsWith("Glow.Core") == true
                ? ".."
                : "glow-react";
            var useSubmitPath = options.Assemblies.FirstOrDefault()?.FullName.StartsWith("Glow.Core") == true
                ? ".."
                : "glow-react/es";

            var imports = new StringBuilder();

            imports.AppendLine(@"/* eslint-disable prettier/prettier */");
            imports.AppendLine(@"import * as React from ""react""");
            imports.AppendLine(@"import { QueryOptions } from ""react-query""");
            // allow adjusting?
            imports.AppendLine($@"import {{ useApi, ApiResult, notifySuccess, notifyError }} from ""{glowPath}""");
            imports.AppendLine(
                $@"import {{ useAction, useSubmit, UseSubmit, ProblemDetails }} from ""{useSubmitPath}/Forms/use-submit""");
            imports.AppendLine(@"import { Formik, FormikFormProps } from ""formik""");
            imports.AppendLine(@"import { Form } from ""formik-antd""");

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

                if (!v.Namespace.StartsWith("System."))
                {
                    imports.AppendLine(
                        $"import * as {v.Namespace.Replace(".", "_")} from \"./{v.Namespace}\"");
                }
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
            actionInputs.AppendLine("export type Actions = {");
            actionOutputs.AppendLine("export type Outputs = {");

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

//export function useTypedAction<ActionName extends keyof ActionTable>(key: ActionName): UseSubmit<Actions[ActionName], Outputs[ActionName]>{
//  const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
//  return s
//}

export type ActionTable = TagWithKey<""url"", Actions>

export type TypedActionHookResult<
  ActionName extends keyof ActionTable
> = UseSubmit<Actions[ActionName], Outputs[ActionName]>

export type TypedActionHook = <ActionName extends keyof ActionTable>(
  key: ActionName,
) => TypedActionHookResult<ActionName>

export const useTypedAction: TypedActionHook = <
  ActionName extends keyof ActionTable
>(
  key: ActionName,
) => {
  const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
  return s
}

// export function useTypedAction<ActionName extends keyof ActionTable>(
//   key: ActionName,
// ): UseSubmit<Actions[ActionName], Outputs[ActionName]> {
//   const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
//   return s
// }

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

export function TypedForm<ActionName extends keyof ActionTable>({
  initialValues,
  actionName,
  formProps,
  children,
  onSuccess,
  onError,
}: React.PropsWithChildren<{
  actionName: ActionName
  initialValues: Actions[ActionName]
  formProps?: FormikFormProps
  onSuccess?: (payload: Outputs[ActionName]) => void
  onError?: (error: ProblemDetails) => void
}>) {
  const [submit, validate] = useTypedAction<ActionName>(actionName)
  return (
    <Formik
      validate={validate}
      validateOnBlur={true}
      enableReinitialize={true}
      validateOnChange={false}
      initialValues={initialValues}
      onSubmit={async (values) => {
        const response = await submit(values)
        if (response.ok) {
          onSuccess && onSuccess(response.payload)
        } else {
          onError && onError(response.error)
          !onError && notifyError(response.error)
        }
      }}
    >
      {(f) => (
        <Form {...formProps}>
          {typeof children === ""function"" ? children(f) : children}
        </Form>
      )}
    </Formik>)
}
");

            File.WriteAllText(path + "api.tsx", imports.ToString());
            File.AppendAllText(path + "api.tsx", queryInputs.ToString());
            File.AppendAllText(path + "api.tsx", queryOutputs.ToString());
            File.AppendAllText(path + "api.tsx", actionOutputs.ToString());
            File.AppendAllText(path + "api.tsx", actionInputs.ToString());

            Console.WriteLine("Rendered api " + options.Path);
        }
    }
}