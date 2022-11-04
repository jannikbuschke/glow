using System.Text;
using Glow.Core.Actions;
using Glow.TypeScript;
using OneOf;

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
            imports.AppendLine(@"import { QueryOptions, UseQueryOptions } from ""react-query""");
            // allow adjusting?
            imports.AppendLine($@"import {{ useApi, ApiResult, useNotify }} from ""{glowPath}""");
            imports.AppendLine(
                $@"import {{ useAction, useSubmit, UseSubmit, ProblemDetails }} from ""{useSubmitPath}""");
            imports.AppendLine(@"import { Formik, FormikConfig, FormikFormProps, FormikProps } from ""formik""");
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

            IEnumerable<Dependency> dependencies = types
                .Modules
                .SelectMany(v => v.GetDependencies())
                .DistinctBy(v => v.Id);

            imports.AppendLine("");

            var queryInputs = new StringBuilder();
            var queryOutputs = new StringBuilder();
            var actionInputs = new StringBuilder();
            var actionOutputs = new StringBuilder();

            queryInputs.AppendLine("export type QueryInputs = {");
            queryOutputs.AppendLine("export type QueryOutputs = {");
            actionInputs.AppendLine("export type Actions = {");
            actionOutputs.AppendLine("export type Outputs = {");

            string GetTypeName(Type type)
            {
                if (!types.Exists(type))
                {
                    Console.WriteLine("Type does not exist " + type + " using any");
                    return "any";
                }

                OneOf<TsType, TsEnum> tsType = types.Find(type);

                var fullName = tsType.Match(v1 => v1.GetFullName(), v2 => v2.GetFullName());
                return fullName;
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

            actionInputs.AppendLine($@"
type TagWithKey<TagName extends string, T> = {{
  [K in keyof T]: {{ [_ in TagName]: K }} & T[K]
}};

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
) => {{
  const s = useAction<Actions[ActionName], Outputs[ActionName]>(key)
  return s
}}

type QueryTable = TagWithKey<""url"", QueryInputs>;

export function useTypedQuery<ActionName extends keyof QueryTable>(key: ActionName, {{
    placeholder,
    input,
    queryOptions
  }}: {{
    placeholder: QueryOutputs[ActionName],
    input:  QueryInputs[ActionName]
    queryOptions?: UseQueryOptions<QueryOutputs[ActionName]>
  }}): ApiResult<QueryOutputs[ActionName]> {{

  const {{ data, ...rest}} = useApi({{
    url: key,
    method:""POST"",
    payload: input,
    // todo: find defaultPlaceholder
    placeholder: placeholder,
    queryOptions: queryOptions
  }})

  const result = data as QueryOutputs[ActionName]

  return {{ data: result, ...rest}} as any
}}

export type FieldPath<P, S> = {{
  _PATH_: string & {{ _BRAND_: P & S }}
}}

export type PathProxy<P, S> = FieldPath<P, S> &
  {{ [K in keyof S]: PathProxy<P, S[K]> }}

const IdPath = {{ _PATH_: """" }} as FieldPath<any, any>

export function pathProxy<S, P = S>(
  parent: FieldPath<P, S> = IdPath as any,
): PathProxy<P, S> {{
  return new Proxy(parent as any, {{
    get(target: any, key: any) {{
      if (key in target) return target[key]
      return pathProxy<any, any>({{
        _PATH_: `${{parent._PATH_ && parent._PATH_ + "".""}}${{key}}`,
        }} as any)
    }},
}})
}}

export function TypedForm<ActionName extends keyof ActionTable>({{
  initialValues,
  actionName,
  formProps,
  children,
  onSuccess,
  beforeSubmit,
  onError,
}}: Omit<FormikConfig<Actions[ActionName]>, ""onSubmit""{(options.WithPathProxy ?@"|""children""":"")}> & {{
{(options.WithPathProxy ? "  children: ((props: FormikProps<Actions[ActionName]>, pathProxy: PathProxy<Actions[ActionName], Actions[ActionName]>) => React.ReactNode) | React.ReactNode" : "")}
  actionName: ActionName
  beforeSubmit?: (values: Actions[ActionName]) => Actions[ActionName]
  formProps?: FormikFormProps
  onSuccess?: (payload: Outputs[ActionName]) => void
  onError?: (error: ProblemDetails) => void
}}) {{
  const _ = React.useMemo(()=> pathProxy<Actions[ActionName]>(),[])
  const {{ messageSuccess, notifyError}} = useNotify()
  const [submit, validate] = useTypedAction<ActionName>(actionName)
  return (
    <Formik
      validate={{(values) => validate(beforeSubmit ? beforeSubmit(values) : values) }}
      validateOnBlur={{true}}
      enableReinitialize={{true}}
      validateOnChange={{false}}
      initialValues={{initialValues}}
      onSubmit={{async (values) => {{
        const response = await submit(beforeSubmit ? beforeSubmit(values) : values)
        if (response.ok) {{
          onSuccess && onSuccess(response.payload)
        }} else {{
          onError && onError(response.error)
          !onError && notifyError(response.error)
        }}
      }}
    }}
    >
      {{(f) => (
{(options.WithPathProxy?
@$"       <form {{...formProps}}>
        {{typeof children === ""function"" ? children(f,_) : children}}
        </form>" :
@$"        <Form {{...formProps}}>
          {{typeof children === ""function"" ? children(f) : children}}
        </Form>")}

      )}}
    </Formik>)
}}
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
