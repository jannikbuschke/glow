export type LayoutKind = "Sequential" | "Explicit" | "Auto"
export const defaultLayoutKind = "Sequential"

export interface StructLayoutAttribute {
  value: LayoutKind
  typeId: any
}

export const defaultStructLayoutAttribute: StructLayoutAttribute = {
  value: {} as any,
  typeId: null,
}

