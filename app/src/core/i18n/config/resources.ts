import globalEn from '@/core/i18n/locales/en/global.json'
import organizationsEn from '@/core/i18n/locales/en/organizations.json'
import projectsEn from '@/core/i18n/locales/en/projects.json'
import userEn from '@/core/i18n/locales/en/user.json'
import globalEs from '@/core/i18n/locales/es/global.json'
import organizationsEs from '@/core/i18n/locales/es/organizations.json'
import projectsEs from '@/core/i18n/locales/es/projects.json'
import userEs from '@/core/i18n/locales/es/user.json'

export const defaultLocale = 'es'

export const resources = {
  en: {
    global: globalEn,
    organizations: organizationsEn,
    projects: projectsEn,
    user: userEn,
  },
  es: {
    global: globalEs,
    organizations: organizationsEs,
    projects: projectsEs,
    user: userEs,
  },
} as const

export const namespaces = Object.keys(resources[defaultLocale]) as Array<
  keyof (typeof resources)[typeof defaultLocale]
>
