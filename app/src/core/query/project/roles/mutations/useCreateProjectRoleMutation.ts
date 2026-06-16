import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { CreateProjectRoleRequest } from '@/core/api/project/roles'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useCreateProjectRoleMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.roles.all, 'create', projectId],
    mutationFn: async (payload: CreateProjectRoleRequest) => {
      return ApiGateway.project.roles.createRole(projectId, payload)
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
        i18n.t('management.rolesPermissions.feedback.roleCreated', {
          ns: 'projects',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
