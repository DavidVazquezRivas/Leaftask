import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { queryClient } from '@/core/query/queryClient'

export const useChatPollingQuery = (interval = 5_000) =>
  useQuery({
    queryKey: [...QueryKeys.chat.all, 'polling'],
    queryFn: async () => {
      const messages = await ApiGateway.chat.pollMessages()

      const affectedChatIds = new Set(messages.map((m) => m.chatId))
      affectedChatIds.forEach((chatId) => {
        queryClient.invalidateQueries({
          queryKey: QueryKeys.chat.messages.list(chatId),
        })
      })

      if (messages.length > 0) {
        queryClient.invalidateQueries({
          queryKey: QueryKeys.chat.list(),
        })
      }

      return messages
    },
    refetchInterval: interval,
    staleTime: 0,
  })
