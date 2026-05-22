import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'
import { toast } from 'sonner'

export const useDeleteProjectMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.management.detail(projectId), 'delete'],
    mutationFn: () => ApiGateway.project.management.deleteProject(projectId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.project.management.all,
      })

      toast.success(
        i18n.t('management.settings.general.feedback.deleted', {
          ns: 'projects',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
