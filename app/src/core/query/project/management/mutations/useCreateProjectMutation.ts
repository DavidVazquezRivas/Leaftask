import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { CreateProjectRequest } from '@/core/api/project/management'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'
import { toast } from 'sonner'

export const useCreateProjectMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.project.management.all, 'create'],
    mutationFn: (payload: CreateProjectRequest) =>
      ApiGateway.project.management.createProject(payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.project.management.all,
      })

      toast.success(
        i18n.t('management.create.feedback.created', { ns: 'projects' })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
