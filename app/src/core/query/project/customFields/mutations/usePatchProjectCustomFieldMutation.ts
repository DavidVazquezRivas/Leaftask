import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { PatchCustomFieldRequest } from '@/core/api/project/customFields'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const usePatchProjectCustomFieldMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.customFields.all, 'patch', projectId],
    mutationFn: async (payload: {
      fieldId: string
      data: PatchCustomFieldRequest
    }) =>
      ApiGateway.project.customFields.patchField(
        projectId,
        payload.fieldId,
        payload.data
      ),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.project.customFields.list(projectId),
      })

      toast.success(
        i18n.t('management.customFields.feedback.updated', {
          ns: 'projects',
          defaultValue: 'Custom field updated successfully.',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
