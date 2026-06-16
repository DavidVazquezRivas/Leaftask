import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useRespondInvitationMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationFn: ({
      organizationId,
      invitationId,
      status,
    }: {
      organizationId: string
      invitationId: string
      status: 'accepted' | 'rejected'
    }) =>
      ApiGateway.organization.invitations.respondToInvitation(organizationId, invitationId, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QueryKeys.notification.all })
      queryClient.invalidateQueries({ queryKey: QueryKeys.organization.invitations.all })
    },
    onError: (error) => handleApiError(error),
  })
}
