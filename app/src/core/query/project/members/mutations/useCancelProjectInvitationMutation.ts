import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useCancelProjectInvitationMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.invitations.all, 'cancel', projectId],
    mutationFn: async (invitationId: string) =>
      ApiGateway.project.members.updateInvitationStatus(
        projectId,
        invitationId,
        { status: 'CANCELLED' }
      ),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.project.invitations.pending(projectId),
      })

      toast.success(
        i18n.t('management.members.pendingInvitations.canceled', {
          ns: 'projects',
          defaultValue: 'Invitation canceled successfully.',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
