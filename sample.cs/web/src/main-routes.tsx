import { Route, Routes } from "react-router"
import styled from "styled-components"
import { PathsContextProvider } from "glow-beta"
import { LoginView } from "./treasure-island/login"
import { GameView } from "./treasure-island/game"
import { AdminView } from "./treasure-island/admin"

export function MainRoutes() {
  return (
    <Routes>
      <Route path="/join" element={<LoginView />} />
      <Route path="/game/:id" element={<GameView />} />
      <Route path="/game/admin-root" element={<AdminView />} />
    </Routes>
  )
}

const MasterDetailContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 10;
  flex: 1;
`

const Container = styled.div`
  padding: 50px;
`
