import * as React from "react"
import { BrowserRouter as Router } from "react-router-dom"
import {
  NotificationsProvider,
  updateNotification,
} from "@mantine/notifications"
import {
  AppShell,
  MantineProvider,
  DefaultMantineColor,
  Tabs,
  Box,
} from "@mantine/core"
import { MainRoutes } from "./main-routes"
import { showNotification } from "@mantine/notifications"
// import { CheckIcon, PlayIcon } from "@modulz/radix-icons"
import { CheckIcon, PlayIcon } from "@radix-ui/react-icons"
import { FaPlay } from "react-icons/fa"
import { useToggle } from "react-use"
import { RoutedTabs } from "glow-mantine/lib/routed-tabs"
import {
  AuthenticationProvider,
  GlowProvider,
  VnextAuthenticationProvider,
} from "glow-core"
import { TypedNotificationsProvider } from "glow-core/lib/notifications/type-notifications"
import { QueryClient, QueryClientProvider } from "react-query"
import { Events } from "./client/subscriptions"

const client = new QueryClient({})
function MyProviders({ children }: { children: React.ReactElement }) {
  return (
    <>
      <QueryClientProvider client={client}>
        {/* <Global
        styles={(theme) => ({
          "*, *::before, *::after": {
            boxSizing: "content-box",
          },
          ":root": {
            "--selected-hexagon-stroke": theme.colors.dark[5],
          },

          body: {
            ...theme.fn.fontStyles(),
            backgroundColor:
              theme.colorScheme === "dark"
                ? theme.colors.dark[7]
                : theme.white,
            color:
              theme.colorScheme === "dark"
                ? theme.colors.dark[0]
                : theme.black,
            lineHeight: theme.lineHeight,
          },
        })}
        /> */}
        <VnextAuthenticationProvider>
          <TypedNotificationsProvider<Events>
            requireLoggedIn={false}
            disableLegacy={true}
          >
            <GlowProvider value={{ componentLibrary: "mantine" }}>
              {/* <GlowNotificationsProvider requireLoggedIn={false} disableLegacy={true}> */}
              <BaseSettings>{children}</BaseSettings>
              {/* </GlowNotificationsProvider> */}
            </GlowProvider>
          </TypedNotificationsProvider>
        </VnextAuthenticationProvider>
      </QueryClientProvider>
    </>
  )
}

function BaseSettings({ children }: { children: React.ReactElement }) {
  return children
}

function Logger() {
  // useNotification<GameCreated>(
  //   "Glow.Sample.TreasureIsland.Domain.GameCreated",
  //   (v) => {
  //     console.log({ v })
  //   },
  // )
  // useNotification<any>("*", (v) => {
  //   showNotification({
  //     title: JSON.stringify(v),
  //     message: "hello",
  //   })
  // })
  return null
}

function App() {
  const [darkMode, toggleTheme] = useToggle(false)

  return (
    <Router>
      <MyProviders>
        <MantineProvider
          // withCSSVariables={true}
          theme={{
            colorScheme: darkMode ? "dark" : "light",
            // Override any other properties from default theme
            // fontFamily: "Open Sans, sans serif",
            // spacing: { xs: 15, sm: 20, md: 25, lg: 30, xl: 40 },
          }}
        >
          <NotificationsProvider position="bottom-center">
            <RoutedTabs
              matchExpression="/:rootPath"
              paramName="rootPath"
              defaultValue="join"
              // position="center"
              // tabs={[
              //   { label: "Join", tabKey: "join" },
              //   {
              //     label: "Game",
              //     tabKey: "game",
              //     icon: <FaPlay color="black" />,
              //   },
              //   {
              //     label: "Admin",
              //     tabKey: "admin",
              //   },
              // ]}
            >
              <Tabs.List>
                <Tabs.Tab value="join">join</Tabs.Tab>
                <Tabs.Tab value="game">game</Tabs.Tab>
                <Tabs.Tab value="admin">admin</Tabs.Tab>
                <Tabs.Tab value="debug">debug</Tabs.Tab>
              </Tabs.List>
              {/*
              <Tabs.Panel value="gallery" pt="xs">
                Gallery tab content
              </Tabs.Panel>

              <Tabs.Panel value="messages" pt="xs">
                Messages tab content
              </Tabs.Panel>

              <Tabs.Panel value="settings" pt="xs">
                Settings tab content
              </Tabs.Panel> */}
            </RoutedTabs>

            <AppShell
              padding={0}
              // header={
              //   <Header height={60} p="xs">
              //     {/* Header content */}
              //   </Header>
              // }
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
                <Logger />
              </Box>
            </AppShell>
          </NotificationsProvider>
        </MantineProvider>
      </MyProviders>
    </Router>
  )
}

export default App
