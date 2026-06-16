import type { ChatMessageData } from '@/core/api/chat'
import { getInitials } from '@/shared/lib/text'
import { formatRelative } from '@/shared/lib/date'
import { useTranslation } from 'react-i18next'

interface MessageBubbleProps {
  message: ChatMessageData
  isOwn: boolean
}

export function MessageBubble({ message, isOwn }: MessageBubbleProps) {
  const { i18n } = useTranslation()

  return (
    <div className={`flex gap-2 ${isOwn ? 'flex-row-reverse' : 'flex-row'}`}>
      {!isOwn && (
        <div className="flex size-7 shrink-0 items-center justify-center rounded-full bg-muted text-xs font-medium text-muted-foreground">
          {getInitials(message.sender?.name ?? null)}
        </div>
      )}

      <div className={`flex max-w-[70%] flex-col gap-1 ${isOwn ? 'items-end' : 'items-start'}`}>
        {!isOwn && message.sender && (
          <span className="px-1 text-xs text-muted-foreground">{message.sender.name}</span>
        )}
        <div
          className={`rounded-2xl px-3 py-2 text-sm ${
            isOwn
              ? 'rounded-tr-sm bg-primary text-primary-foreground'
              : 'rounded-tl-sm bg-muted text-foreground'
          }`}
        >
          {message.content}
        </div>
        <span className="px-1 text-[10px] text-muted-foreground">
          {formatRelative(message.timestamp, i18n.language)}
        </span>
      </div>
    </div>
  )
}
