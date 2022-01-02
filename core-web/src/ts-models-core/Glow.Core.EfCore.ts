import { Func1Task } from "./System"

export interface ResetDatabase {
  deleteDatabase: boolean
  iKnowWhatIAmDoing: boolean
  afterCreated: Func1Task
}

export const defaultResetDatabase: ResetDatabase = {
  deleteDatabase: false,
  iKnowWhatIAmDoing: false,
  afterCreated: {} as any,
}
