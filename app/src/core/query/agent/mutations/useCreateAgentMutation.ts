import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { CreateAgentRequest } from '@/core/api/agent'
import type { GetProjectMembersSuccessResponse, ProjectMemberData } from '@/core/api/project/members'
import { queryClient } from '@/core/query/queryClient'
import { useApiErrorHandler } from '@/core/query/hooks'

const membersQueryPredicate = (projectId: string) => (query: { queryKey: readonly unknown[] }) =>
  query.queryKey.includes('members') && query.queryKey.includes(projectId)

export const useCreateAgentMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationFn: (body: CreateAgentRequest) => ApiGateway.agent.create(body),
    onSuccess: (agent, variables) => {
      const predicate = membersQueryPredicate(agent.projectId)

      // Optimistic update: add the new agent immediately without waiting for the outbox
      queryClient.setQueriesData<GetProjectMembersSuccessResponse>(
        { predicate },
        (old) => {
          if (!old) return old
          if (old.data.some((m) => m.id === agent.id)) return old
          const newMember: ProjectMemberData = {
            id: agent.id,
            name: agent.name,
            email: null,
            role: variables.roleId,
            type: 'agent',
          }
          return { ...old, data: [...old.data, newMember] }
        }
      )

      // Delayed invalidation so the refetch happens after the outbox has likely processed
      setTimeout(() => {
        queryClient.invalidateQueries({ predicate })
      }, 3000)
    },
    onError: (error) => handleApiError(error),
  })
}
