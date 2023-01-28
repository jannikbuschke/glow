import { FSharpMap } from "../client/Microsoft_FSharp_Collections"

export function containsKey<TKey>(
  map: FSharpMap<TKey, any>,
  key: TKey,
): boolean {
  const result = map.some(([k, _]) => {
    console.log({ k, _ })
    return k === key
  })
  console.log("check if key exists", key, map, result)

  return map.some(([k, _]) => k === key)
}

export function get<TKey, TValue>(
  map: FSharpMap<TKey, TValue>,
  key: TKey,
): TValue | undefined {
  const item = map.find(([k, _]) => {
    return k === key
  })
  return item ? item[1] : undefined
}

export function values<TKey, TValue>(map: FSharpMap<TKey, TValue>): TValue[] {
  return map.map(([_, v]) => v)
}

export const FsMap = {
  containsKey,
  get,
  values,
}
