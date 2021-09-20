interface Translations {
  [lang: string]: {
    basedOn: "en" | "de"

    resources: { [key: string]: string }
  }
}

export async function fetchTranslations() {
  const response = await fetch("/languages/translations.json")
  if (response.ok) {
    try {
      const data = await response.json()
      console.log({ translations: data })
      return data as Translations
    } catch (e) {
      console.warn("could not load additional translations", { error: e })
      return null
    }
  } else {
    const error = await response.text()
    console.warn("could not load additional translations", { error })
    return null
  }
}

interface TranslationCollection {
  id: string
  values: [
    {
      languageId: string
      displayName: string
      basedOn: string
      resources: { [key: string]: string }
    },
  ]
}

export async function fetchTranslationCollection() {
  const response = await fetch("/api/translations/get", {
    method: "POST",
    body: JSON.stringify({}),
    credentials: "same-origin",
    headers: {
      "x-submit-intent": "execute",
      "content-type": "application/json",
    },
  })
  if (response.ok) {
    try {
      const data = await response.json()
      console.log({ translations: data })
      return data as TranslationCollection
    } catch (e) {
      console.warn("could not load additional translations", { error: e })
      return null
    }
  } else {
    const error = await response.text()
    console.warn("could not load additional translations", { error })
    return null
  }
}
