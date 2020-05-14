export interface OdataPage<T> {
  value: T[]
  count?: number | undefined
}
