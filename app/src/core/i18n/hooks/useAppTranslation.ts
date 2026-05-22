import { useTranslation } from 'react-i18next'

export const useAppTranslation = (namespace?: string | string[]) => {
  return useTranslation(namespace)
}
