import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { UpdateWorkItemRequest } from '@/core/api/workitems'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useUpdateWorkItemMutation = (
  projectId: string,
  itemId: string
) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.workItem.management.all, 'update', projectId, itemId],
    mutationFn: (payload: UpdateWorkItemRequest) =>
      ApiGateway.workItem.management.updateWorkItem(projectId, itemId, payload),
    onSuccess: (response) => {
      queryClient.setQueryData(
        QueryKeys.workItem.management.detail(projectId, itemId),
        response
      )
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.management.list(projectId),
      })
    },
    onError: (error) => {
      handleApiError(error)
    },
  })
}
