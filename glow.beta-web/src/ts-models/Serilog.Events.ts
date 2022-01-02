/* eslint-disable prettier/prettier */
export type LogEventLevel = "Verbose" | "Debug" | "Information" | "Warning" | "Error" | "Fatal"
export const defaultLogEventLevel = "Verbose"
export const LogEventLevelValues: { [key in LogEventLevel]: LogEventLevel } = {
  Verbose: "Verbose",
  Debug: "Debug",
  Information: "Information",
  Warning: "Warning",
  Error: "Error",
  Fatal: "Fatal",
}
export const LogEventLevelValuesArray: LogEventLevel[] = Object.keys(LogEventLevelValues) as LogEventLevel[]

