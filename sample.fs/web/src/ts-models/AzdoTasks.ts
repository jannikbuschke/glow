/* eslint-disable prettier/prettier */
import { WorkItemType } from "./Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models"
import { defaultWorkItemType } from "./Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models"

export interface GetAreaPaths {
  projectId: string | null
}

export const defaultGetAreaPaths: GetAreaPaths = {
  projectId: null,
}

export interface DeleteWorkspace {
  id: string
}

export const defaultDeleteWorkspace: DeleteWorkspace = {
  id: "00000000-0000-0000-0000-000000000000",
}

export interface GetAreas {
}

export const defaultGetAreas: GetAreas = {
}

export interface GetArea {
  id: string
}

export const defaultGetArea: GetArea = {
  id: "00000000-0000-0000-0000-000000000000",
}

export interface UpsertWorkspace {
  id: string
  displayName: string | null
  projectId: string | null
  areaPath: string | null
  apiKeys: (string | null)[]
}

export const defaultUpsertWorkspace: UpsertWorkspace = {
  id: "00000000-0000-0000-0000-000000000000",
  displayName: null,
  projectId: null,
  areaPath: null,
  apiKeys: [],
}

export interface GetProjects {
}

export const defaultGetProjects: GetProjects = {
}

export interface GetTask {
  taskId: number
}

export const defaultGetTask: GetTask = {
  taskId: 0,
}

export interface CreateTask {
  title: string | null
  description: string | null
  workItemType: string | null
  workspaceId: string
  createdBy: string | null
}

export const defaultCreateTask: CreateTask = {
  title: null,
  description: null,
  workItemType: null,
  workspaceId: "00000000-0000-0000-0000-000000000000",
  createdBy: null,
}

export interface UpdateTask {
  taskId: number
  title: string | null
  description: string | null
}

export const defaultUpdateTask: UpdateTask = {
  taskId: 0,
  title: null,
  description: null,
}

export interface GetCreateTaskViewmodel {
  workspaceId: string
}

export const defaultGetCreateTaskViewmodel: GetCreateTaskViewmodel = {
  workspaceId: "00000000-0000-0000-0000-000000000000",
}

export interface GetComments {
  taskId: number
}

export const defaultGetComments: GetComments = {
  taskId: 0,
}

export interface GetTasks {
  workspaceId: string
  apiKey: string | null
}

export const defaultGetTasks: GetTasks = {
  workspaceId: "00000000-0000-0000-0000-000000000000",
  apiKey: null,
}

export interface GetWorkspaceViewmodel {
  workspaceId: string
  apiKey: string | null
}

export const defaultGetWorkspaceViewmodel: GetWorkspaceViewmodel = {
  workspaceId: "00000000-0000-0000-0000-000000000000",
  apiKey: null,
}

export interface Workspace {
  id: string
  displayName: string | null
  projectId: string
  areaPath: string | null
  apiKeys: (string | null)[]
}

export const defaultWorkspace: Workspace = {
  id: "00000000-0000-0000-0000-000000000000",
  displayName: null,
  projectId: "00000000-0000-0000-0000-000000000000",
  areaPath: null,
  apiKeys: [],
}

export interface CreateTaskViewmodel {
  id: string
  workItemTypes: WorkItemType[]
}

export const defaultCreateTaskViewmodel: CreateTaskViewmodel = {
  id: "00000000-0000-0000-0000-000000000000",
  workItemTypes: [],
}

export interface WorkspaceViewmodel {
  workspaceId: string
  projectName: string | null
  areaPath: string | null
}

export const defaultWorkspaceViewmodel: WorkspaceViewmodel = {
  workspaceId: "00000000-0000-0000-0000-000000000000",
  projectName: null,
  areaPath: null,
}

