import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'

export const useRefreshSessionMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: QueryKeys.user.session.refresh(),
    mutationFn: async () => {
      return ApiGateway.user.session.refreshSession()
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
