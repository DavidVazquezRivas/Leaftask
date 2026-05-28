import { useNavigate } from 'react-router-dom'
import { Bot, User } from 'lucide-react'
import type { ChatData } from '@/core/api/chat'
import { AppPaths } from '@/core/router/paths'
import { getInitials } from '@/shared/lib/text'
import { formatRelative } from '@/shared/lib/date'
import { useTranslation } from 'react-i18next'

interface ChatListItemProps {
  chat: ChatData
  isActive: boolean
}

export function ChatListItem({ chat, isActive }: ChatListItemProps) {
  const navigate = useNavigate()
  const { i18n } = useTranslation()

  return (
    <button
      onClick={() => navigate(AppPaths.chatDetail(chat.id))}
      className={`flex w-full items-start gap-3 rounded-lg px-3 py-2.5 text-left transition-colors hover:bg-accent/50 ${
        isActive ? 'bg-accent' : ''
      }`}
    >
      <div className="relative flex size-9 shrink-0 items-center justify-center rounded-full bg-muted text-sm font-medium text-muted-foreground">
        {getInitials(chat.name)}
        <div className="absolute -bottom-0.5 -right-0.5 flex size-4 items-center justify-center rounded-full bg-card text-muted-foreground">
          {chat.type === 'agent' ? <Bot size={10} /> : <User size={10} />}
        </div>
      </div>

      <div className="min-w-0 flex-1">
        <div className="flex items-baseline justify-between gap-2">
          <span className={`truncate text-sm ${chat.unreadCount > 0 ? 'font-semibold' : 'font-medium'}`}>
            {chat.name}
          </span>
          {chat.lastMessage && (
            <span className={`shrink-0 text-[10px] ${chat.unreadCount > 0 ? 'font-medium text-foreground' : 'text-muted-foreground'}`}>
              {formatRelative(chat.lastMessage.timestamp, i18n.language)}
            </span>
          )}
        </div>
        <div className="flex items-center justify-between gap-2">
          {chat.lastMessage ? (
            <p className={`truncate text-xs ${chat.unreadCount > 0 ? 'text-foreground' : 'text-muted-foreground'}`}>
              {chat.lastMessage.content}
            </p>
          ) : (
            <span />
          )}
          {chat.unreadCount > 0 && (
            <span className="flex h-4 min-w-4 shrink-0 items-center justify-center rounded-full bg-primary px-1 text-[10px] font-semibold leading-none text-primary-foreground">
              {chat.unreadCount > 99 ? '99+' : chat.unreadCount}
            </span>
          )}
        </div>
      </div>
    </button>
  )
}
