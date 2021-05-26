interface Translations {
  [lang: string]: {
    basedOn: "en" | "de"

    resources: { [key: string]: string }
  }
}

export async function fetchTranslations() {
  const response = await fetch("/languages/translations.json")
  if (response.ok) {
    const data = await response.json()
    console.log({ translations: data })
    return data as Translations
  } else {
    const error = await response.text()
    console.warn("could not load additional translations", { error })
    return null
  }
}
