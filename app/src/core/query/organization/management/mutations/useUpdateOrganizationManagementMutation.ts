import { useMutation } from '@tanstack/react-query'
import { isAxiosError } from 'axios'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { ApiError } from '@/core/api/global/errors/ApiError'
import type { PatchOrganizationManagementRequest } from '@/core/api/organization/management'
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

export const useUpdateOrganizationManagementMutation = (
  organizationId: string
) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [
      ...QueryKeys.organization.management.all,
      'update',
      organizationId,
    ],
    mutationFn: async (payload: PatchOrganizationManagementRequest) => {
      return ApiGateway.organization.management.updateOrganizationById(
        organizationId,
        payload
      )
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.organization.management.detail(organizationId),
      })

      toast.success(
        i18n.t('management.settings.general.feedback.updated', {
          ns: 'organizations',
        })
      )
    },
    onError: (error) => {
      if (isForbiddenError(error)) {
        toast.info(
          i18n.t('management.settings.general.permissions.forbiddenAction', {
            ns: 'organizations',
          })
        )
        return
      }

      handleApiError(error)
    },
  })
}
