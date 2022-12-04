import { Route, Routes } from "react-router"
import { LoginView } from "./treasure-island/login"
import { GameView } from "./treasure-island/game"
import { AdminView } from "./treasure-island/admin"
import { EsEventList } from "./debug/event-list"

export function MainRoutes() {
  return (
    <Routes>
      <Route path="/join" element={<LoginView />} />
      <Route path="/game/:id" element={<GameView />} />
      <Route path="/admin" element={<AdminView />} />
      <Route path="/debug" element={<EsEventList />} />
    </Routes>
  )
}
