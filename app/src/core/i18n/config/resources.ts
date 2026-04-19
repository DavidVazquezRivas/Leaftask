import globalEn from '@/core/i18n/locales/en/global.json'
import userEn from '@/core/i18n/locales/en/user.json'
import globalEs from '@/core/i18n/locales/es/global.json'
import userEs from '@/core/i18n/locales/es/user.json'

export const defaultLocale = 'es'

export const resources = {
  en: {
    global: globalEn,
    user: userEn,
  },
  es: {
    global: globalEs,
    user: userEs,
  },
} as const

export const namespaces = Object.keys(resources[defaultLocale]) as Array<
  keyof (typeof resources)[typeof defaultLocale]
>
