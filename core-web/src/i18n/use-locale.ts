import * as React from "react"
import i18n from "i18next"
import dayjs from "dayjs"

export function useLocale() {
  const [language, setLanguage] = React.useState(i18n.language || "de")
  React.useEffect(() => {
    // we use i18n for language detection (see i18n.ts)
    const lang = i18n.language || "de"
    document.documentElement.lang = lang
    dayjs.locale(lang)
    setLanguage(lang)
    i18n.on("languageChanged", (lng) => {
      document.documentElement.lang = lng
      dayjs.locale(lng)
      setLanguage(lng)
    })
  }, [])
  return { language }
}
