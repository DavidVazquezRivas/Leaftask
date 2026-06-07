import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { CreateAgentRequest } from '@/core/api/agent'
import { QueryKeys } from '@/core/query/QueryKeys'
import { queryClient } from '@/core/query/queryClient'
import { useApiErrorHandler } from '@/core/query/hooks'

export const useCreateAgentMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationFn: (body: CreateAgentRequest) => ApiGateway.agent.create(body),
    onSuccess: (agent) => {
      queryClient.invalidateQueries({
        predicate: (query) =>
          query.queryKey.includes('members') &&
          query.queryKey.includes(agent.projectId),
      })
    },
    onError: (error) => handleApiError(error),
  })
}
