import * as React from "react"
import { Form } from "formik-antd"
import mitt, { Handler, WildcardHandler } from "mitt"
import { useNotification, useWildcardNotification } from "glow-core/lib/notifications/type-notifications"
import * as System from "./System"
import * as TreasureIsland from "./TreasureIsland"
import * as Microsoft_FSharp_Core from "./Microsoft_FSharp_Core"
import * as Microsoft_FSharp_Collections from "./Microsoft_FSharp_Collections"
import * as Glow_Api from "./Glow_Api"
import * as Glow_Debug from "./Glow_Debug"
import * as Glow_TestAutomation from "./Glow_TestAutomation"
import * as Glow_Azure_AzureKeyVault from "./Glow_Azure_AzureKeyVault"
import * as Glow_Core_Profiles from "./Glow_Core_Profiles"
import * as System_Collections_Generic from "./System_Collections_Generic"
import * as MediatR from "./MediatR"

export type Events = {
  'TreasureIsland.ItemPicked': TreasureIsland.ItemPicked,
  'TreasureIsland.ItemRemoved': TreasureIsland.ItemRemoved,
  'TreasureIsland.ItemDropped': TreasureIsland.ItemDropped,
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

