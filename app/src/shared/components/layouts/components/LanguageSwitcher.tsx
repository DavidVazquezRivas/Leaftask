import { resources, useAppTranslation } from '@/core/i18n'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

const supportedLocales = Object.keys(resources)

const toSentenceCase = (value: string): string => {
  if (!value) {
    return value
  }

  return value.charAt(0).toUpperCase() + value.slice(1)
}

export function LanguageSwitcher() {
  const { i18n } = useAppTranslation('global')
  const activeLanguage = (i18n.resolvedLanguage ?? i18n.language).split('-')[0]
  const languageDisplay = new Intl.DisplayNames([activeLanguage], {
    type: 'language',
  })

  return (
    <Select
      value={activeLanguage}
      onValueChange={(locale) => {
        void i18n.changeLanguage(locale)
      }}
    >
      <SelectTrigger className="min-w-28" size="sm">
        <SelectValue />
      </SelectTrigger>

      <SelectContent>
        {supportedLocales.map((locale) => {
          const rawLabel = languageDisplay.of(locale) ?? locale.toUpperCase()
          const label = toSentenceCase(rawLabel)

          return (
            <SelectItem key={locale} value={locale}>
              {label}
            </SelectItem>
          )
        })}
      </SelectContent>
    </Select>
  )
}
