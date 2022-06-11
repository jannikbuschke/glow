// Assembly: Glow.Sample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
import * as React from "react"
import { QueryOptions, UseQueryOptions } from "react-query"
import { useApi, ApiResult, notifySuccess, notifyError } from "glow-core"
import { useAction, useSubmit, UseSubmit, ProblemDetails } from "glow-core"
import { Formik, FormikConfig, FormikFormProps } from "formik"
import { Form } from "formik-antd"
import mitt from "mitt"
import * as Glow_Sample_TreasureIsland_Projections from "./Glow.Sample.TreasureIsland.Projections"
import * as Glow_Sample_TreasureIsland_Domain from "./Glow.Sample.TreasureIsland.Domain"
import * as Glow_Sample_TreasureIsland_Api from "./Glow.Sample.TreasureIsland.Api"
import * as Glow_Sample_Azdo from "./Glow.Sample.Azdo"
import * as MediatR from "./MediatR"
import * as Microsoft_TeamFoundation_DistributedTask_WebApi from "./Microsoft.TeamFoundation.DistributedTask.WebApi"
import * as Microsoft_VisualStudio_Services_WebApi from "./Microsoft.VisualStudio.Services.WebApi"
import * as Microsoft_VisualStudio_Services_Common from "./Microsoft.VisualStudio.Services.Common"
import * as Microsoft_TeamFoundation_SourceControl_WebApi from "./Microsoft.TeamFoundation.SourceControl.WebApi"
import * as Microsoft_TeamFoundation_Core_WebApi from "./Microsoft.TeamFoundation.Core.WebApi"


export type Events = {
  'Glow.Sample.TreasureIsland.Projections.CurrentGameState': Glow_Sample_TreasureIsland_Projections.CurrentGameState,
  'Glow.Sample.TreasureIsland.Domain.PlayerJoined': Glow_Sample_TreasureIsland_Domain.PlayerJoined,
  'Glow.Sample.TreasureIsland.Domain.PlayerInitialized': Glow_Sample_TreasureIsland_Domain.PlayerInitialized,
  'Glow.Sample.TreasureIsland.Domain.PlayerCreated': Glow_Sample_TreasureIsland_Domain.PlayerCreated,
  'Glow.Sample.TreasureIsland.Domain.PlayerMoved': Glow_Sample_TreasureIsland_Domain.PlayerMoved,
  'Glow.Sample.TreasureIsland.Domain.PlayerEnabledForWalk': Glow_Sample_TreasureIsland_Domain.PlayerEnabledForWalk,
  'Glow.Sample.TreasureIsland.Domain.GameCreated': Glow_Sample_TreasureIsland_Domain.GameCreated,
  'Glow.Sample.TreasureIsland.Domain.GameStarted': Glow_Sample_TreasureIsland_Domain.GameStarted,
  'Glow.Sample.TreasureIsland.Domain.GameRestarted': Glow_Sample_TreasureIsland_Domain.GameRestarted,
  'Glow.Sample.TreasureIsland.Domain.GameEnded': Glow_Sample_TreasureIsland_Domain.GameEnded,
}

export const emitter = mitt<Events>();

