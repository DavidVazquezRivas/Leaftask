import { useMutation, type InfiniteData } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { ChatMessageData } from '@/core/api/chat'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

type MessagesPage = { data: ChatMessageData[]; nextCursor: string | null }

export const useSendMessageMutation = (chatId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationFn: (content: string) => ApiGateway.chat.sendMessage(chatId, { content }),
    onSuccess: (newMessage) => {
      queryClient.setQueryData<InfiniteData<MessagesPage>>(
        QueryKeys.chat.messages.list(chatId),
        (old) => {
          if (!old) return old
          const pages = old.pages.map((p, i) =>
            i === old.pages.length - 1 ? { ...p, data: [...p.data, newMessage] } : p
          )
          return { ...old, pages }
        }
      )
      queryClient.invalidateQueries({ queryKey: QueryKeys.chat.list() })
    },
    onError: (error) => handleApiError(error),
  })
}
