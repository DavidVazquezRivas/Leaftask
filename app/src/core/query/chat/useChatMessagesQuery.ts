import { useInfiniteQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'

const PAGE_SIZE = 30

export const useChatMessagesQuery = (chatId: string) =>
  useInfiniteQuery({
    queryKey: QueryKeys.chat.messages.list(chatId),
    queryFn: ({ pageParam }) =>
      ApiGateway.chat.listMessages(chatId, {
        limit: PAGE_SIZE,
        cursor: pageParam as string | null,
      }),
    initialPageParam: null as string | null,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
    enabled: !!chatId,
  })
