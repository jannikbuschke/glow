export type LayoutKind = "Sequential" | "Explicit" | "Auto"
export const defaultLayoutKind = "Sequential"
export const LayoutKindValues: { [key in LayoutKind]: LayoutKind } = {
  Sequential: "Sequential",
  Explicit: "Explicit",
  Auto: "Auto",
}
export const LayoutKindValuesArray: LayoutKind[] = Object.keys(LayoutKindValues) as LayoutKind[]

export interface StructLayoutAttribute {
  value: LayoutKind
  typeId: any
}

export const defaultStructLayoutAttribute: StructLayoutAttribute = {
  value: {} as any,
  typeId: null,
}

