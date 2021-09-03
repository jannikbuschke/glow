export type Operation = "StartsWith" | "Equals" | "LessThan" | "GreaterThan" | "Contains"
export const defaultOperation = "StartsWith"
export const OperationValues: { [key in Operation]: Operation } = {
  StartsWith: "StartsWith",
  Equals: "Equals",
  LessThan: "LessThan",
  GreaterThan: "GreaterThan",
  Contains: "Contains",
}
export const OperationValuesArray: Operation[] = Object.keys(OperationValues) as Operation[]

export type Direction = "Asc" | "Desc"
export const defaultDirection = "Asc"
export const DirectionValues: { [key in Direction]: Direction } = {
  Asc: "Asc",
  Desc: "Desc",
}
export const DirectionValuesArray: Direction[] = Object.keys(DirectionValues) as Direction[]

export interface Where {
  property: string | null
  operation: Operation
  value: string | null
}

export const defaultWhere: Where = {
  property: null,
  operation: {} as any,
  value: null,
}

export interface OrderBy {
  property: string | null
  direction: Direction
}

export const defaultOrderBy: OrderBy = {
  property: null,
  direction: {} as any,
}

export interface Query {
  search: string | null
  take: number | null
  skip: number | null
  where: Where
  orderBy: OrderBy
  count: boolean | null
}

export const defaultQuery: Query = {
  search: null,
  take: null,
  skip: null,
  where: {} as any,
  orderBy: {} as any,
  count: null,
}

export interface QueryResult<T> {
  count: number | null
  value: any
}


