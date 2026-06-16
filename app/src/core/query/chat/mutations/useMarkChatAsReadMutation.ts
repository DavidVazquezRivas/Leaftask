import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { ChatData } from '@/core/api/chat'
import { QueryKeys } from '@/core/query/QueryKeys'
import { queryClient } from '@/core/query/queryClient'

export const useMarkChatAsReadMutation = () =>
  useMutation({
    mutationFn: (chatId: string) => ApiGateway.chat.markAsRead(chatId),
    onSuccess: (_, chatId) => {
      queryClient.setQueryData<ChatData[]>(QueryKeys.chat.list(), (old) =>
        old?.map((c) => (c.id === chatId ? { ...c, unreadCount: 0 } : c))
      )
    },
  })
