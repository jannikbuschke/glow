import React from "react"
import ReactDOM from "react-dom"
import App from "./app"
import "./main.css"

import dayjs from "dayjs"
import LocalizedFormat from "dayjs/plugin/localizedFormat"
import dayjsDuration from "dayjs/plugin/duration"
import relativeTime from "dayjs/plugin/relativeTime"
import timezone from "dayjs/plugin/timezone"
import utc from "dayjs/plugin/utc"
import "dayjs/locale/de"
import "dayjs/locale/en"

// is also done in shared/src/index.ts
dayjs.extend(LocalizedFormat)
dayjs.extend(dayjsDuration)
dayjs.extend(relativeTime)
dayjs.extend(utc)
dayjs.extend(timezone)

ReactDOM.render(<App />, document.getElementById("root"))
// ReactDOM.createRoot(document.getElementById("root")!).render(
//   <React.StrictMode>
//     <App />
//   </React.StrictMode>
// );
