import { useInfiniteQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'

const PAGE_SIZE = 20

export const useApprovalsQuery = () =>
  useInfiniteQuery({
    queryKey: QueryKeys.approval.list(),
    queryFn: ({ pageParam }) =>
      ApiGateway.notification.listApprovals({
        limit: PAGE_SIZE,
        cursor: pageParam as string | null,
      }),
    initialPageParam: null as string | null,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
  })
