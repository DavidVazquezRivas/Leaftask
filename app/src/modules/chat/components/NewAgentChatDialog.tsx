import { useState } from 'react'
import { Bot } from 'lucide-react'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/shared/components/ui/dialog'
import { Button } from '@/shared/components/ui/button'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'
import type { ChatData } from '@/core/api/chat'
import { useMyProjectsQuery } from '@/core/query/project/management/queries/useMyProjectsQuery'
import { useProjectMembersQuery } from '@/core/query/project/members/queries/useProjectMembersQuery'
import { useCreateChatMutation } from '@/core/query/chat'
import { getInitials } from '@/shared/lib/text'

interface NewAgentChatDialogProps {
  existingChats: ChatData[]
  onCreated?: (chatId: string) => void
}

export function NewAgentChatDialog({ existingChats, onCreated }: NewAgentChatDialogProps) {
  const [open, setOpen] = useState(false)
  const [selectedProjectId, setSelectedProjectId] = useState('')

  const projectsQuery = useMyProjectsQuery()
  const membersQuery = useProjectMembersQuery(selectedProjectId || null, { limit: 50 })
  const createMutation = useCreateChatMutation()

  const projects = projectsQuery.data?.data ?? []

  const existingAgentIds = new Set(
    existingChats
      .filter((c) => c.type === 'agent' && c.otherParticipantId)
      .map((c) => c.otherParticipantId!)
  )

  const availableAgents = (membersQuery.data?.data ?? []).filter(
    (m) => m.type === 'agent' && !existingAgentIds.has(m.id)
  )

  const handleSelect = (agentId: string, agentName: string) => {
    createMutation.mutate(
      { otherParticipantId: agentId, otherParticipantType: 'agent' },
      {
        onSuccess: (chat) => {
          setOpen(false)
          setSelectedProjectId('')
          onCreated?.(chat.id)
        },
      }
    )
  }

  const handleOpenChange = (value: boolean) => {
    if (!value) setSelectedProjectId('')
    setOpen(value)
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button size="icon-sm" variant="ghost" className="size-6 shrink-0" title="Iniciar chat con agente">
          <Bot size={13} />
        </Button>
      </DialogTrigger>

      <DialogContent className="max-w-sm">
        <DialogHeader>
          <DialogTitle>Chatear con un agente</DialogTitle>
        </DialogHeader>

        <Select value={selectedProjectId} onValueChange={setSelectedProjectId} disabled={projectsQuery.isLoading}>
          <SelectTrigger>
            <SelectValue placeholder={projectsQuery.isLoading ? 'Cargando proyectos...' : 'Selecciona un proyecto'} />
          </SelectTrigger>
          <SelectContent>
            {projects.map((p) => (
              <SelectItem key={p.id} value={p.id}>
                {p.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>

        {selectedProjectId && (
          <div className="max-h-64 overflow-y-auto">
            {membersQuery.isLoading ? (
              <p className="py-4 text-center text-xs text-muted-foreground">Cargando agentes...</p>
            ) : availableAgents.length === 0 ? (
              <p className="py-4 text-center text-xs text-muted-foreground">
                {(membersQuery.data?.data ?? []).filter((m) => m.type === 'agent').length === 0
                  ? 'Sin agentes en este proyecto'
                  : 'Ya tienes chat con todos los agentes de este proyecto'}
              </p>
            ) : (
              <ul className="flex flex-col gap-0.5">
                {availableAgents.map((agent) => (
                  <li key={agent.id}>
                    <button
                      onClick={() => handleSelect(agent.id, agent.name)}
                      disabled={createMutation.isPending}
                      className="flex w-full items-center gap-3 rounded-md px-2 py-2 text-left text-sm disabled:cursor-not-allowed disabled:opacity-40 enabled:hover:bg-accent"
                    >
                      <div className="flex size-8 shrink-0 items-center justify-center rounded-full bg-muted text-xs font-medium text-muted-foreground">
                        {getInitials(agent.name)}
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="truncate font-medium">{agent.name}</p>
                      </div>
                    </button>
                  </li>
                ))}
              </ul>
            )}
          </div>
        )}
      </DialogContent>
    </Dialog>
  )
}
