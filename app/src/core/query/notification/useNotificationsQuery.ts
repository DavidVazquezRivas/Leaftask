import { useInfiniteQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'

const PAGE_SIZE = 20

export const useNotificationsQuery = (status: 'all' | 'unread' = 'all') =>
  useInfiniteQuery({
    queryKey: QueryKeys.notification.list({ status }),
    queryFn: ({ pageParam }) =>
      ApiGateway.notification.listNotifications({
        limit: PAGE_SIZE,
        cursor: pageParam as string | null,
        status,
      }),
    initialPageParam: null as string | null,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
  })
