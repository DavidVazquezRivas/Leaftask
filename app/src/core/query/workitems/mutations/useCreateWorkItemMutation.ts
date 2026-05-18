import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { CreateWorkItemRequest } from '@/core/api/workitems'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'
import { toast } from 'sonner'

export const useCreateWorkItemMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.workItem.management.all, 'create', projectId],
    mutationFn: (payload: CreateWorkItemRequest) =>
      ApiGateway.workItem.management.createWorkItem(projectId, payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.management.list(projectId),
      })

      toast.success(
        i18n.t('create.feedback.created', { ns: 'workitems' })
      )
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
