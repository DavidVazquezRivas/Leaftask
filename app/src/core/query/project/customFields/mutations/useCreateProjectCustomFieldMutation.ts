import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { CreateCustomFieldRequest } from '@/core/api/project/customFields'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useCreateProjectCustomFieldMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.customFields.all, 'create', projectId],
    mutationFn: async (payload: CreateCustomFieldRequest) =>
      ApiGateway.project.customFields.createField(projectId, payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.project.customFields.list(projectId),
      })

      toast.success(
        i18n.t('management.customFields.feedback.created', {
          ns: 'projects',
          defaultValue: 'Custom field created successfully.',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
