import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useProjectPendingInvitationsQuery = (
  projectId: string | null
) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.project.invitations.pending(projectId ?? ''),
    queryFn: () =>
      ApiGateway.project.members.getPendingInvitations(projectId as string),
    enabled: Boolean(accessToken && projectId),
  })
}
