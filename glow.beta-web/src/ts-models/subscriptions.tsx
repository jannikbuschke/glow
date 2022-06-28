/* eslint-disable prettier/prettier */
// Assembly: glow.beta, Version=0.23.0.0, Culture=neutral, PublicKeyToken=null
import * as React from "react"
import { Form } from "formik-antd"
import mitt, { Handler, WildcardHandler } from "mitt"
import { useNotification, useWildcardNotification } from "glow-core/lib/notifications/type-notifications"
import * as Glow_Core_Application from "./Glow.Core.Application"
import * as MediatR from "./MediatR"
import * as Glow_Core_Queries from "./Glow.Core.Queries"
import * as Serilog_Events from "./Serilog.Events"

export type Events = {
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



