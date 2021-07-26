import * as React from "react"
import enUs from "antd/es/locale/en_US"
import deDe from "antd/es/locale/de_DE"
import i18n from "i18next"
import dayjs from "dayjs"

function getLocale(lang: string) {
  switch (lang.toLowerCase()) {
    case "de-DE":
    case "de-De":
    case "dede":
    case "de":
      return deDe
    case "en-Us":
    case "en-US":
    case "enen":
    case "enus":
    case "en":
      return enUs
  }
  return deDe
}

export function useLocale({
  onLanguageChanged,
}: {
  onLanguageChanged: (lang: string) => void
}) {
  const [locale, setLocale] = React.useState(enUs)
  React.useEffect(() => {
    // we use i18n for language detection (see i18n.ts)
    const lang = i18n.language || "de"
    document.documentElement.lang = lang
    dayjs.locale(lang)
    setLocale(getLocale(lang))
    onLanguageChanged(lang)
    i18n.on("languageChanged", (lng) => {
      document.documentElement.lang = lng
      dayjs.locale(lng)
      setLocale(getLocale(lng))
      onLanguageChanged(lng)
    })
  }, [])
  return locale
}
