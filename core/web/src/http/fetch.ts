export async function fetchJson<T>(key: string) {
  const response = await fetch(key)
  if (response.ok) {
    const data = (await response.json()) as T
    return data
  } else {
    throw new Error(response.statusText + " (" + (await response.text()) + ")")
  }
}

export async function postJson<Response>(
  url: string,
  payload: any,
  headers?: any,
) {
  const response = await fetch(url, {
    method: "POST",
    headers: {
      "content-type": "application/json",
      ...(headers ? headers : {}),
    },
    body: JSON.stringify(payload),
  })
  if (response.ok) {
    const data = (await response.json()) as Response
    return data
  } else {
    throw new Error(response.statusText + " (" + (await response.text()) + ")")
  }
}
