export type ProjectState = "New" | "WellFormed" | "Deleting" | "CreatePending" | "Deleted" | "Unchanged" | "All"
export const defaultProjectState = "New"
export const ProjectStateValues: { [key in ProjectState]: ProjectState } = {
  New: "New",
  WellFormed: "WellFormed",
  Deleting: "Deleting",
  CreatePending: "CreatePending",
  Deleted: "Deleted",
  Unchanged: "Unchanged",
  All: "All",
}
export const ProjectStateValuesArray: ProjectState[] = Object.keys(ProjectStateValues) as ProjectState[]

export type ProjectVisibility = "Private" | "Organization" | "Public" | "SystemPrivate" | "Unchanged"
export const defaultProjectVisibility = "Private"
export const ProjectVisibilityValues: { [key in ProjectVisibility]: ProjectVisibility } = {
  Private: "Private",
  Organization: "Organization",
  Public: "Public",
  SystemPrivate: "SystemPrivate",
  Unchanged: "Unchanged",
}
export const ProjectVisibilityValuesArray: ProjectVisibility[] = Object.keys(ProjectVisibilityValues) as ProjectVisibility[]

export interface TeamProjectReference {
  id: string
  abbreviation: string | null
  name: string | null
  description: string | null
  url: string | null
  state: ProjectState
  revision: number
  visibility: ProjectVisibility
  defaultTeamImageUrl: string | null
  lastUpdateTime: string
}

export const defaultTeamProjectReference: TeamProjectReference = {
  id: "00000000-0000-0000-0000-000000000000",
  abbreviation: null,
  name: null,
  description: null,
  url: null,
  state: {} as any,
  revision: 0,
  visibility: {} as any,
  defaultTeamImageUrl: null,
  lastUpdateTime: "1/1/0001 12:00:00 AM",
}

