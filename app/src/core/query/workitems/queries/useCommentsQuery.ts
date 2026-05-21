import { useInfiniteQuery } from '@tanstack/react-query'
import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'

const PAGE_SIZE = 20

export const useCommentsQuery = (projectId: string, itemId: string) => {
  return useInfiniteQuery({
    queryKey: QueryKeys.workItem.comments.list(projectId, itemId),
    queryFn: ({ pageParam }) =>
      ApiGateway.workItem.comments.list(projectId, itemId, {
        limit: PAGE_SIZE,
        cursor: pageParam ?? null,
      }),
    initialPageParam: null as string | null,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
    enabled: !!projectId && !!itemId,
  })
}
