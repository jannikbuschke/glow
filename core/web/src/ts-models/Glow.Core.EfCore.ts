export interface ResetDatabase {
  deleteDatabase: boolean
  iKnowWhatIAmDoing: boolean
  afterCreated: any
}

export const defaultResetDatabase: ResetDatabase = {
  deleteDatabase: false,
  iKnowWhatIAmDoing: false,
  afterCreated: null,
}

