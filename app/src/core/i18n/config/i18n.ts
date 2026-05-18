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
} else {
  // HMR: push updated resource bundles into the existing i18n instance
  for (const [lng, nss] of Object.entries(resources as Record<string, Record<string, object>>)) {
    for (const [ns, bundle] of Object.entries(nss)) {
      i18n.addResourceBundle(lng, ns, bundle, true, true)
    }
  }
}

export { i18n }
