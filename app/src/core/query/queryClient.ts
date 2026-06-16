import { MutationCache, QueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { ApiError } from '@/core/api/global/errors/ApiError'
import { i18n } from '@/core/i18n'

export const queryClient = new QueryClient({
  mutationCache: new MutationCache({
    onError: (error) => {
      if (!(error instanceof ApiError) || error.status !== 403) return

      if (error.code.endsWith('.ApprovalRequired')) {
        toast.info(
          i18n.t('errors.approvalRequested', {
            ns: 'global',
            defaultValue: 'Tu solicitud ha sido enviada y está pendiente de aprobación.',
          })
        )
        return
      }

      toast.error(
        i18n.t('errors.forbidden', {
          ns: 'global',
          defaultValue: 'No tienes permiso para realizar esta acción.',
        })
      )
    },
  }),
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 1000 * 30,
    },
    mutations: {
      retry: 0,
    },
  },
})
