// Assembly: sample.fs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
import * as React from "react"
import { Form } from "formik-antd"
import mitt, { Handler, WildcardHandler } from "mitt"
import { useNotification, useWildcardNotification } from "glow-core/lib/notifications/type-notifications"
import * as AzdoTasks from "./AzdoTasks"
import * as Sample_Fs_Agenda from "./Sample.Fs.Agenda"
import * as Microsoft_TeamFoundation_WorkItemTracking_WebApi_Models from "./Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models"
import * as Microsoft_VisualStudio_Services_WebApi from "./Microsoft.VisualStudio.Services.WebApi"
import * as Microsoft_TeamFoundation_Core_WebApi from "./Microsoft.TeamFoundation.Core.WebApi"
import * as Microsoft_VisualStudio_Services_Common from "./Microsoft.VisualStudio.Services.Common"
import * as Microsoft_FSharp_Core from "./Microsoft.FSharp.Core"
import * as MediatR from "./MediatR"

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



