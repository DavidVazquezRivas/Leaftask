import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { PatchProjectMemberRoleRequest } from '@/core/api/project/members'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useUpdateProjectMemberRoleMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.members.all, 'update', projectId],
    mutationFn: async (payload: {
      memberId: string
      data: PatchProjectMemberRoleRequest
    }) =>
      ApiGateway.project.members.updateMemberRole(
        projectId,
        payload.memberId,
        payload.data
      ),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.project.members.all,
      })

      toast.success(
        i18n.t('management.members.feedback.roleUpdated', {
          ns: 'projects',
          defaultValue: 'Member role updated successfully.',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
