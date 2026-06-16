import { useState } from 'react'
import { Bot } from 'lucide-react'
import {
  Dialog,
  DialogContent,
  DialogFooter,
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
import type { AgentData } from '@/core/api/agent'
import { useCreateAgentMutation } from '@/core/query/agent'
import { useMyProjectsQuery } from '@/core/query/project/management/queries/useMyProjectsQuery'
import { useProjectRolesQuery } from '@/core/query/project/roles/queries/useProjectRolesQuery'

interface CreateAgentDialogProps {
  projectId?: string
  onCreated?: (agent: AgentData) => void
  trigger: React.ReactNode
}

export function CreateAgentDialog({ projectId: fixedProjectId, onCreated, trigger }: CreateAgentDialogProps) {
  const [open, setOpen] = useState(false)
  const [selectedProjectId, setSelectedProjectId] = useState<string>(fixedProjectId ?? '')
  const [roleId, setRoleId] = useState('')
  const [name, setName] = useState('')
  const [instructions, setInstructions] = useState('')

  const effectiveProjectId = fixedProjectId ?? selectedProjectId

  const projectsQuery = useMyProjectsQuery()
  const rolesQuery = useProjectRolesQuery(effectiveProjectId || null)
  const createMutation = useCreateAgentMutation()

  const projects = projectsQuery.data?.data ?? []
  const roles = rolesQuery.data?.data ?? []

  const canSubmit =
    Boolean(effectiveProjectId) &&
    Boolean(roleId) &&
    name.trim().length > 0 &&
    instructions.trim().length > 0 &&
    !createMutation.isPending

  const handleOpenChange = (value: boolean) => {
    if (!value) {
      setSelectedProjectId(fixedProjectId ?? '')
      setRoleId('')
      setName('')
      setInstructions('')
    }
    setOpen(value)
  }

  const handleProjectChange = (value: string) => {
    setSelectedProjectId(value)
    setRoleId('')
  }

  const handleSubmit = () => {
    if (!canSubmit) return

    createMutation.mutate(
      { projectId: effectiveProjectId, roleId, name: name.trim(), instructions: instructions.trim() },
      {
        onSuccess: (agent) => {
          setOpen(false)
          onCreated?.(agent)
        },
      }
    )
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>{trigger}</DialogTrigger>

      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Bot size={16} />
            Crear agente
          </DialogTitle>
        </DialogHeader>

        <div className="flex flex-col gap-4">
          {!fixedProjectId && (
            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-medium">Proyecto</label>
              <Select value={selectedProjectId} onValueChange={handleProjectChange}>
                <SelectTrigger>
                  <SelectValue placeholder="Selecciona un proyecto..." />
                </SelectTrigger>
                <SelectContent>
                  {projects.map((p) => (
                    <SelectItem key={p.id} value={p.id}>
                      {p.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          )}

          <div className="flex flex-col gap-1.5">
            <label className="text-sm font-medium">Rol en el proyecto</label>
            <Select
              value={roleId}
              onValueChange={setRoleId}
              disabled={!effectiveProjectId || rolesQuery.isLoading}
            >
              <SelectTrigger>
                <SelectValue
                  placeholder={
                    !effectiveProjectId
                      ? 'Selecciona un proyecto primero'
                      : rolesQuery.isLoading
                        ? 'Cargando roles...'
                        : 'Selecciona un rol...'
                  }
                />
              </SelectTrigger>
              <SelectContent>
                {roles.map((r) => (
                  <SelectItem key={r.id} value={r.id}>
                    {r.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="flex flex-col gap-1.5">
            <label className="text-sm font-medium">Nombre</label>
            <input
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Ej. Asistente de revisión de código"
              className="rounded-md border border-border bg-muted/40 px-3 py-2 text-sm outline-none placeholder:text-muted-foreground focus:border-ring"
            />
          </div>

          <div className="flex flex-col gap-1.5">
            <label className="text-sm font-medium">Instrucciones</label>
            <textarea
              value={instructions}
              onChange={(e) => setInstructions(e.target.value)}
              placeholder="Describe el propósito del agente, sus responsabilidades y cómo debe comportarse..."
              rows={4}
              className="resize-none rounded-md border border-border bg-muted/40 px-3 py-2 text-sm outline-none placeholder:text-muted-foreground focus:border-ring"
            />
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => handleOpenChange(false)}>
            Cancelar
          </Button>
          <Button onClick={handleSubmit} disabled={!canSubmit}>
            {createMutation.isPending ? 'Creando...' : 'Crear agente'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
