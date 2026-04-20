import { useMutation } from '@tanstack/react-query'
import { isAxiosError } from 'axios'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { ApiError } from '@/core/api/global/errors/ApiError'
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

export const useDeleteOrganizationRoleMutation = (organizationId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [
      ...QueryKeys.organization.roles.all,
      'delete',
      organizationId,
    ],
    mutationFn: async (roleId: string) => {
      return ApiGateway.organization.roles.deleteRole(organizationId, roleId)
    },
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: QueryKeys.organization.roles.list(organizationId),
        }),
        queryClient.invalidateQueries({
          queryKey: QueryKeys.organization.management.detail(organizationId),
        }),
      ])

      toast.success(
        i18n.t('management.settings.rolesPermissions.feedback.roleDeleted', {
          ns: 'organizations',
        })
      )
    },
    onError: (error) => {
      if (isForbiddenError(error)) {
        toast.info(
          i18n.t(
            'management.settings.rolesPermissions.feedback.supervisedAction',
            {
              ns: 'organizations',
            }
          )
        )
        return
      }

      handleApiError(error)
    },
  })
}
