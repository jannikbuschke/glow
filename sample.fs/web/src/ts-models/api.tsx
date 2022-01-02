import * as React from "react"
import { QueryOptions, UseQueryOptions } from "react-query"
import { useApi, ApiResult, notifySuccess, notifyError } from "glow-core"
import { useAction, useSubmit, UseSubmit, ProblemDetails } from "glow-core/es/actions/use-submit"
import { Formik, FormikConfig, FormikFormProps } from "formik"
import { Form } from "formik-antd"
import * as AzdoTasks from "./AzdoTasks"
import * as Sample_Fs_Agenda from "./Sample.Fs.Agenda"
import * as Microsoft_TeamFoundation_WorkItemTracking_WebApi_Models from "./Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models"
import * as Microsoft_VisualStudio_Services_WebApi from "./Microsoft.VisualStudio.Services.WebApi"
import * as Microsoft_TeamFoundation_Core_WebApi from "./Microsoft.TeamFoundation.Core.WebApi"
import * as Microsoft_VisualStudio_Services_Common from "./Microsoft.VisualStudio.Services.Common"
import * as MediatR from "./MediatR"

type QueryInputs = {
  "/api/get-area-paths": AzdoTasks.GetAreaPaths,
  "/api/get-workspaces": AzdoTasks.GetAreas,
  "/api/get-workspace": AzdoTasks.GetArea,
  "/api/get-projects": AzdoTasks.GetProjects,
  "/api/get-task": AzdoTasks.GetTask,
  "/api/get-create-task-viewmodel": AzdoTasks.GetCreateTaskViewmodel,
  "/api/get-comments": AzdoTasks.GetComments,
  "/api/get-tasks": AzdoTasks.GetTasks,
  "/api/get-workspace-viewmodel": AzdoTasks.GetWorkspaceViewmodel,
  "/api/get-meeting": Sample_Fs_Agenda.GetMeeting,
}
type QueryOutputs = {
  "/api/get-area-paths": Microsoft_TeamFoundation_WorkItemTracking_WebApi_Models.WorkItemClassificationNode,
  "/api/get-workspaces": AzdoTasks.Workspace[],
  "/api/get-workspace": AzdoTasks.Workspace,
  "/api/get-projects": Microsoft_TeamFoundation_Core_WebApi.TeamProjectReference[],
  "/api/get-task": Microsoft_TeamFoundation_WorkItemTracking_WebApi_Models.WorkItem,
  "/api/get-create-task-viewmodel": AzdoTasks.CreateTaskViewmodel,
  "/api/get-comments": Microsoft_TeamFoundation_WorkItemTracking_WebApi_Models.WorkItemComments,
  "/api/get-tasks": Microsoft_TeamFoundation_WorkItemTracking_WebApi_Models.WorkItem[],
  "/api/get-workspace-viewmodel": AzdoTasks.WorkspaceViewmodel,
  "/api/get-meeting": Sample_Fs_Agenda.Meeting,
}
export type Outputs = {
  "/api/delete-workspace": AzdoTasks.Workspace,
  "/api/upsert-workspace": AzdoTasks.Workspace,
  "/api/create-task": Microsoft_TeamFoundation_WorkItemTracking_WebApi_Models.WorkItem,
  "/api/update-task": Microsoft_TeamFoundation_WorkItemTracking_WebApi_Models.WorkItem,
  "/api/meeting/create": Sample_Fs_Agenda.Meeting,
  "/api/reorder-agenda-items": MediatR.Unit,
  "/api/upsert-meeting-item": MediatR.Unit,
}
export type Actions = {
  "/api/delete-workspace": AzdoTasks.DeleteWorkspace,
  "/api/upsert-workspace": AzdoTasks.UpsertWorkspace,
  "/api/create-task": AzdoTasks.CreateTask,
  "/api/update-task": AzdoTasks.UpdateTask,
  "/api/meeting/create": Sample_Fs_Agenda.CreateMeeting,
  "/api/reorder-agenda-items": Sample_Fs_Agenda.ReorderAgendaItems,
  "/api/upsert-meeting-item": Sample_Fs_Agenda.UpsertMeetingItem,
}

type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
};

export type ActionTable = TagWithKey<"url", Actions>

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

type QueryTable = TagWithKey<"url", QueryInputs>;

export function useTypedQuery<ActionName extends keyof QueryTable>(key: ActionName, {
    placeholder,
    input,
    queryOptions
  }: {
    placeholder: QueryOutputs[ActionName],
    input:  QueryInputs[ActionName]
    queryOptions?: UseQueryOptions<QueryOutputs[ActionName]>
  }): ApiResult<QueryOutputs[ActionName]> {

  const { data, ...rest} = useApi({
    url: key,
    method:"POST",
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
}: Omit<FormikConfig<Actions[ActionName]>, "onSubmit"> & {
  actionName: ActionName
  formProps?: FormikFormProps
  onSuccess?: (payload: Outputs[ActionName]) => void
  onError?: (error: ProblemDetails) => void
}) {
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
          {typeof children === "function" ? children(f) : children}
        </Form>
      )}
    </Formik>)
}

