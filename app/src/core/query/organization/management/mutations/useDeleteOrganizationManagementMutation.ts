import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useDeleteOrganizationManagementMutation = (
  organizationId: string
) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [
      ...QueryKeys.organization.management.all,
      'delete',
      organizationId,
    ],
    mutationFn: async () => {
      return ApiGateway.organization.management.deleteOrganizationById(
        organizationId
      )
    },
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: QueryKeys.organization.management.detail(organizationId),
        }),
        queryClient.invalidateQueries({
          queryKey: QueryKeys.organization.management.all,
        }),
      ])

      toast.success(
        i18n.t('management.settings.general.feedback.deleted', {
          ns: 'organizations',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
