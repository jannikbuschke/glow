export function sleep(timeout: number): Promise<void> {
  return new Promise((resolve) => {
    setTimeout(resolve, timeout)
  })
}

export function replaceAt<T>(array: T[], index: number, value: T): T[] {
  const copy = array.slice(0)
  copy[index] = value
  return copy
}
