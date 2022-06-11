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

import {
  AuthenticationProvider,
  NotificationsProvider as GlowNotificationsProvider,
  TypedNotificationsProvider,
  useNotification,
} from "glow-core"
import { GameCreated } from "./ts-models/Glow.Sample.TreasureIsland.Domain"

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
        <TypedNotificationsProvider requireLoggedIn={false} disableLegacy={true} >

        {/* <GlowNotificationsProvider requireLoggedIn={false} disableLegacy={true}> */}
          <BaseSettings>{children}</BaseSettings>
        {/* </GlowNotificationsProvider> */}
      </AuthenticationProvider>
        </TypedNotificationsProvider>
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
          <NotificationsProvider position="top-right">
            <AppShell
              padding="md"
              navbar={
                <Navbar width={{ base: 250, xs: 250 }}>
                  {/* Navbar content */}
                  {/* First section with normal height (depends on section content) */}
                  <Navbar.Section p={16}>
                    <div
                      style={{
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center",
                      }}
                    >
                      {/* <img
                            src={logo_lang}
                            style={{
                              height: "30px",
                              width: "auto",
                            }}
                          /> */}

                      {/* <Sun
                          cursor={"pointer"}
                          color={darkMode ? "white" : "black"}
                          onClick={() => toggleTheme()}
                        /> */}
                    </div>
                  </Navbar.Section>
                  <Divider size="sm" style={{ marginTop: "0px" }} />

                  {/* Grow section will take all available space that is not taken by first and last sections */}
                  <Navbar.Section grow style={{}}>
                    <MainLinks
                      size="md"
                      padding={12}
                      borderRightSize={2}
                      iconColor={iconColor}
                      data={[
                        {
                          icon: <IoAppsSharp />,
                          label: "Join",
                          to: "/join",
                        },
                        // {
                        //   icon: <BsCalendar3Week />,
                        //   label: "Sitzungen",
                        //   to: "/meetings",
                        // },
                        // {
                        //   icon: <FaVoteYea />,
                        //   label: "Umlaufbeschlüsse",
                        //   to: "/circular-resolutions",
                        // },
                        // {
                        //   icon: <FiUsers />,
                        //   label: "Vorstände und Profile",
                        //   to: "/committees",
                        // },
                        // {
                        //   icon: <FaTasks />,
                        //   color: "blue",
                        //   size: "lg",
                        //   label: "Tasks",
                        //   to: "/tasks",
                        // },
                        {
                          icon: <FiSettings />,
                          label: "Einstellungen",
                          to: "/settings",
                        },
                        // { icon: undefined, color: "violet", label: "Discussions" },
                        // { icon: undefined, color: "grape", label: "Databases" },
                      ]}
                    />
                  </Navbar.Section>
                  <Divider
                    size="sm"
                    style={{ marginTop: "0px", marginBottom: "13px" }}
                  />

                  {/* Grow section will take all available space that is not taken by first and last sections */}
                  {/* <Navbar.Section grow>
                      <SidebarUserProfile />
                    </Navbar.Section> */}

                  {/* Last section with normal height (depends on section content) */}
                  {/* <Navbar.Section>Last section</Navbar.Section> */}
                </Navbar>
              }
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
