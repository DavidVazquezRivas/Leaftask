import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'

export const useGoogleOAuthLoginMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: QueryKeys.user.session.loginWithGoogle(),
    mutationFn: async (token: string) => {
      return ApiGateway.user.session.loginWithGoogle(token)
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
