import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useDeleteProjectCustomFieldMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.customFields.all, 'delete', projectId],
    mutationFn: async (fieldId: string) =>
      ApiGateway.project.customFields.deleteField(projectId, fieldId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.project.customFields.list(projectId),
      })

      toast.success(
        i18n.t('management.customFields.feedback.deleted', {
          ns: 'projects',
          defaultValue: 'Custom field deleted successfully.',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
