import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { PatchProjectRoleRequest } from '@/core/api/project/roles'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { isForbiddenError, useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useUpdateProjectRoleMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.roles.all, 'update', projectId],
    mutationFn: async (payload: {
      roleId: string
      data: PatchProjectRoleRequest
    }) => {
      return ApiGateway.project.roles.updateRole(
        projectId,
        payload.roleId,
        payload.data
      )
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.project.roles.list(projectId),
      })

      toast.success(
        i18n.t('management.rolesPermissions.feedback.roleUpdated', {
          ns: 'projects',
        })
      )
    },
    onError: (error) => {
      if (isForbiddenError(error)) {
        toast.info(
          i18n.t(
            'management.rolesPermissions.feedback.supervisedAction',
            { ns: 'projects' }
          )
        )
        return
      }

      handleApiError(error)
    },
  })
}
