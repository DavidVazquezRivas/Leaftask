import { useMutation } from '@tanstack/react-query'

import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import { isApiErrorResponse } from '@/core/api/global/types/response'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useRespondProjectInvitationMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationFn: async ({
      projectId,
      invitationId,
      status,
    }: {
      projectId: string
      invitationId: string
      status: 'accepted' | 'rejected'
    }) => {
      const response = await apiClient.patch(
        ApiRoutes.Project.Invitations.Update(projectId, invitationId),
        { status }
      )
      const payload = response.data
      if (payload && isApiErrorResponse(payload)) {
        throw new ApiError(payload.error.code, {
          message: payload.error.message,
          status: response.status,
        })
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QueryKeys.notification.all })
      queryClient.invalidateQueries({ queryKey: QueryKeys.project.invitations.all })
    },
    onError: (error) => handleApiError(error),
  })
}
