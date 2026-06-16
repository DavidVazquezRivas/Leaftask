import { toast } from 'sonner'

import { ApiError } from '@/core/api/global/errors/ApiError'
import { i18n, namespaces } from '@/core/i18n'

export const useApiErrorHandler = () => {
  return (error: unknown) => {
    if (error instanceof ApiError) {
      // 403s are handled globally in MutationCache — skip to avoid duplicate toasts
      if (error.status === 403) return ''

      const translatedMessage = i18n.t(error.code, {
        ns: namespaces,
        keySeparator: false,
        defaultValue: i18n.t('errors.unknown', { ns: 'global' }),
      })

      toast.error(translatedMessage)
      return translatedMessage
    }

    const fallbackMessage = i18n.t('errors.unknown', { ns: 'global' })
    toast.error(fallbackMessage)
    return fallbackMessage
  }
}
