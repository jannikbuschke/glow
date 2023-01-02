import * as React from "react"
import { BrowserRouter as Router } from "react-router-dom"
import { NotificationsProvider } from "@mantine/notifications"
import { AppShell, MantineProvider, Tabs, Box } from "@mantine/core"
import { MainRoutes } from "./main-routes"
import { useToggle } from "react-use"
import { RoutedTabs } from "glow-mantine/lib/routed-tabs"
import { GlowProvider, VnextAuthenticationProvider } from "glow-core"
import { TypedNotificationsProvider } from "glow-core/lib/notifications/type-notifications"
import { QueryClient, QueryClientProvider } from "react-query"
import { Events } from "./client/subscriptions"

const client = new QueryClient({})

export function RootProviders({ children }: { children: React.ReactElement }) {
  const [darkMode, toggleTheme] = useToggle(false)

  return (
    <>
      <QueryClientProvider client={client}>
        <GlowProvider value={{ componentLibrary: "mantine" }}>
          <VnextAuthenticationProvider>
            <TypedNotificationsProvider<Events>
              requireLoggedIn={false}
              disableLegacy={true}
            >
              <MantineProvider
                withCSSVariables={true}
                withGlobalStyles={true}
                withNormalizeCSS={true}
                theme={{ colorScheme: darkMode ? "dark" : "light" }}
              >
                <NotificationsProvider position="bottom-center">
                  {/* <GlowNotificationsProvider requireLoggedIn={false} disableLegacy={true}> */}
                  {children}
                  {/* </GlowNotificationsProvider> */}
                </NotificationsProvider>
              </MantineProvider>
            </TypedNotificationsProvider>
          </VnextAuthenticationProvider>
        </GlowProvider>
      </QueryClientProvider>
    </>
  )
}

function App() {
  return (
    <Router>
      <RootProviders>
        <>
          <RoutedTabs
            matchExpression="/:rootPath"
            paramName="rootPath"
            defaultValue="join"
          >
            <Tabs.List>
              <Tabs.Tab value="join">join</Tabs.Tab>
              <Tabs.Tab value="game">game</Tabs.Tab>
              <Tabs.Tab value="admin">admin</Tabs.Tab>
              <Tabs.Tab value="debug">debug</Tabs.Tab>
            </Tabs.List>
          </RoutedTabs>

          <AppShell
            padding={0}
            styles={(theme) => ({
              main: {
                backgroundColor:
                  theme.colorScheme === "dark"
                    ? theme.colors.dark![8]
                    : theme.colors.gray![0],
              },
            })}
          >
            <Box p="xs">
              <MainRoutes />
            </Box>
          </AppShell>
        </>
      </RootProviders>
    </Router>
  )
}

export default App
