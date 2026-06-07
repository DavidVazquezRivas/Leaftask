import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { queryClient } from '@/core/query/queryClient'
import { useApiErrorHandler } from '@/core/query/hooks'

export const useDeleteAgentMutation = (projectId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationFn: (agentId: string) => ApiGateway.agent.delete(agentId, projectId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        predicate: (query) =>
          query.queryKey.includes('members') &&
          query.queryKey.includes(projectId),
      })
    },
    onError: (error) => handleApiError(error),
  })
}
