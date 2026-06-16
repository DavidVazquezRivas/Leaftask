import { useState } from 'react'
import { UserPlus } from 'lucide-react'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/shared/components/ui/dialog'
import { Button } from '@/shared/components/ui/button'
import type { ChatData } from '@/core/api/chat'
import { useUsersInfiniteQuery } from '@/core/query/user/users/queries/useUsersInfiniteQuery'
import { useSessionMeQuery } from '@/core/query/user/session/queries/useSessionMeQuery'
import { useChatsQuery } from '@/core/query/chat'
import { useCreateChatMutation } from '@/core/query/chat'
import { getInitials } from '@/shared/lib/text'

interface NewChatDialogProps {
  onCreated?: (chatId: string) => void
}

export function NewChatDialog({ onCreated }: NewChatDialogProps) {
  const [open, setOpen] = useState(false)
  const [search, setSearch] = useState('')

  const meQuery = useSessionMeQuery()
  const chatsQuery = useChatsQuery()
  const usersQuery = useUsersInfiniteQuery({ search: search || undefined }, open)
  const createMutation = useCreateChatMutation()

  const currentUserId = meQuery.data?.id
  const existingChats: ChatData[] = chatsQuery.data ?? []
  const existingParticipantIds = new Set(
    existingChats
      .filter((c) => c.type === 'person' && c.otherParticipantId)
      .map((c) => c.otherParticipantId!)
  )

  const users = usersQuery.data?.data ?? []

  const handleSelect = (userId: string) => {
    createMutation.mutate(
      { otherParticipantId: userId, otherParticipantType: 'person' },
      {
        onSuccess: (chat) => {
          setOpen(false)
          setSearch('')
          onCreated?.(chat.id)
        },
      }
    )
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button size="icon-sm" variant="ghost" className="size-6 shrink-0">
          <UserPlus size={13} />
        </Button>
      </DialogTrigger>

      <DialogContent className="max-w-sm">
        <DialogHeader>
          <DialogTitle>Nueva conversación</DialogTitle>
        </DialogHeader>

        <input
          autoFocus
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Buscar persona..."
          className="w-full rounded-md border border-border bg-muted/40 px-3 py-2 text-sm outline-none placeholder:text-muted-foreground"
        />

        <div className="max-h-64 overflow-y-auto">
          {usersQuery.isLoading ? (
            <p className="py-4 text-center text-xs text-muted-foreground">Cargando...</p>
          ) : users.length === 0 ? (
            <p className="py-4 text-center text-xs text-muted-foreground">Sin resultados</p>
          ) : (
            <ul className="flex flex-col gap-0.5">
              {users.map((user) => {
                const isSelf = user.id === currentUserId
                const hasChat = existingParticipantIds.has(user.id)
                const disabled = isSelf || hasChat || createMutation.isPending

                return (
                  <li key={user.id}>
                    <button
                      onClick={() => !disabled && handleSelect(user.id)}
                      disabled={disabled}
                      className="flex w-full items-center gap-3 rounded-md px-2 py-2 text-left text-sm disabled:cursor-not-allowed disabled:opacity-40 enabled:hover:bg-accent"
                    >
                      <div className="flex size-8 shrink-0 items-center justify-center rounded-full bg-muted text-xs font-medium text-muted-foreground">
                        {getInitials(user.fullName)}
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="truncate font-medium">{user.fullName}</p>
                        <p className="truncate text-xs text-muted-foreground">{user.email}</p>
                      </div>
                      {isSelf && (
                        <span className="shrink-0 text-[10px] text-muted-foreground">Tú</span>
                      )}
                      {hasChat && !isSelf && (
                        <span className="shrink-0 text-[10px] text-muted-foreground">Ya existe</span>
                      )}
                    </button>
                  </li>
                )
              })}
            </ul>
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
