/* eslint-disable prettier/prettier */
// Assembly: Glow.Sample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
import * as React from "react"
import { Form } from "formik-antd"
import mitt, { Handler, WildcardHandler } from "mitt"
import { useNotification, useWildcardNotification } from "glow-core/lib/notifications/type-notifications"
import * as Glow_Sample from "./Glow.Sample"
import * as Glow_Sample_TreasureIsland_Api from "./Glow.Sample.TreasureIsland.Api"
import * as Glow_Sample_Azdo from "./Glow.Sample.Azdo"
import * as MediatR from "./MediatR"
import * as Microsoft_TeamFoundation_DistributedTask_WebApi from "./Microsoft.TeamFoundation.DistributedTask.WebApi"
import * as Microsoft_VisualStudio_Services_WebApi from "./Microsoft.VisualStudio.Services.WebApi"
import * as Microsoft_VisualStudio_Services_Common from "./Microsoft.VisualStudio.Services.Common"
import * as Microsoft_TeamFoundation_SourceControl_WebApi from "./Microsoft.TeamFoundation.SourceControl.WebApi"
import * as Microsoft_TeamFoundation_Core_WebApi from "./Microsoft.TeamFoundation.Core.WebApi"

export type Events = {
  'Glow.Sample.ItemPicked': Glow_Sample.ItemPicked,
  'Glow.Sample.ItemRemoved': Glow_Sample.ItemRemoved,
  'Glow.Sample.ItemDropped': Glow_Sample.ItemDropped,
  'Glow.Sample.PlayerJoined': Glow_Sample.PlayerJoined,
  'Glow.Sample.PlayerInitialized': Glow_Sample.PlayerInitialized,
  'Glow.Sample.UnitCreated': Glow_Sample.UnitCreated,
  'Glow.Sample.UnitMoved': Glow_Sample.UnitMoved,
  'Glow.Sample.UnitAttacked': Glow_Sample.UnitAttacked,
  'Glow.Sample.GameTick': Glow_Sample.GameTick,
  'Glow.Sample.UnitDied': Glow_Sample.UnitDied,
  'Glow.Sample.DamageTaken': Glow_Sample.DamageTaken,
  'Glow.Sample.ActiveUnitChanged': Glow_Sample.ActiveUnitChanged,
  'Glow.Sample.UnitEnabledForWalk': Glow_Sample.UnitEnabledForWalk,
  'Glow.Sample.GameCreated': Glow_Sample.GameCreated,
  'Glow.Sample.GameStarted': Glow_Sample.GameStarted,
  'Glow.Sample.GameRestarted': Glow_Sample.GameRestarted,
  'Glow.Sample.GameDrawn': Glow_Sample.GameDrawn,
  'Glow.Sample.GameAborted': Glow_Sample.GameAborted,
  'Glow.Sample.GameEnded': Glow_Sample.GameEnded,
  'Glow.Sample.CurrentGameState': Glow_Sample.CurrentGameState,
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



