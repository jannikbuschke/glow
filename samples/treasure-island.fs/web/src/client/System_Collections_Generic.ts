//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import * as TsType from "./TsType"
import * as System from "./System"

export type KeyValuePair<TKey,TValue> = {Key:TKey,Value:TValue}
export const defaultKeyValuePair: <TKey,TValue>(tKey:TKey,tValue:TValue) => KeyValuePair<TKey,TValue> = <TKey,TValue>(tKey:TKey,tValue:TValue) => ({Key:tKey,Value:tValue})
export type IEnumerable<T> = Array<T>
export const defaultIEnumerable: <T>(t:T) => IEnumerable<T> = <T>(t:T) => []
export type ValueCollection<TKey,TValue> = {
  count: System.Int32
}
export const defaultValueCollection: <TKey,TValue>(tKey:TKey,tValue:TValue) => ValueCollection<TKey,TValue> = <TKey,TValue>(tKey:TKey,tValue:TValue) => ({
  count: System.defaultInt32,
})
export type KeyCollection<TKey,TValue> = {
  count: System.Int32
}
export const defaultKeyCollection: <TKey,TValue>(tKey:TKey,tValue:TValue) => KeyCollection<TKey,TValue> = <TKey,TValue>(tKey:TKey,tValue:TValue) => ({
  count: System.defaultInt32,
})
export type IEqualityComparer<T> = {
  
}
export const defaultIEqualityComparer: <T>(t:T) => IEqualityComparer<T> = <T>(t:T) => ({
  
})
export type Dictionary<TKey, TValue> = { [key: string]: TValue }
export const defaultDictionary: <TKey, TValue>(t:TKey,tValue:TValue) => Dictionary<TKey, TValue> = <TKey, TValue>(t:TKey,tValue:TValue) => ({})
export type ICollection<T> = Array<T>
export const defaultICollection: <T>(t:T) => ICollection<T> = <T>(t:T) => []
export type List<T> = Array<T>
export const defaultList: <T>(t:T) => List<T> = <T>(t:T) => ([])
export type IReadOnlyList<T> = Array<T>
export const defaultIReadOnlyList: <T>(t:T) => IReadOnlyList<T> = <T>(t:T) => ([])
