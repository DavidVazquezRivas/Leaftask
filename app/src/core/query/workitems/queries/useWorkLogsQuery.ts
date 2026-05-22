import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { WorkLogData } from '@/core/api/workitems'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useWorkLogsQuery = (
  projectId: string | null,
  itemId: string | null
) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.workItem.workLogs.list(projectId ?? '', itemId ?? ''),
    queryFn: async (): Promise<WorkLogData[]> => {
      const logs: WorkLogData[] = []
      let cursor: string | null = null

      do {
        const response = await ApiGateway.workItem.workLogs.getWorkLogs(
          projectId as string,
          itemId as string,
          { limit: 50, cursor }
        )
        logs.push(...response.data)
        cursor = response.meta?.pagination?.nextCursor ?? null
      } while (cursor)

      return logs
    },
    enabled: Boolean(accessToken && projectId && itemId),
  })
}
