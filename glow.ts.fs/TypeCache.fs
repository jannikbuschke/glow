// copy pasted from https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/src/FSharp.SystemTextJson/TypeCache.fs
namespace System.Text.Json.Serialization

module TypeCache =
  open FSharp.Reflection

  // Have to use concurrentdictionary here because dictionaries thrown on non-locked access:
  (* Error Message:
        System.InvalidOperationException : Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.
        Stack Trace:
            at System.Collections.Generic.Dictionary`2.TryInsert(TKey key, TValue value, InsertionBehavior behavior) *)
  type Dict<'a, 'b> = System.Collections.Concurrent.ConcurrentDictionary<'a, 'b>

  type TypeKind =
    | Record
    | Union
    | List
    | Set
    | Array
    | Map
    | Tuple
    | Enum
    | Other

  /// cached access to FSharpType.* and System.Type to prevent repeated access to reflection members
  let getKind =
    let cache = Dict<System.Type, TypeKind>()
    let listTy = typedefof<_ list>
    let setTy = typedefof<Set<_>>
    let mapTy = typedefof<Map<_, _>>

    fun (ty: System.Type) ->
      cache.GetOrAdd(
        ty,
        fun ty ->
          if ty.IsGenericType && ty.GetGenericTypeDefinition() = listTy then
            TypeKind.List
          elif ty.IsGenericType && ty.GetGenericTypeDefinition() = setTy then
            TypeKind.Set
          elif ty.IsGenericType && ty.GetGenericTypeDefinition() = mapTy then
            TypeKind.Map
          elif FSharpType.IsTuple(ty) then
            TypeKind.Tuple
          elif FSharpType.IsUnion(ty, true) then
            TypeKind.Union
          elif FSharpType.IsRecord(ty, true) then
            TypeKind.Record
          elif ty.IsEnum then
            TypeKind.Enum
          elif ty.IsArray then
            TypeKind.Array
          else
            TypeKind.Other
      )

  let isUnion ty = getKind ty = TypeKind.Union

  let isRecord ty = getKind ty = TypeKind.Record

  let isList ty = getKind ty = TypeKind.List

  let isSet ty = getKind ty = TypeKind.Set

  let isMap ty = getKind ty = TypeKind.Map

  let isTuple ty = getKind ty = TypeKind.Tuple
