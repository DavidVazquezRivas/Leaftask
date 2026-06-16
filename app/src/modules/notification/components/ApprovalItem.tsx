import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Check, ChevronDown, ChevronUp, ExternalLink, MessageSquare, X } from 'lucide-react'
import type { ApprovalData } from '@/core/api/notification'
import { AppPaths } from '@/core/router'
import { Button } from '@/shared/components/ui/button'
import { useUpdateApprovalStatusMutation, useAddApprovalCommentMutation } from '@/core/query/notification'
import { useWorkItemDetailQuery } from '@/core/query/workitems'
import { cn } from '@/shared/lib/utils'

interface ApprovalItemProps {
  approval: ApprovalData
  orgMap: Record<string, string>
  projectMap: Record<string, string>
}

function getRelativeTime(timestamp: string): string {
  const now = new Date()
  const date = new Date(timestamp)
  const diffMs = now.getTime() - date.getTime()
  const diffMin = Math.floor(diffMs / 60_000)
  const diffHours = Math.floor(diffMin / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffMin < 1) return 'ahora'
  if (diffMin < 60) return `hace ${diffMin}m`
  if (diffHours < 24) return `hace ${diffHours}h`
  return `hace ${diffDays}d`
}

const ACTION_LABELS: Record<string, string> = {
  // Work Items
  CreateWorkItemCommand: 'Crear work item',
  UpdateWorkItemCommand: 'Actualizar work item',
  DeleteWorkItemCommand: 'Eliminar work item',
  AddCommentCommand: 'Añadir comentario',
  UpdateCommentCommand: 'Editar comentario',
  DeleteCommentCommand: 'Eliminar comentario',
  UploadAttachmentCommand: 'Subir adjunto',
  DeleteAttachmentCommand: 'Eliminar adjunto',
  LogWorkCommand: 'Registrar trabajo',
  UpdateWorkLogCommand: 'Actualizar registro de trabajo',
  DeleteWorkLogCommand: 'Eliminar registro de trabajo',
  // Projects
  CreateProjectCommand: 'Crear proyecto',
  PatchProjectCommand: 'Editar configuración del proyecto',
  InviteProjectMemberCommand: 'Invitar miembro al proyecto',
  RemoveProjectMemberCommand: 'Eliminar miembro del proyecto',
  UpdateProjectMemberRoleCommand: 'Cambiar rol de miembro del proyecto',
  CreateProjectRoleCommand: 'Crear rol de proyecto',
  PatchProjectRoleCommand: 'Editar rol de proyecto',
  DeleteProjectRoleCommand: 'Eliminar rol de proyecto',
  CreateCustomFieldCommand: 'Crear campo personalizado',
  PatchCustomFieldCommand: 'Editar campo personalizado',
  DeleteCustomFieldCommand: 'Eliminar campo personalizado',
  // Organizations
  PatchOrganizationCommand: 'Editar organización',
  DeleteOrganizationCommand: 'Eliminar organización',
  CreateOrganizationInvitationCommand: 'Invitar a la organización',
  DeleteOrganizationMemberCommand: 'Eliminar miembro de la organización',
  UpdateOrganizationMemberRoleCommand: 'Cambiar rol de miembro',
  CreateOrganizationRoleCommand: 'Crear rol de organización',
  PatchOrganizationRoleCommand: 'Editar rol de organización',
  DeleteOrganizationRoleCommand: 'Eliminar rol de organización',
  // Agents
  CreateAgentCommand: 'Crear agente',
  DeleteAgentCommand: 'Eliminar agente',
}

// Commands that operate on an existing work item and have workItemId in the payload
const WORK_ITEM_COMMANDS = new Set([
  'UpdateWorkItemCommand',
  'DeleteWorkItemCommand',
  'AddCommentCommand',
  'UpdateCommentCommand',
  'DeleteCommentCommand',
  'UploadAttachmentCommand',
  'DeleteAttachmentCommand',
  'LogWorkCommand',
  'UpdateWorkLogCommand',
  'DeleteWorkLogCommand',
])

// Commands where the appropriate destination is project settings
const PROJECT_SETTINGS_COMMANDS = new Set([
  'PatchProjectCommand',
  'CreateProjectRoleCommand',
  'PatchProjectRoleCommand',
  'DeleteProjectRoleCommand',
  'InviteProjectMemberCommand',
  'RemoveProjectMemberCommand',
  'UpdateProjectMemberRoleCommand',
  'CreateCustomFieldCommand',
  'PatchCustomFieldCommand',
  'DeleteCustomFieldCommand',
])

