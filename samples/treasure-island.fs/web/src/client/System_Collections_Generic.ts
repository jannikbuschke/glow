//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////

import {TsType} from "./"
import {System} from "./"

export type KeyValuePair<TKey,TValue> = {Key:TKey,Value:TValue}
export var defaultKeyValuePair: <TKey,TValue>(tKey:TKey,tValue:TValue) => KeyValuePair<TKey,TValue> = <TKey,TValue>(tKey:TKey,tValue:TValue) => ({Key:tKey,Value:tValue})
export type IEnumerable<T> = Array<T>
export var defaultIEnumerable: <T>(t:T) => IEnumerable<T> = <T>(t:T) => []
export type ValueCollection<TKey,TValue> = {
  count: System.Int32
}
export var defaultValueCollection: <TKey,TValue>(tKey:TKey,tValue:TValue) => ValueCollection<TKey,TValue> = <TKey,TValue>(tKey:TKey,tValue:TValue) => ({
  count: 0, //#//
})
export type KeyCollection<TKey,TValue> = {
  count: System.Int32
}
export var defaultKeyCollection: <TKey,TValue>(tKey:TKey,tValue:TValue) => KeyCollection<TKey,TValue> = <TKey,TValue>(tKey:TKey,tValue:TValue) => ({
  count: 0, //#//
})
export type IEqualityComparer<T> = {
  
}
export var defaultIEqualityComparer: <T>(t:T) => IEqualityComparer<T> = <T>(t:T) => ({
  
})
export type Dictionary<TKey, TValue> = { [key: string]: TValue }
export var defaultDictionary: <TKey, TValue>(t:TKey,tValue:TValue) => Dictionary<TKey, TValue> = <TKey, TValue>(t:TKey,tValue:TValue) => ({})
export type ICollection<T> = Array<T>
export var defaultICollection: <T>(t:T) => ICollection<T> = <T>(t:T) => []
export type List<T> = Array<T>
export var defaultList: <T>(t:T) => List<T> = <T>(t:T) => ([])
export type IReadOnlyList<T> = Array<T>
export var defaultIReadOnlyList: <T>(t:T) => IReadOnlyList<T> = <T>(t:T) => ([])
