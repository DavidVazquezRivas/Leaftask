import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useDeleteWorkItemMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.workItem.management.all, 'delete', projectId],
    mutationFn: (itemId: string) =>
      ApiGateway.workItem.management.deleteWorkItem(projectId, itemId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.management.list(projectId),
      })
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
