import { useEffect, useRef } from 'react'
import { Bot, MessageSquare, User } from 'lucide-react'
import type { ChatData } from '@/core/api/chat'
import { useChatMessagesQuery, useMarkChatAsReadMutation, useSendMessageMutation } from '@/core/query/chat'
import { MessageBubble } from './MessageBubble'
import { MessageInput } from './MessageInput'

interface ChatViewProps {
  chat: ChatData
  currentUserId: string
}

export function ChatView({ chat, currentUserId }: ChatViewProps) {
  const messagesQuery = useChatMessagesQuery(chat.id)
  const sendMutation = useSendMessageMutation(chat.id)
  const markAsRead = useMarkChatAsReadMutation()
  const bottomRef = useRef<HTMLDivElement>(null)

  const allMessages = messagesQuery.data?.pages.flatMap((p) => p.data) ?? []

  useEffect(() => {
    if (chat.unreadCount > 0) markAsRead.mutate(chat.id)
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chat.id])

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [allMessages.length])

  return (
    <div className="flex h-full flex-col">
      <div className="flex items-center gap-3 border-b border-border px-4 py-3">
        <div className="flex size-8 items-center justify-center rounded-full bg-muted text-muted-foreground">
          {chat.type === 'agent' ? <Bot size={16} /> : <User size={16} />}
        </div>
        <div>
          <p className="text-sm font-medium">{chat.name}</p>
          <p className="text-xs text-muted-foreground capitalize">{chat.type === 'agent' ? 'Agente' : 'Persona'}</p>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-4 py-4">
        {messagesQuery.hasNextPage && (
          <button
            onClick={() => messagesQuery.fetchNextPage()}
            disabled={messagesQuery.isFetchingNextPage}
            className="mb-4 w-full text-center text-xs text-muted-foreground hover:text-foreground"
          >
            {messagesQuery.isFetchingNextPage ? 'Cargando...' : 'Cargar más mensajes'}
          </button>
        )}

        {allMessages.length === 0 && !messagesQuery.isLoading && (
          <div className="flex h-full flex-col items-center justify-center gap-2 text-muted-foreground">
            <MessageSquare size={32} className="opacity-30" />
            <p className="text-sm">Sin mensajes aún. ¡Di hola!</p>
          </div>
        )}

        <div className="flex flex-col gap-3">
          {allMessages.map((msg) => (
            <MessageBubble
              key={msg.id}
              message={msg}
              isOwn={msg.sender?.id === currentUserId}
            />
          ))}
        </div>

        <div ref={bottomRef} />
      </div>

      <MessageInput
        onSend={(content) => sendMutation.mutate(content)}
        disabled={sendMutation.isPending}
      />
    </div>
  )
}
