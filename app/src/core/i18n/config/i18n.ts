import i18n from 'i18next'
import { initReactI18next } from 'react-i18next'

import {
  defaultLocale,
  namespaces,
  resources,
} from '@/core/i18n/config/resources'

if (!i18n.isInitialized) {
  void i18n.use(initReactI18next).init({
    resources,
    lng: defaultLocale,
    fallbackLng: defaultLocale,
    defaultNS: 'global',
    ns: namespaces,
    interpolation: {
      escapeValue: false,
    },
    returnNull: false,
  })
}

export { i18n }
