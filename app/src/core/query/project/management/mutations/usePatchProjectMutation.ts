import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { PatchProjectRequest } from '@/core/api/project/management'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'
import { toast } from 'sonner'

export const usePatchProjectMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.management.detail(projectId), 'patch'],
    mutationFn: (payload: PatchProjectRequest) =>
      ApiGateway.project.management.patchProject(projectId, payload),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: QueryKeys.project.management.detail(projectId),
        }),
        queryClient.invalidateQueries({
          queryKey: QueryKeys.project.management.all,
        }),
      ])

      toast.success(
        i18n.t('management.settings.general.feedback.updated', {
          ns: 'projects',
        })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
