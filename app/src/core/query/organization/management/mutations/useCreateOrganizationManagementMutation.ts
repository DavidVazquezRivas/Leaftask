import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { CreateOrganizationManagementRequest } from '@/core/api/organization/management'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'
import { toast } from 'sonner'

export const useCreateOrganizationManagementMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.organization.management.all, 'create'],
    mutationFn: async (payload: CreateOrganizationManagementRequest) => {
      return ApiGateway.organization.management.createOrganization(payload)
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.organization.management.all,
      })

      toast.success(
        i18n.t('management.settings.general.feedback.created', {
          ns: 'organizations',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
