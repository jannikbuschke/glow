import * as React from "react"
import styled from "styled-components"

export const PageContainer = (props: any) => <div style={{}} {...props} />
export const PageHeader = (props: any) => <h1 {...props} />

export const PageContentContainer = (props: any) => <ErrorBoundary {...props} />

export const ActionContainer = styled.div`
  display: flex;
  margin-bottom: 8px;
  & > * {
    margin: 4px;
  }
`

export const PageActionContainer = (props: any) => (
  <ActionContainer {...props} />
)

export class ErrorBoundary extends React.Component {
  public state = { hasError: false }

  public componentDidCatch(error: any, info: any) {
    // Display fallback UI
    this.setState({ hasError: true })
    // You can also log the error to an error reporting service
    // tslint:disable-next-line:no-console
    console.error(error, info)
  }

  public render() {
    if (this.state.hasError) {
      // You can render any custom fallback UI
      return <h1>Something went wrong.</h1>
    }
    return this.props.children
  }
}
