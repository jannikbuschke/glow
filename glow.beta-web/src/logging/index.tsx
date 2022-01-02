import * as React from "react"
import styled, { keyframes } from "styled-components"
// import { useNotifications } from "../notifications"
import { useNotifications } from "glow-core/es/notifications"
import * as emitt from "mitt"
import { useLocalFeatures } from "glow-core/es/feature-flags"

export function LoggingView(props: React.HTMLAttributes<HTMLDivElement>) {
  const [features] = useLocalFeatures<{
    [key: string]: boolean
    uiPerformanceLogging: boolean
  }>({ uiPerformanceLogging: false })
  const { emitter } = useNotifications()
  const [state, dispatch] = React.useReducer(reducer, [])
  React.useEffect(() => {
    const msgName = "Gertrud.Notifications.PerformanceLogMessage"
    const on: emitt.Handler = (msg: any) => {
      dispatch(msg)
    }
    emitter.on(msgName, on)
    return () => {
      emitter.off(msgName, on)
    }
  }, [])
  if (!features.uiPerformanceLogging) {
    return null
  }
  return (
    <Container {...props}>
      {state.slice(0, 8).map((v, i) => (
        <Item key={v.id}>{v.payload.message}</Item>
      ))}
    </Container>
  )
}

function reducer(state: any[], data: any) {
  return [data, ...state]
}

const animation = keyframes`
  from {
    opacity: 1;
  }
  to {
    opacity: 0.2;
  }
`

const Item = styled.div`
  animation: ${animation} 6s ease-in forwards;
  border-top: 1px solid rgba(0, 0, 0, 0.2);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  padding-left: 3px;
  background: white;
`

const Container = styled.div`
  position: fixed;
  bottom: 15px;
  right: 15px;
  width: 500px;
  max-width: calc(100% - 30px);
  z-index: 1;
  box-shadow: 0 6px 9px rgba(0, 0, 0, 0.11), 0 3px 3px rgba(0, 0, 0, 0.15);
  pointer-events: none;
`
