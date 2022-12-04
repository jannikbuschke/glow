import * as React from "react"
import { Form } from "formik-antd"
import mitt, { Handler, WildcardHandler } from "mitt"
import { useNotification, useWildcardNotification } from "glow-core/lib/notifications/type-notifications"
import * as TreasureIsland from "./TreasureIsland"
import * as System from "./System"
import * as Microsoft_FSharp_Core from "./Microsoft_FSharp_Core"
import * as Microsoft_FSharp_Collections from "./Microsoft_FSharp_Collections"
import * as System_Collections_Generic from "./System_Collections_Generic"

export type Events = {
  'TreasureIsland.ItemPicked': TreasureIsland.ItemPicked,
  'TreasureIsland.ItemRemoved': TreasureIsland.ItemRemoved,
  'TreasureIsland.ItemDropped': TreasureIsland.ItemDropped,
  'TreasureIsland.PlayerJoined': TreasureIsland.PlayerJoined,
  'TreasureIsland.PlayerUnitInitialized': TreasureIsland.PlayerUnitInitialized,
  'TreasureIsland.PlayerUnitCreated': TreasureIsland.PlayerUnitCreated,
  'TreasureIsland.UnitMoved': TreasureIsland.UnitMoved,
  'TreasureIsland.UnitAttacked': TreasureIsland.UnitAttacked,
  'TreasureIsland.GameTick': TreasureIsland.GameTick,
  'TreasureIsland.UnitDied': TreasureIsland.UnitDied,
  'TreasureIsland.DamageTaken': TreasureIsland.DamageTaken,
  'TreasureIsland.ActiveUnitChanged': TreasureIsland.ActiveUnitChanged,
  'TreasureIsland.UnitEnabledForWalk': TreasureIsland.UnitEnabledForWalk,
  'TreasureIsland.GameCreated': TreasureIsland.GameCreated,
  'TreasureIsland.GameRestarted': TreasureIsland.GameRestarted,
  'TreasureIsland.GameDrawn': TreasureIsland.GameDrawn,
  'TreasureIsland.GameAborted': TreasureIsland.GameAborted,
  'TreasureIsland.GameEnded': TreasureIsland.GameEnded,
  'TreasureIsland.GameEventNotification': TreasureIsland.GameEventNotification,
  'TreasureIsland.CurrentGameState': TreasureIsland.CurrentGameState,
}

// export const emitter = mitt<Events>();

type TagWithKey<TagName extends string, T> = {
  [K in keyof T]: { [_ in TagName]: K } & T[K]
}

export type EventTable = TagWithKey<"url", Events>

export function useSubscription<EventName extends keyof EventTable>(
    name: EventName,
    callback: Handler<Events[EventName]>,
    deps?: any[],
    ) {
    const ctx = useNotification<EventName, Events>(name, callback, deps)
    return ctx
}

export function useSubscriptions(
  callback: WildcardHandler<Events>,
  deps?: any[],
) {
  const ctx = useWildcardNotification<Events>(callback, deps)
  return ctx
}

