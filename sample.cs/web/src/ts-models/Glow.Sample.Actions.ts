/* eslint-disable prettier/prettier */
export interface SampleAction {
  foo: string | null
}

export const defaultSampleAction: SampleAction = {
  foo: null,
}

export interface SampleAction2 {
  message: string | null
}

export const defaultSampleAction2: SampleAction2 = {
  message: null,
}

export interface Response {
  value: string | null
}

export const defaultResponse: Response = {
  value: null,
}

