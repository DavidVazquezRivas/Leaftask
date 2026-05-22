import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useWorkItemStatusesQuery = () => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.workItem.config.statuses,
    queryFn: () => ApiGateway.workItem.management.getWorkItemStatuses(),
    enabled: Boolean(accessToken),
    staleTime: Infinity,
  })
}
