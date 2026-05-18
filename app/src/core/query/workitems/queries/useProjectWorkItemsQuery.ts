import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { GetWorkItemsSuccessResponse, WorkItemData } from '@/core/api/workitems'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useProjectWorkItemsQuery = (projectId: string | null) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.workItem.management.list(projectId ?? ''),
    queryFn: async (): Promise<GetWorkItemsSuccessResponse> => {
      const items: WorkItemData[] = []
      let cursor: string | null = null
      let lastMeta: GetWorkItemsSuccessResponse['meta']

      do {
        const response = await ApiGateway.workItem.management.getProjectWorkItems(
          projectId as string,
          { cursor }
        )
        items.push(...response.data)
        lastMeta = response.meta
        cursor = response.meta?.pagination?.nextCursor ?? null
      } while (cursor)

      return { data: items, meta: lastMeta }
    },
    enabled: Boolean(accessToken && projectId),
  })
}
