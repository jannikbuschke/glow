import * as React from "react"
import { BrowserRouter as Router } from "react-router-dom"

import {
  NotificationsProvider,
  updateNotification,
} from "@mantine/notifications"
import {
  AppShell,
  MantineProvider,
  Navbar,
  Divider,
  DefaultMantineColor,
  Global,
  Tabs,
} from "@mantine/core"
import { Text } from "@mantine/core"
import { MainLinks } from "glow-mantine"
import { MainRoutes } from "./main-routes"
import { showNotification } from "@mantine/notifications"
import { CheckIcon } from "@modulz/radix-icons"
import { FaTasks, FaVoteYea } from "react-icons/fa"
import { FiSettings, FiUsers } from "react-icons/fi"
import { BsCalendar3Week, BsUiChecks } from "react-icons/bs"
import { IoAppsSharp } from "react-icons/io5"
import { useToggle } from "react-use"
import { RoutedTabs } from "glow-mantine/lib/routed-tabs"

import {
  AuthenticationProvider,
  NotificationsProvider as GlowNotificationsProvider,
  useNotification,
} from "glow-core"
import { TypedNotificationsProvider } from "glow-core/lib/notifications/type-notifications"
import { GameCreated } from "./ts-models/Glow.Sample.TreasureIsland.Domain"
import { Events } from "./ts-models/subscriptions"
import { ArchiveIcon, PlayIcon } from "@modulz/radix-icons"

const iconColor: DefaultMantineColor = "blue"

function Planner1Providers({ children }: { children: React.ReactElement }) {
  return (
    <>
      <Global
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
              theme.colorScheme === "dark" ? theme.colors.dark[7] : theme.white,
            color:
              theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.black,
            lineHeight: theme.lineHeight,
          },
        })}
      />
      <AuthenticationProvider>
        <TypedNotificationsProvider<Events>
          requireLoggedIn={false}
          disableLegacy={true}
        >
          {/* <GlowNotificationsProvider requireLoggedIn={false} disableLegacy={true}> */}
          <BaseSettings>{children}</BaseSettings>
          {/* </GlowNotificationsProvider> */}
        </TypedNotificationsProvider>
      </AuthenticationProvider>
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
  const [darkMode, toggleTheme] = useToggle(true)

  return (
    <Router>
      <Planner1Providers>
        <MantineProvider
          withCSSVariables={true}
          withGlobalStyles={true}
          withNormalizeCSS={true}
          theme={{
            // colorScheme: "dark",
            colorScheme: darkMode ? "dark" : "light",
            // Override any other properties from default theme
            // fontFamily: "Open Sans, sans serif",
            // spacing: { xs: 15, sm: 20, md: 25, lg: 30, xl: 40 },
          }}
        >
          <NotificationsProvider position="bottom-center">
            <RoutedTabs
              position="center"
              tabs={[
                { label: "Join", tabKey: "join" },
                { label: "Game", tabKey: "game", icon: <PlayIcon /> },
                {
                  label: "Admin",
                  tabKey: "admin",
                  icon: <PlayIcon />,
                },
              ]}
            />
            {/* <Tabs position="center">
              <Tabs.Tab label="Join" icon={<PlayIcon />}>
                Gallery tab content
              </Tabs.Tab>
              <Tabs.Tab label="Game" icon={<FiSettings />}>
                Gallery tab content
              </Tabs.Tab>
            </Tabs> */}
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
                      ? theme.colors.dark[8]
                      : theme.colors.gray[0],
                },
              })}
            >
              {/* <NotificationTest /> */}
              <MainRoutes />
              <Logger />
            </AppShell>
          </NotificationsProvider>
        </MantineProvider>
      </Planner1Providers>
    </Router>
  )
}

export default App

function NotificationTest() {
  React.useEffect(() => {
    setTimeout(() => {
      showNotification({
        id: "1",
        title: "Syncing files",
        message: "Syncing meeting files to SharePoint",
        loading: true,
      })
    }, 200)

    setTimeout(() => {
      updateNotification({
        id: "1",
        color: "teal",
        title: "Syncing files",
        message: "All meeting item files are now synced",
        icon: <CheckIcon />,
        loading: false,
        autoClose: 5000,
      })
    }, 2000)
  }, [])
  return null
}
