import { useState } from 'react'
import { Check, ChevronDown, ChevronUp, MessageSquare, X } from 'lucide-react'
import type { ApprovalData } from '@/core/api/notification'
import { Button } from '@/shared/components/ui/button'
import { useUpdateApprovalStatusMutation, useAddApprovalCommentMutation } from '@/core/query/notification'
import { cn } from '@/shared/lib/utils'

interface ApprovalItemProps {
  approval: ApprovalData
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

export function ApprovalItem({ approval }: ApprovalItemProps) {
  const [expanded, setExpanded] = useState(false)
  const [comment, setComment] = useState('')
  const updateStatusMutation = useUpdateApprovalStatusMutation()
  const addCommentMutation = useAddApprovalCommentMutation()

  const isPending = approval.status === 'pending'

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
              <span className="text-sm font-medium">{approval.target.name}</span>
              <StatusBadge status={approval.status} />
            </div>
            <p className="mt-0.5 text-xs text-muted-foreground">
              Solicitado por <span className="font-medium">{approval.requester.name}</span> en <span className="font-medium">{approval.context.name}</span>
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
                {expanded ? <ChevronUp className="size-3" /> : <ChevronDown className="size-3" />}
              </button>
            )}
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
