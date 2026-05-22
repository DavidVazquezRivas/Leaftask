import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { UpdateWorkLogRequest } from '@/core/api/workitems'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useUpdateWorkLogMutation = (projectId: string, itemId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.workItem.workLogs.all, 'update', projectId, itemId],
    mutationFn: ({ logId, payload }: { logId: string; payload: UpdateWorkLogRequest }) =>
      ApiGateway.workItem.workLogs.updateWorkLog(projectId, itemId, logId, payload),
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
