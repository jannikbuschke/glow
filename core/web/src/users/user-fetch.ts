import { fetchJson } from "../http/fetch"
import { User } from "./user-select"

export async function fetchUsers(search: string, customUrl?: string) {
  const response = await fetchJson<User[]>(
    customUrl
      ? `${customUrl}search=${search}`
      : `/api/tops/user-search?api-version=3.0&search=${search}`,
    { method: "GET", credentials: "same-origin" },
  )
  return response
}
