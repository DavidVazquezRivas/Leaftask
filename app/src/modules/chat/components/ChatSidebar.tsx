import { useState } from 'react'
import { MessageSquare, Search, Settings } from 'lucide-react'
import { Link, useNavigate } from 'react-router-dom'
import type { ChatData } from '@/core/api/chat'
import { AppPaths } from '@/core/router/paths'
import { useAppTranslation } from '@/core/i18n'
import { usePrivateLayoutSession } from '@/shared/components/layouts/hooks/usePrivateLayoutSession'
import { Button } from '@/shared/components/ui/button'
import { ChatListItem } from './ChatListItem'
import { NewChatDialog } from './NewChatDialog'
import { NewAgentChatDialog } from './NewAgentChatDialog'

interface ChatSidebarProps {
  chats: ChatData[]
  activeChatId?: string
}

function SectionHeader({
  label,
  action,
}: {
  label: string
  action?: React.ReactNode
}) {
  return (
    <div className="mb-1 flex items-center justify-between">
      <p className="text-[10px] font-semibold uppercase tracking-widest text-muted-foreground">
        {label}
      </p>
      {action}
    </div>
  )
}

export function ChatSidebar({ chats, activeChatId }: ChatSidebarProps) {
  const [search, setSearch] = useState('')
  const navigate = useNavigate()
  const { displayName } = usePrivateLayoutSession()
  const { t } = useAppTranslation('global')

  const filtered = search
    ? chats.filter((c) => c.name.toLowerCase().includes(search.toLowerCase()))
    : chats

  const usuarios = filtered.filter((c) => c.type === 'person')
  const agentes = filtered.filter((c) => c.type === 'agent')

  const handleCreated = (chatId: string) => {
    navigate(AppPaths.chatDetail(chatId))
  }

  const handleAgentChatCreated = (chatId: string) => {
    navigate(AppPaths.chatDetail(chatId))
  }

  return (
    <aside className="h-full w-76 shrink-0 border-r bg-card/95">
      <div className="flex h-full flex-col overflow-hidden">
        <header className="flex min-h-16 items-center border-b px-5 py-3">
          <div className="flex min-w-0 items-center gap-3">
            <div className="grid size-8 place-items-center rounded-md bg-muted text-muted-foreground">
              <MessageSquare className="size-4" />
            </div>
            <p className="truncate text-sm font-semibold">Mensajes</p>
          </div>
        </header>

        <div className="border-b px-5 py-3">
          <div className="flex items-center gap-2 rounded-md border border-border bg-muted/40 px-2.5 py-1.5">
            <Search size={13} className="shrink-0 text-muted-foreground" />
            <input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Buscar..."
              className="flex-1 bg-transparent text-sm outline-none placeholder:text-muted-foreground"
            />
          </div>
        </div>

        <section className="min-h-0 flex-1 overflow-y-auto px-5 py-4">
          <div className="mb-5">
            <SectionHeader
              label="Usuarios"
              action={<NewChatDialog onCreated={handleCreated} />}
            />
            {usuarios.length === 0 ? (
              <p className="py-1 text-xs text-muted-foreground">Sin conversaciones</p>
            ) : (
              <div className="flex flex-col gap-0.5">
                {usuarios.map((chat) => (
                  <ChatListItem key={chat.id} chat={chat} isActive={chat.id === activeChatId} />
                ))}
              </div>
            )}
          </div>

          <div>
            <SectionHeader
              label="Agentes"
              action={
                <NewAgentChatDialog
                  existingChats={chats}
                  onCreated={handleAgentChatCreated}
                />
              }
            />
            {agentes.length === 0 ? (
              <p className="py-1 text-xs text-muted-foreground">Sin conversaciones</p>
            ) : (
              <div className="flex flex-col gap-0.5">
                {agentes.map((chat) => (
                  <ChatListItem key={chat.id} chat={chat} isActive={chat.id === activeChatId} />
                ))}
              </div>
            )}
          </div>
        </section>

        <footer className="border-t px-5 py-4">
          <div className="flex items-center justify-between gap-3">
            <div className="min-w-0">
              <p className="truncate text-sm font-semibold">{displayName}</p>
              <p className="truncate text-xs text-muted-foreground">
                {t('privatePanel.userRolePlaceholder')}
              </p>
            </div>
            <Button asChild size="icon-sm" variant="ghost" aria-label={t('privatePanel.settings')}>
              <Link to={AppPaths.APP_PROFILE} title={t('privatePanel.settings')}>
                <Settings />
              </Link>
            </Button>
          </div>
        </footer>
      </div>
    </aside>
  )
}
