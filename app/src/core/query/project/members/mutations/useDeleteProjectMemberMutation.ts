import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import {
  isOwnerProtectionError,
  useApiErrorHandler,
} from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useDeleteProjectMemberMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.members.all, 'delete', projectId],
    mutationFn: async (memberId: string) =>
      ApiGateway.project.members.deleteMember(projectId, memberId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.project.members.all,
      })

      toast.success(
        i18n.t('management.members.feedback.memberDeleted', {
          ns: 'projects',
          defaultValue: 'Member removed successfully.',
        })
      )
    },
    onError: (error) => {
      if (isOwnerProtectionError(error)) {
        toast.error(
          i18n.t('management.members.feedback.ownerCannotBeRemoved', {
            ns: 'projects',
            defaultValue: 'Owner members cannot be removed.',
          })
        )
        return
      }

      handleApiError(error)
    },
  })
}
