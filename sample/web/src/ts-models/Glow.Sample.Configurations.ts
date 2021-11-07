/* eslint-disable prettier/prettier */
export type Enum = "EnumVal1" | "EnumVal2"
export const defaultEnum = "EnumVal1"
export const EnumValues: { [key in Enum]: Enum } = {
  EnumVal1: "EnumVal1",
  EnumVal2: "EnumVal2",
}
export const EnumValuesArray: Enum[] = Object.keys(EnumValues) as Enum[]

export interface Nested {
  value: string | null
}

export const defaultNested: Nested = {
  value: null,
}

export interface SampleConfiguration {
  prop1: string | null
  prop2: number
  nested: Nested
  enum: Enum
  nullableEnum: Enum | null
}

export const defaultSampleConfiguration: SampleConfiguration = {
  prop1: null,
  prop2: 0,
  nested: {} as any,
  enum: {} as any,
  nullableEnum: {} as any,
}

