import { useParams } from 'react-router-dom'
import { MessageSquare } from 'lucide-react'
import { useChatsQuery } from '@/core/query/chat'
import { useSessionMeQuery } from '@/core/query/user/session/queries/useSessionMeQuery'
import { ChatSidebar } from './components/ChatSidebar'
import { ChatView } from './components/ChatView'

export function ChatPage() {
  const { chatId } = useParams<{ chatId: string }>()

  const chatsQuery = useChatsQuery()
  const meQuery = useSessionMeQuery()

  const chats = chatsQuery.data ?? []
  const activeChat = chatId ? chats.find((c) => c.id === chatId) : undefined
  const currentUserId = meQuery.data?.id ?? ''

  return (
    <div className="flex h-full overflow-hidden">
      <ChatSidebar chats={chats} activeChatId={chatId} />

      <div className="flex flex-1 flex-col overflow-hidden">
        {activeChat ? (
          <ChatView chat={activeChat} currentUserId={currentUserId} />
        ) : (
          <div className="flex h-full flex-col items-center justify-center gap-3 text-muted-foreground">
            <MessageSquare size={48} className="opacity-20" />
            <p className="text-sm">Selecciona una conversación</p>
          </div>
        )}
      </div>
    </div>
  )
}
