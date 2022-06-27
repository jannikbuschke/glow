import React from "react"
import "antd/dist/antd.css"
import { BrowserRouter as Router } from "react-router-dom"
import styled from "styled-components"
import { LeftNavigation } from "glow-beta"
import { FolderOpenOutlined, EditOutlined } from "@ant-design/icons"
import { QueryClient, QueryClientProvider } from "react-query"
import { Box, ChakraProvider } from "@chakra-ui/react"
import { Routes, Route, Outlet } from "react-router"
import { DndRoutes } from "./experiments/drag-and-drop/index"
import { AppShell, Global, Header, MantineProvider } from "@mantine/core"
import { AuthenticationProvider } from "glow-core"
import { TypedNotificationsProvider } from "glow-core/lib/notifications/type-notifications"
import { Events } from "./ts-models/subscriptions"
import { NotificationsProvider } from "@mantine/notifications"
import { RoutedTabs } from "glow-mantine/lib/routed-tabs"

function App() {
  return (
    <AppShell
      padding={0}
      header={
        <Header height={60} p="xs">
          <RoutedTabs
            position="center"
            tabs={[
              { label: "DND", tabKey: "dnd" },
              {
                label: "Game",
                tabKey: "game",
                //  icon: <PlayIcon />
              },
              {
                label: "Admin",
                tabKey: "admin",
                // icon: <PlayIcon />,
              },
            ]}
          />
          {/* Header content */}
        </Header>
      }
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
      <DndRoutes />
      {/*
        <MainRoutes />
        <Logger /> */}
    </AppShell>
  )
}

const client = new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
    },
  },
})

export function Root() {
  // const [darkMode, toggleTheme] = useToggle(true)

  return (
    <MantineProvider
      withCSSVariables={true}
      withGlobalStyles={true}
      withNormalizeCSS={true}
      theme={
        {
          // colorScheme: "dark",
          // colorScheme: darkMode ? "dark" : "light",
          // Override any other properties from default theme
          // fontFamily: "Open Sans, sans serif",
          // spacing: { xs: 15, sm: 20, md: 25, lg: 30, xl: 40 },
        }
      }
    >
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
        <NotificationsProvider position="bottom-center">
          <TypedNotificationsProvider<Events>
            requireLoggedIn={false}
            disableLegacy={true}
          >
            <ChakraProvider>
              <QueryClientProvider client={client}>
                <Router>
                  <App />
                </Router>
              </QueryClientProvider>
            </ChakraProvider>
          </TypedNotificationsProvider>
        </NotificationsProvider>
      </AuthenticationProvider>
    </MantineProvider>
  )
}
