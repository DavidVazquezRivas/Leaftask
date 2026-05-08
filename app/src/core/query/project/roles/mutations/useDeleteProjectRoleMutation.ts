import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { isForbiddenError, useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useDeleteProjectRoleMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.roles.all, 'delete', projectId],
    mutationFn: async (roleId: string) => {
      return ApiGateway.project.roles.deleteRole(projectId, roleId)
    },
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: QueryKeys.project.roles.list(projectId),
        }),
        queryClient.invalidateQueries({
          queryKey: QueryKeys.project.management.detail(projectId),
        }),
      ])

      toast.success(
        i18n.t('management.rolesPermissions.feedback.roleDeleted', {
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
