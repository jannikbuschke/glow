import * as React from "react"
import { useEffect, useState } from "react"
// use
export function useData<T>(uri: string, placeholder?: T) {
  const [data, setData] = useState<T | null>(placeholder ? placeholder : null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState("")
  const [key, setKey] = useState(Math.random())

  useEffect(() => {
    setLoading(true)

    fetch(uri, {
      headers: { "content-type": "application/json" },
      credentials: "same-origin",
    })
      .then((r: any) => {
        if (r.ok) {
          setError("")
          const contentType = r.headers ? r.headers.get("content-type") : ""
          if (contentType && contentType.indexOf("application/json") !== -1) {
            return r
          }
          throw new Error(
            `expected application/json response but got '${contentType}'`,
          )
        } else {
          console.error("http error", r)
          throw Error(r.statusText)
        }
      })
      .then((response: any) => response.json())
      .then((data: any) => {
        setData(data)
        setLoading(false)
      })
      .catch((e: any) => {
        setError(e.toString())
        setLoading(false)
      })
  }, [uri, key])

  return { data, loading, error, reload: () => setKey(Math.random()) }
}
