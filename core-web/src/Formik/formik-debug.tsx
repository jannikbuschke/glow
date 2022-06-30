import { Field, useFormikContext } from "formik"
import * as React from "react"
import { useLocalStorage } from "../local-storage"

type Environment = "development" | "test" | "production" | string

type IsDevelopment = () => boolean

const defaultContext = {
  environment: "production",
  isDevelopment: () => false,
}
const environmentContext = React.createContext(defaultContext)

export function useEnvironment() {
  const ctx = React.useContext(environmentContext)
  return ctx
}

export function EnvironmentProvider({
  children,
  environment,
  isDevelopment,
}: {
  children: React.ReactNode
  environment?: Environment
  isDevelopment?: IsDevelopment
}) {
  const value = React.useMemo(() => {
    const env: Environment = environment || "production"
    return {
      environment: env,
      isDevelopment: isDevelopment
        ? isDevelopment
        : () => env === "development",
    }
  }, [environment, isDevelopment])

  return (
    <environmentContext.Provider value={value}>
      {children}
    </environmentContext.Provider>
  )
}

export function ToggleWrapper({ children }: { children: React.ReactElement }) {
  const { isDevelopment } = useEnvironment()
  const [showFormContent, setShowFormContent] = React.useState(false)
  const [d] = useLocalStorage("gertrud_d", "false")
  const [a] = useLocalStorage("gertrud_d_autoshow", "false")
  const debug = d === "true"
  const autoShow = a === "true"
  return isDevelopment() || showFormContent || autoShow ? (
    children
  ) : debug ? (
    <button style={{ color: "blue" }} onClick={() => setShowFormContent(true)}>
      +
    </button>
  ) : null
}

export function StringifyProps({ style, ...props }: any) {
  return (
    <ToggleWrapper>
      <pre style={style}>{JSON.stringify(props, null, 2)}</pre>
    </ToggleWrapper>
  )
}

export function FormikValues(
  props: React.DetailedHTMLProps<
    React.HTMLAttributes<HTMLDivElement>,
    HTMLDivElement
  >,
) {
  const ctx = useFormikContext()
  return (
    <ToggleWrapper>
      <pre style={{ padding: 15, ...props }}>
        {JSON.stringify(ctx.values, null, 2)}
      </pre>
    </ToggleWrapper>
  )
}

export function FormikDebug(
  props: React.DetailedHTMLProps<
    React.HTMLAttributes<HTMLDivElement>,
    HTMLDivElement
  >,
) {
  const ctx = useFormikContext()

  return (
    <ToggleWrapper>
      <pre style={{ padding: 15, ...props }}>
        {JSON.stringify(ctx, null, 2)}
      </pre>
    </ToggleWrapper>
  )
}

export default FormikDebug
