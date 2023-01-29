//////////////////////////////////////
//   This file is auto generated   //
//////////////////////////////////////



export type List<T> = Array<T>
export var defaultList: <T>(defaultT:T) => List<T> = <T>(defaultT:T) => []


export type Dictionary<TKey,TValue> = { [key: string]: TValue }
export var defaultDictionary: <TKey,TValue>(defaultTKey:TKey,defaultTValue:TValue) => Dictionary<TKey,TValue> = <TKey,TValue>(defaultTKey:TKey,defaultTValue:TValue) => ({})


export type IReadOnlyList<T> = Array<T>
export var defaultIReadOnlyList: <T>(defaultT:T) => IReadOnlyList<T> = <T>(defaultT:T) => []


export type IEnumerable<T> = Array<T>
export var defaultIEnumerable: <T>(defaultT:T) => IEnumerable<T> = <T>(defaultT:T) => []


export type KeyValuePair<TKey,TValue> = ({Key:TKey,Value:TValue})
export var defaultKeyValuePair: <TKey,TValue>(defaultTKey:TKey,defaultTValue:TValue) => KeyValuePair<TKey,TValue> = <TKey,TValue>(defaultTKey:TKey,defaultTValue:TValue) => ({Key:defaultTKey,Value:defaultTValue})

