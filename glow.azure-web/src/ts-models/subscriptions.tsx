/* eslint-disable prettier/prettier */
// Assembly: glow.azure, Version=0.23.0.0, Culture=neutral, PublicKeyToken=null
import * as React from "react"
import { Form } from "formik-antd"
import mitt, { Handler, WildcardHandler } from "mitt"
import { useNotification, useWildcardNotification } from "glow-core/lib/notifications/type-notifications"
import * as Glow_TestAutomation from "./Glow.TestAutomation"
import * as Glow_Azure_AzureKeyVault from "./Glow.Azure.AzureKeyVault"
import * as MediatR from "./MediatR"
import * as Glow_Core_Profiles from "./Glow.Core.Profiles"

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



