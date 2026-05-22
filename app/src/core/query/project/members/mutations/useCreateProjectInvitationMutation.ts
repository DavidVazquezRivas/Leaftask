import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { ApiError } from '@/core/api/global/errors/ApiError'
import type { CreateProjectInvitationRequest } from '@/core/api/project/members'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { isForbiddenError, useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useCreateProjectInvitationMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.invitations.all, 'create', projectId],
    mutationFn: async (payload: CreateProjectInvitationRequest) =>
      ApiGateway.project.members.createInvitation(projectId, payload),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: QueryKeys.project.members.all,
        }),
        queryClient.invalidateQueries({
          queryKey: QueryKeys.project.invitations.pending(projectId),
        }),
      ])

      toast.success(
        i18n.t('management.members.feedback.invitationSent', {
          ns: 'projects',
          defaultValue: 'Invitation sent successfully.',
        })
      )
    },
    onError: (error) => {
      if (isForbiddenError(error)) {
        toast.info(
          i18n.t('management.members.permissions.noInvite', {
            ns: 'projects',
            defaultValue: "You don't have permission to invite members.",
          })
        )
        return
      }

      if (error instanceof ApiError && error.status === 409) {
        toast.error(
          i18n.t('management.members.feedback.alreadyInvited', {
            ns: 'projects',
            defaultValue: 'This user already has a pending invitation.',
          })
        )
        return
      }

      handleApiError(error)
    },
  })
}