// Commands where the destination is org settings
const ORG_SETTINGS_COMMANDS = new Set([
  'PatchOrganizationCommand',
  'DeleteOrganizationCommand',
  'CreateOrganizationRoleCommand',
  'PatchOrganizationRoleCommand',
  'DeleteOrganizationRoleCommand',
  'CreateOrganizationInvitationCommand',
  'DeleteOrganizationMemberCommand',
  'UpdateOrganizationMemberRoleCommand',
])

function extractClassName(assemblyQualifiedName: string): string {
  return assemblyQualifiedName.split(',')[0].trim().split('.').at(-1) ?? assemblyQualifiedName
}

function resolveActionLabel(actionType: string): string {
  const className = extractClassName(actionType)
  return ACTION_LABELS[className] ?? className.replace(/Command$/, '').replace(/([A-Z])/g, ' $1').trim()
}

interface EntityContext {
  workItemId: string | null
  inlineTitle: string | null
  specificActionLabel: string | null
}

function resolveUpdateWorkItemLabel(payload: Record<string, unknown>): string {
  const changes: string[] = []
  if (payload.Title != null) changes.push('título')
  if (payload.Description != null) changes.push('descripción')
  if (payload.StatusId != null) changes.push('estado')
  if (payload.TypeId != null) changes.push('tipo')
  if (payload.UpdateAssignee === true) {
    changes.push(payload.AssigneeId != null ? 'responsable' : 'responsable (desasignar)')
  }
  if (payload.Progress != null) changes.push('progreso')
  if (payload.Estimation != null) changes.push('estimación')
  if (payload.LimitDate != null) changes.push('fecha límite')
  if (payload.UpdateParent === true) changes.push('elemento padre')
  if (payload.CustomFields != null && Object.keys(payload.CustomFields as object).length > 0) {
    changes.push('campos personalizados')
  }
  if (changes.length === 0) return 'Actualizar work item'
  if (changes.length === 1) {
    const field = changes[0]
    if (field === 'responsable') return 'Asignar responsable'
    if (field === 'responsable (desasignar)') return 'Desasignar responsable'
    if (field === 'estado') return 'Cambiar estado'
    return `Actualizar ${field}`
  }
  return `Actualizar work item (${changes.join(', ')})`
}

function extractEntityContext(actionType: string | null, actionPayload: string | null): EntityContext {
  if (!actionType || !actionPayload) return { workItemId: null, inlineTitle: null, specificActionLabel: null }
  const className = extractClassName(actionType)
  try {
    const payload = JSON.parse(actionPayload) as Record<string, unknown>
    if (WORK_ITEM_COMMANDS.has(className)) {
      const specificActionLabel = className === 'UpdateWorkItemCommand'
        ? resolveUpdateWorkItemLabel(payload)
        : null
      return { workItemId: (payload.WorkItemId as string | undefined) ?? null, inlineTitle: null, specificActionLabel }
    }
    if (className === 'CreateWorkItemCommand') {
      return { workItemId: null, inlineTitle: (payload.Title as string | undefined) ?? null, specificActionLabel: null }
    }
  } catch {
    // malformed payload — ignore
  }
  return { workItemId: null, inlineTitle: null, specificActionLabel: null }
}

