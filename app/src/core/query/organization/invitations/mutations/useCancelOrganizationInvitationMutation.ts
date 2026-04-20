import { isAxiosError } from 'axios'
import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { ApiError } from '@/core/api/global/errors/ApiError'
import type { CancelOrganizationInvitationRequest } from '@/core/api/organization/invitations'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

const isForbiddenError = (error: unknown): boolean => {
  if (error instanceof ApiError) {
    return error.status === 403
  }

  return isAxiosError(error) && error.response?.status === 403
}

export const useCancelOrganizationInvitationMutation = (
  organizationId: string
) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [
      ...QueryKeys.organization.invitations.all,
      'cancel',
      organizationId,
    ],
    mutationFn: async (payload: {
      invitationId: string
      data: CancelOrganizationInvitationRequest
    }) => {
      return ApiGateway.organization.invitations.cancelInvitation(
        organizationId,
        payload.invitationId,
        payload.data
      )
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.organization.invitations.pending(organizationId),
      })

      toast.success(
        i18n.t('management.settings.members.pendingInvitations.canceled', {
          ns: 'organizations',
          defaultValue: 'Invitation canceled successfully.',
        })
      )
    },
    onError: (error) => {
      if (isForbiddenError(error)) {
        toast.info(
          i18n.t('management.settings.members.permissions.noCancelInvitation', {
            ns: 'organizations',
            defaultValue: "You don't have permission to cancel invitations.",
          })
        )
        return
      }

      handleApiError(error)
    },
  })
}
