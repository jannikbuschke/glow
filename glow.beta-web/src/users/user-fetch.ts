import { fetchJson, useFetchJson } from "glow-core"
import { User } from "./user-select"

// async function fetchUsers(search: string, customUrl?: string) {
//   const response = await fetchJson<User[]>(
//     customUrl
//       ? `${customUrl}search=${search}`
//       : `/api/tops/user-search?api-version=3.0&search=${search}`,
//     { method: "GET", credentials: "same-origin" },
//   )
//   return response
// }

export function useUsers() {
  const fetchJson = useFetchJson<User[]>()
  return async (search: string, customUrl?: string) => {
    const response = await fetchJson(
      customUrl
        ? `${customUrl}search=${search}`
        : `/api/tops/user-search?api-version=3.0&search=${search}`,
      { method: "GET", credentials: "same-origin" },
    )
    return response
  }
}