function StatusBadge({ status }: { status: ApprovalData['status'] }) {
  const base = 'inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium'
  switch (status) {
    case 'pending':
      return <span className={cn(base, 'bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400')}>Pendiente</span>
    case 'approved':
      return <span className={cn(base, 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400')}>Aprobado</span>
    case 'rejected':
      return <span className={cn(base, 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400')}>Rechazado</span>
    default:
      return null
  }
}

export function ApprovalItem({ approval, orgMap, projectMap }: ApprovalItemProps) {
  const navigate = useNavigate()
  const [expanded, setExpanded] = useState(false)
  const [comment, setComment] = useState('')
  const updateStatusMutation = useUpdateApprovalStatusMutation()
  const addCommentMutation = useAddApprovalCommentMutation()

  const isPending = approval.status === 'pending'

  const contextName =
    (approval.contextType === 'organization' ? orgMap[approval.context.id] : projectMap[approval.context.id]) ??
    approval.context.name

  const { workItemId, inlineTitle, specificActionLabel } = extractEntityContext(approval.actionType, approval.actionPayload)
  const workItemQuery = useWorkItemDetailQuery(
    workItemId ? approval.context.id : null,
    workItemId ?? null
  )
  const entityName = workItemQuery.data?.data?.title ?? inlineTitle

  const navigationPath = (() => {
    const className = approval.actionType ? extractClassName(approval.actionType) : null
    if (approval.contextType === 'organization') {
      return className && ORG_SETTINGS_COMMANDS.has(className)
        ? AppPaths.organizationSettings(approval.context.id)
        : AppPaths.organization(approval.context.id)
    }
    if (className && WORK_ITEM_COMMANDS.has(className) && workItemId) {
      return AppPaths.projectWorkItem(approval.context.id, workItemId)
    }
    if (className && PROJECT_SETTINGS_COMMANDS.has(className)) {
      return AppPaths.projectSettings(approval.context.id)
    }
    return AppPaths.project(approval.context.id)
  })()

  const baseActionLabel = specificActionLabel
    ?? (approval.actionType ? resolveActionLabel(approval.actionType) : approval.permissionName)
  const actionLabel = entityName ? `${baseActionLabel}: "${entityName}"` : baseActionLabel

  const handleApprove = () => {
    updateStatusMutation.mutate({ approvalId: approval.id, status: 'approved' })
  }

  const handleReject = () => {
    updateStatusMutation.mutate({ approvalId: approval.id, status: 'rejected' })
  }

  const handleAddComment = () => {
    if (!comment.trim()) return
    addCommentMutation.mutate(
      { approvalId: approval.id, content: comment.trim() },
      { onSuccess: () => setComment('') }
    )
  }

  return (
    <div className="rounded-lg border bg-card">
      <div className="p-4">
        <div className="flex items-start justify-between gap-3">
          <div className="min-w-0 flex-1">
            <div className="flex flex-wrap items-center gap-2">
              <span className="text-sm font-medium">{actionLabel}</span>
              <StatusBadge status={approval.status} />
            </div>
            <p className="mt-0.5 text-xs text-muted-foreground">
              Solicitado por <span className="font-medium">{approval.requester.name}</span> en <span className="font-medium">{contextName}</span>
            </p>
            <p className="mt-0.5 text-xs text-muted-foreground">{getRelativeTime(approval.createdAt)}</p>
          </div>

          <div className="flex shrink-0 items-center gap-1.5">
            {approval.comments.length > 0 && (
              <button
                type="button"
                className="flex items-center gap-1 rounded px-1.5 py-1 text-xs text-muted-foreground hover:bg-muted"
                onClick={() => setExpanded((v) => !v)}
              >
                <MessageSquare className="size-3" />
                {approval.comments.length}
              </button>
            )}
            <button
              type="button"
              title={`Ir a ${contextName}`}
              className="rounded p-1 text-muted-foreground hover:bg-muted"
              onClick={() => navigate(navigationPath)}
            >
              <ExternalLink className="size-4" />
            </button>
            <button
              type="button"
              className="rounded p-1 text-muted-foreground hover:bg-muted"
              onClick={() => setExpanded((v) => !v)}
            >
              {expanded ? <ChevronUp className="size-4" /> : <ChevronDown className="size-4" />}
            </button>
          </div>
        </div>

        {isPending && (
          <div className="mt-3 flex gap-2">
            <Button
              size="sm"
              variant="default"
              className="h-7 text-xs"
              onClick={handleApprove}
              disabled={updateStatusMutation.isPending}
            >
              <Check className="mr-1 size-3" />
              Aprobar
            </Button>
            <Button
              size="sm"
              variant="outline"
              className="h-7 text-xs"
              onClick={handleReject}
              disabled={updateStatusMutation.isPending}
            >
              <X className="mr-1 size-3" />
              Rechazar
            </Button>
          </div>
        )}
      </div>

      {expanded && (
        <div className="border-t px-4 py-3">
          {approval.comments.length > 0 && (
            <div className="mb-3 space-y-2">
              {approval.comments.map((c) => (
                <div key={c.id} className="flex gap-2">
                  <div className="grid size-5 shrink-0 place-items-center rounded-full bg-muted text-[10px] font-semibold uppercase">
                    {c.author.name.slice(0, 1)}
                  </div>
                  <div className="min-w-0 flex-1">
                    <div className="flex items-baseline gap-1.5">
                      <span className="text-xs font-medium">{c.author.name}</span>
                      <span className="text-[10px] text-muted-foreground">{getRelativeTime(c.timestamp)}</span>
                    </div>
                    <p className="text-xs text-muted-foreground">{c.content}</p>
                  </div>
                </div>
              ))}
            </div>
          )}

          <div className="flex gap-2">
            <input
              type="text"
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              placeholder="Añadir comentario..."
              className="flex-1 rounded-md border border-border bg-muted/40 px-3 py-1.5 text-xs outline-none placeholder:text-muted-foreground focus:border-primary"
              onKeyDown={(e) => {
                if (e.key === 'Enter' && !e.shiftKey) {
                  e.preventDefault()
                  handleAddComment()
                }
              }}
            />
            <Button
              size="sm"
              variant="ghost"
              className="h-7 text-xs"
              onClick={handleAddComment}
              disabled={!comment.trim() || addCommentMutation.isPending}
            >
              Enviar
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
