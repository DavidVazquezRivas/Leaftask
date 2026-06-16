import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { PatchOrganizationRoleRequest } from '@/core/api/organization/roles'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useUpdateOrganizationRoleMutation = (organizationId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [
      ...QueryKeys.organization.roles.all,
      'update',
      organizationId,
    ],
    mutationFn: async (payload: {
      roleId: string
      data: PatchOrganizationRoleRequest
    }) => {
      return ApiGateway.organization.roles.updateRole(
        organizationId,
        payload.roleId,
        payload.data
      )
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.organization.roles.list(organizationId),
      })

      toast.success(
        i18n.t('management.settings.rolesPermissions.feedback.roleUpdated', {
          ns: 'organizations',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
