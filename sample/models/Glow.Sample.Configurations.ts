

export type Enum = "EnumVal1" | "EnumVal2"
export const defaultEnum = "EnumVal1"

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
  nullableEnum: Enum
}

export const defaultSampleConfiguration: SampleConfiguration = {
  prop1: null,
  prop2: 0,
  nested: defaultNested,
  enum: defaultEnum,
  nullableEnum: defaultEnum,
}

