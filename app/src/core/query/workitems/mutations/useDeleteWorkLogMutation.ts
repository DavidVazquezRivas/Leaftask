import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useDeleteWorkLogMutation = (projectId: string, itemId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.workItem.workLogs.all, 'delete', projectId, itemId],
    mutationFn: (logId: string) =>
      ApiGateway.workItem.workLogs.deleteWorkLog(projectId, itemId, logId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.workLogs.list(projectId, itemId),
      })
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.management.detail(projectId, itemId),
      })
    },
    onError: (error) => handleApiError(error),
  })
}
