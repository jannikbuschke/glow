import { RenderObject } from "glow-core"
import { useParams } from "react-router"
import { match } from "ts-pattern"
import { useTypedQuery } from "../client/api"
import { defaultFSharpResult } from "../client/Microsoft_FSharp_Core"
import { Guid } from "../client/System"
import { defaultError, defaultGame } from "../client/TreasureIsland"

export function GameDebugData({ gameId }: { gameId }) {
  const { id } = useParams<{ id: Guid }>()
  const {
    data: currentState,
    refetch,
    isFetched,
  } = useTypedQuery("/api/ti/get-game-sate", {
    input: { gameId: id! },
    placeholder: defaultFSharpResult(defaultGame, defaultError),
  })

  if (!isFetched) {
    return <div>Loading</div>
  }
  return <RenderObject {...currentState} />
}
