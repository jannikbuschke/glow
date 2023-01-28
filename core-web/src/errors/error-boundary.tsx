import { Alert } from "@mantine/core"
import React from "react"
import { RenderObject } from "../debugging"

export class ErrorBoundary extends React.Component<{},{hasError:boolean,error:any,errorInfo:any}> {
  constructor(props) {
    super(props)
    this.state = { hasError: false,error:null,errorInfo:null }
  }

  static getDerivedStateFromError(error) {
    // Update state so the next render will show the fallback UI.
    return { hasError: true }
  }

  override componentDidCatch(error, errorInfo) {
    console.log(error,errorInfo)
    this.setState(s=>({...s,error,errorInfo}))
    // You can also log the error to an error reporting service
    // logErrorToMyService(error, errorInfo)
  }

  override render() {
    if (this.state.hasError) {
      // You can render any custom fallback UI
      return <Alert color="red" title="Something went wrong."><RenderObject {...this.state} /></Alert>
    }

    return this.props.children
  }
}
