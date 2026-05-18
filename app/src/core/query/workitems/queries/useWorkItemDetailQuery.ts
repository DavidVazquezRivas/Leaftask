import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useWorkItemDetailQuery = (
  projectId: string | null,
  itemId: string | null
) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.workItem.management.detail(projectId ?? '', itemId ?? ''),
    queryFn: () =>
      ApiGateway.workItem.management.getWorkItemDetail(
        projectId as string,
        itemId as string
      ),
    enabled: Boolean(accessToken && projectId && itemId),
  })
}
