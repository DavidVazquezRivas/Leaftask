import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { LogWorkRequest } from '@/core/api/workitems'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useLogWorkMutation = (projectId: string, itemId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.workItem.workLogs.all, 'create', projectId, itemId],
    mutationFn: (payload: LogWorkRequest) =>
      ApiGateway.workItem.workLogs.logWork(projectId, itemId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.workLogs.list(projectId, itemId),
      })
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.management.detail(projectId, itemId),
      })
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.management.list(projectId),
      })
    },
    onError: (error) => handleApiError(error),
  })
}
