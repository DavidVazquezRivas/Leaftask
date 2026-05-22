import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useWorkItemTypesQuery = () => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.workItem.config.types,
    queryFn: () => ApiGateway.workItem.management.getWorkItemTypes(),
    enabled: Boolean(accessToken),
    staleTime: Infinity,
  })
}
