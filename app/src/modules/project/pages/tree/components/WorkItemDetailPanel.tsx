import { useState, useEffect, useMemo } from 'react'
import {
  X,
  Pencil,
  Clock,
  Calendar as CalendarIcon,
  GitBranch,
  Activity,
  MessageSquare,
  Loader2,
  User,
  Trash2,
} from 'lucide-react'
import { useTranslation } from 'react-i18next'

import type { WorkItemTypeData, WorkItemStatusData, WorkItemData } from '@/core/api/workitems'
import type { UpdateWorkItemRequest } from '@/core/api/workitems'
import type { ProjectMemberData } from '@/core/api/project/members'
import { useWorkItemDetailQuery } from '@/core/query/workitems'
import { useUpdateWorkItemMutation, useDeleteWorkItemMutation } from '@/core/query/workitems'
import { Button } from '@/shared/components/ui/button'
import { Calendar } from '@/shared/components/ui/calendar'
import { Input } from '@/shared/components/ui/input'
import { Popover, PopoverContent, PopoverTrigger } from '@/shared/components/ui/popover'
import { Textarea } from '@/shared/components/ui/textarea'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'
import { cn } from '@/shared/lib/utils'

type EditingField =
  | 'title'
  | 'description'
  | 'status'
  | 'type'
  | 'assignee'
  | 'limitDate'
  | 'estimation'
  | 'progress'
  | 'parent'
  | null

interface WorkItemDetailPanelProps {
  open: boolean
  projectId: string
  itemId: string | null
  onClose: () => void
  onDelete?: () => void
  types: WorkItemTypeData[]
  statuses: WorkItemStatusData[]
  workItems: WorkItemData[]
  members: ProjectMemberData[]
}

const STATUS_COLORS: Record<string, string> = {
  'Por hacer': 'bg-zinc-500/15 text-zinc-400 border border-zinc-500/30',
  'En progreso': 'bg-blue-500/15 text-blue-400 border border-blue-500/30',
  'Hecho': 'bg-green-500/15 text-green-400 border border-green-500/30',
  'Bloqueado': 'bg-red-500/15 text-red-400 border border-red-500/30',
}
const DEFAULT_STATUS_COLOR = 'bg-muted text-muted-foreground border border-border'

const TYPE_COLORS: Record<string, string> = {
  'Task': 'bg-indigo-500/15 text-indigo-400 border border-indigo-500/30',
  'Bug': 'bg-red-500/15 text-red-400 border border-red-500/30',
}
const DEFAULT_TYPE_COLOR = 'bg-muted text-muted-foreground border border-border'

function getInitials(fullName: string): string {
  return fullName
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map((w) => w[0].toUpperCase())
    .join('')
}

function formatDate(isoString: string): string {
  return new Date(isoString).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

function formatRelative(isoString: string, locale: string): string {
  const diffMs = Date.now() - new Date(isoString).getTime()
  const minutes = Math.floor(diffMs / 60_000)
  const rtf = new Intl.RelativeTimeFormat(locale, { numeric: 'auto' })
  if (minutes < 1) return rtf.format(0, 'seconds')
  if (minutes < 60) return rtf.format(-minutes, 'minutes')
  const hours = Math.floor(minutes / 60)
  if (hours < 24) return rtf.format(-hours, 'hours')
  const days = Math.floor(hours / 24)
  if (days < 7) return rtf.format(-days, 'days')
  return new Date(isoString).toLocaleDateString(locale)
}

export function WorkItemDetailPanel({
  open,
  projectId,
  itemId,
  onClose,
  onDelete,
  types,
  statuses,
  workItems,
  members,
}: WorkItemDetailPanelProps) {
  const { t, i18n } = useTranslation('workitems')
  const [activeTab, setActiveTab] = useState<'comments' | 'activity'>('comments')
  const [editingField, setEditingField] = useState<EditingField>(null)
  const [draftTitle, setDraftTitle] = useState('')
  const [draftDescription, setDraftDescription] = useState('')
  const [draftStatusId, setDraftStatusId] = useState('')
  const [draftTypeId, setDraftTypeId] = useState('')
  const [draftAssigneeId, setDraftAssigneeId] = useState<string | null>(null)
  const [draftLimitDate, setDraftLimitDate] = useState('')
  const [draftEstimation, setDraftEstimation] = useState('')
  const [draftProgress, setDraftProgress] = useState(0)
  const [draftParentId, setDraftParentId] = useState<string | null>(null)
  const [confirmingDelete, setConfirmingDelete] = useState(false)

  const detailQuery = useWorkItemDetailQuery(projectId, itemId)
  const updateMutation = useUpdateWorkItemMutation(projectId, itemId ?? '')
  const deleteMutation = useDeleteWorkItemMutation(projectId)

  const detail = detailQuery.data?.data

  useEffect(() => {
    setEditingField(null)
    setActiveTab('comments')
    setConfirmingDelete(false)
  }, [itemId])

  const startEdit = (field: NonNullable<EditingField>) => {
    if (!detail) return
    switch (field) {
      case 'title': setDraftTitle(detail.title); break
      case 'description': setDraftDescription(detail.description ?? ''); break
      case 'status': setDraftStatusId(detail.statusId); break
      case 'type': setDraftTypeId(detail.typeId); break
      case 'assignee': setDraftAssigneeId(detail.assignee?.id ?? null); break
      case 'limitDate': setDraftLimitDate(detail.limitDate?.slice(0, 10) ?? ''); break
      case 'estimation': setDraftEstimation(String(detail.estimation ?? 0)); break
      case 'progress': setDraftProgress(Math.round((detail.progress ?? 0) * 100)); break
      case 'parent': setDraftParentId(detail.parentId ?? null); break
    }
    setEditingField(field)
  }

  const cancelEdit = () => setEditingField(null)

  const forbiddenParentIds = useMemo<Set<string>>(() => {
    if (!itemId) return new Set()
    const result = new Set<string>()
    const queue = [itemId]
    while (queue.length > 0) {
      const id = queue.shift()!
      for (const w of workItems) {
        if (w.parentId === id) {
          result.add(w.id)
          queue.push(w.id)
        }
      }
    }
    return result
  }, [itemId, workItems])

  const save = (payload: UpdateWorkItemRequest) => {
    updateMutation.mutate(payload, { onSuccess: () => setEditingField(null) })
  }

  const isPending = updateMutation.isPending || deleteMutation.isPending

  const handleDelete = () => {
    if (!itemId) return
    const id = itemId
    deleteMutation.mutate(id, {
      onSuccess: () => {
        onDelete?.()
        onClose()
      },
    })
  }

  const statusName = statuses.find((s) => s.id === detail?.statusId)?.name ?? ''
  const typeName = types.find((tp) => tp.id === detail?.typeId)?.name ?? ''
  const parentItem = workItems.find((w) => w.id === detail?.parentId)

  const fieldDisplayNames: Record<string, string> = {
    assigneeId: t('detail.assignee'),
    statusId: t('detail.status'),
    typeId: t('detail.type'),
    parentId: t('detail.parent'),
    progress: t('detail.progress'),
    estimation: t('detail.estimation'),
    limitDate: t('detail.limitDate'),
    title: t('create.fields.titleLabel'),
    description: t('detail.description'),
  }

  const resolveActivityValue = (fieldName: string, value: string): string => {
    if (!value) {
      if (fieldName === 'assigneeId') return t('detail.unassigned')
      if (fieldName === 'parentId') return t('detail.noParent')
      return '—'
    }
    switch (fieldName) {
      case 'statusId':
        return statuses.find((s) => s.id === value)?.name ?? value
      case 'typeId':
        return types.find((tp) => tp.id === value)?.name ?? value
      case 'assigneeId':
        return members.find((m) => m.id === value)?.name ?? value
      case 'parentId': {
        const w = workItems.find((wi) => wi.id === value)
        return w ? `${w.code} – ${w.title}` : value
      }
      case 'progress':
        return `${value}%`
      case 'estimation':
        return `${parseFloat(value)}h`
      default:
        return value
    }
  }
  const progressPercent = Math.round((detail?.progress ?? 0) * 100)
  const estimationH = detail?.estimation ?? 0
  const dedicationTotal = detail?.dedication.total ?? 0
  const dedicationRegisters = detail?.dedication.registers ?? 0
  const dedicationPercent =
    estimationH > 0 && dedicationTotal > 0
      ? Math.round(((dedicationTotal / estimationH) - 1) * 100)
      : null

  return (
    <div
      className={cn(
        'fixed inset-0 z-50',
        open ? 'pointer-events-auto' : 'pointer-events-none'
      )}
    >
      {/* Backdrop */}
      <div
        className={cn(
          'absolute inset-0 transition-opacity duration-300',
          open ? 'opacity-100' : 'opacity-0'
        )}
        onClick={onClose}
      />

      {/* Panel */}
      <div
        className={cn(
          'absolute right-0 top-0 h-full w-125 bg-card border-l border-border flex flex-col shadow-2xl transition-transform duration-300',
          open ? 'translate-x-0' : 'translate-x-full'
        )}
      >
        {detailQuery.isLoading ? (
          <div className="flex h-full items-center justify-center">
            <Loader2 className="size-5 animate-spin text-muted-foreground" />
          </div>
        ) : !detail ? null : (
          <>
            {/* Header */}
            <div className="flex-none px-6 pt-5 pb-4 border-b border-border">
              <p className="text-xs text-muted-foreground mb-1">{detail.code}</p>

              {/* Title */}
              {editingField === 'title' ? (
                <div className="space-y-2">
                  <Input
                    value={draftTitle}
                    onChange={(e) => setDraftTitle(e.target.value)}
                    className="text-base font-semibold"
                    autoFocus
                    disabled={isPending}
                  />
                  <div className="flex items-center gap-1.5">
                    <Button
                      size="sm"
                      className="h-6 text-xs px-2"
                      onClick={() => save({ title: draftTitle })}
                      disabled={isPending || !draftTitle.trim()}
                    >
                      {t('detail.save')}
                    </Button>
                    <Button
                      size="sm"
                      variant="ghost"
                      className="h-6 text-xs px-2"
                      onClick={cancelEdit}
                      disabled={isPending}
                    >
                      {t('detail.cancel')}
                    </Button>
                    <button
                      type="button"
                      onClick={onClose}
                      className="ml-auto rounded-md p-1 text-muted-foreground hover:text-foreground hover:bg-accent transition-colors"
                    >
                      <X size={16} />
                    </button>
                  </div>
                </div>
              ) : (
                <div className="flex items-start justify-between gap-3">
                  <div className="flex items-start gap-2 group flex-1 min-w-0">
                    <h2 className="text-lg font-semibold leading-tight text-foreground">
                      {detail.title}
                    </h2>
                    <button
                      type="button"
                      onClick={() => startEdit('title')}
                      className="shrink-0 opacity-0 group-hover:opacity-100 transition-opacity mt-1 text-muted-foreground hover:text-foreground"
                    >
                      <Pencil size={12} />
                    </button>
                  </div>
                  <button
                    type="button"
                    onClick={onClose}
                    className="shrink-0 rounded-md p-1 text-muted-foreground hover:text-foreground hover:bg-accent transition-colors"
                  >
                    <X size={16} />
                  </button>
                </div>
              )}

            </div>

            {/* Scrollable body */}
            <div className="flex-1 overflow-y-auto px-6 py-5 space-y-4">

              {/* Status + Type */}
              <div className="grid grid-cols-2 gap-3">
                {/* Status */}
                <div className="rounded-lg border border-border bg-muted/20 px-3 py-2.5 space-y-1.5">
                  <div className="flex items-center justify-between">
                    <p className="text-xs text-muted-foreground">{t('detail.status')}</p>
                    {editingField !== 'status' && (
                      <button
                        type="button"
                        onClick={() => startEdit('status')}
                        className="text-muted-foreground hover:text-foreground transition-colors"
                      >
                        <Pencil size={11} />
                      </button>
                    )}
                  </div>
                  {editingField === 'status' ? (
                    <div className="space-y-1.5">
                      <Select value={draftStatusId} onValueChange={setDraftStatusId} disabled={isPending}>
                        <SelectTrigger className="h-7 text-xs">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          {statuses.map((s) => (
                            <SelectItem key={s.id} value={s.id}>{s.name}</SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <div className="flex gap-1.5">
                        <Button size="sm" className="h-6 text-xs px-2" onClick={() => save({ statusId: draftStatusId })} disabled={isPending}>
                          {t('detail.save')}
                        </Button>
                        <Button size="sm" variant="ghost" className="h-6 text-xs px-2" onClick={cancelEdit} disabled={isPending}>
                          {t('detail.cancel')}
                        </Button>
                      </div>
                    </div>
                  ) : (
                    <p className={cn('text-sm font-medium', STATUS_COLORS[statusName] ? '' : 'text-foreground')}>
                      <span className={cn('text-xs font-medium px-2 py-0.5 rounded-full', STATUS_COLORS[statusName] ?? DEFAULT_STATUS_COLOR)}>
                        {statusName || '—'}
                      </span>
                    </p>
                  )}
                </div>

                {/* Type */}
                <div className="rounded-lg border border-border bg-muted/20 px-3 py-2.5 space-y-1.5">
                  <div className="flex items-center justify-between">
                    <p className="text-xs text-muted-foreground">{t('detail.type')}</p>
                    {editingField !== 'type' && (
                      <button
                        type="button"
                        onClick={() => startEdit('type')}
                        className="text-muted-foreground hover:text-foreground transition-colors"
                      >
                        <Pencil size={11} />
                      </button>
                    )}
                  </div>
                  {editingField === 'type' ? (
                    <div className="space-y-1.5">
                      <Select value={draftTypeId} onValueChange={setDraftTypeId} disabled={isPending}>
                        <SelectTrigger className="h-7 text-xs">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          {types.map((tp) => (
                            <SelectItem key={tp.id} value={tp.id}>{tp.name}</SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <div className="flex gap-1.5">
                        <Button size="sm" className="h-6 text-xs px-2" onClick={() => save({ typeId: draftTypeId })} disabled={isPending}>
                          {t('detail.save')}
                        </Button>
                        <Button size="sm" variant="ghost" className="h-6 text-xs px-2" onClick={cancelEdit} disabled={isPending}>
                          {t('detail.cancel')}
                        </Button>
                      </div>
                    </div>
                  ) : (
                    <p>
                      <span className={cn('text-xs font-medium px-2 py-0.5 rounded-full', TYPE_COLORS[typeName] ?? DEFAULT_TYPE_COLOR)}>
                        {typeName || '—'}
                      </span>
                    </p>
                  )}
                </div>
              </div>

              {/* Description */}
              <section className="space-y-1.5">
                <div className="flex items-center justify-between">
                  <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                    {t('detail.description')}
                  </p>
                  {editingField !== 'description' && (
                    <button
                      type="button"
                      onClick={() => startEdit('description')}
                      className="text-muted-foreground hover:text-foreground transition-colors"
                    >
                      <Pencil size={11} />
                    </button>
                  )}
                </div>
                {editingField === 'description' ? (
                  <div className="space-y-2">
                    <Textarea
                      value={draftDescription}
                      onChange={(e) => setDraftDescription(e.target.value)}
                      rows={4}
                      className="resize-none text-sm"
                      autoFocus
                      disabled={isPending}
                    />
                    <div className="flex gap-1.5">
                      <Button
                        size="sm"
                        className="h-7 text-xs px-3"
                        onClick={() => save({ description: draftDescription })}
                        disabled={isPending}
                      >
                        {t('detail.save')}
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="h-7 text-xs px-3"
                        onClick={cancelEdit}
                        disabled={isPending}
                      >
                        {t('detail.cancel')}
                      </Button>
                    </div>
                  </div>
                ) : (
                  <div className="rounded-lg border border-border bg-muted/30 px-3 py-2.5 text-sm text-foreground min-h-14">
                    {detail.description ? (
                      detail.description
                    ) : (
                      <span className="text-muted-foreground italic">
                        {t('detail.noDescription')}
                      </span>
                    )}
                  </div>
                )}
              </section>

              {/* Assignee + Limit Date */}
              <div className="grid grid-cols-2 gap-3">
                {/* Assignee */}
                <div className="rounded-lg border border-border bg-muted/20 px-3 py-2.5 space-y-1.5">
                  <div className="flex items-center justify-between">
                    <p className="text-xs text-muted-foreground flex items-center gap-1.5">
                      <User size={11} />
                      {t('detail.assignee')}
                    </p>
                    {editingField !== 'assignee' && (
                      <button
                        type="button"
                        onClick={() => startEdit('assignee')}
                        className="text-muted-foreground hover:text-foreground transition-colors"
                      >
                        <Pencil size={11} />
                      </button>
                    )}
                  </div>
                  {editingField === 'assignee' ? (
                    <div className="space-y-1.5">
                      <Select
                        value={draftAssigneeId ?? 'none'}
                        onValueChange={(v) => setDraftAssigneeId(v === 'none' ? null : v)}
                        disabled={isPending}
                      >
                        <SelectTrigger className="h-7 text-xs">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="none">{t('detail.unassigned')}</SelectItem>
                          {members.map((m) => (
                            <SelectItem key={m.id} value={m.id}>{m.name}</SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <div className="flex gap-1.5">
                        <Button
                          size="sm"
                          className="h-6 text-xs px-2"
                          onClick={() => save({ assigneeId: draftAssigneeId, updateAssignee: true })}
                          disabled={isPending}
                        >
                          {t('detail.save')}
                        </Button>
                        <Button
                          size="sm"
                          variant="ghost"
                          className="h-6 text-xs px-2"
                          onClick={cancelEdit}
                          disabled={isPending}
                        >
                          {t('detail.cancel')}
                        </Button>
                      </div>
                    </div>
                  ) : detail.assignee ? (
                    <div className="flex items-center gap-2">
                      <span className="inline-flex size-6 items-center justify-center rounded-full bg-indigo-500/20 text-indigo-400 text-[10px] font-semibold shrink-0">
                        {getInitials(detail.assignee.fullName)}
                      </span>
                      <span className="text-sm font-medium text-foreground truncate">
                        {detail.assignee.fullName}
                      </span>
                    </div>
                  ) : (
                    <p className="text-sm text-muted-foreground">{t('detail.unassigned')}</p>
                  )}
                </div>

                {/* Limit Date */}
                <div className="rounded-lg border border-border bg-muted/20 px-3 py-2.5 space-y-1.5">
                  <div className="flex items-center justify-between">
                    <p className="text-xs text-muted-foreground flex items-center gap-1.5">
                      <CalendarIcon size={11} />
                      {t('detail.limitDate')}
                    </p>
                    {editingField !== 'limitDate' && (
                      <button
                        type="button"
                        onClick={() => startEdit('limitDate')}
                        className="text-muted-foreground hover:text-foreground transition-colors"
                      >
                        <Pencil size={11} />
                      </button>
                    )}
                  </div>
                  {editingField === 'limitDate' ? (
                    <div className="space-y-1.5">
                      <Popover>
                        <PopoverTrigger asChild>
                          <Button
                            variant="outline"
                            className="h-7 w-full justify-start text-xs font-normal"
                            disabled={isPending}
                          >
                            <CalendarIcon size={11} className="mr-1.5 shrink-0 opacity-60" />
                            {draftLimitDate
                              ? formatDate(draftLimitDate + 'T12:00:00')
                              : <span className="text-muted-foreground">Select date…</span>
                            }
                          </Button>
                        </PopoverTrigger>
                        <PopoverContent className="w-auto p-0" align="start">
                          <Calendar
                            mode="single"
                            selected={draftLimitDate ? new Date(draftLimitDate + 'T12:00:00') : undefined}
                            onSelect={(d) => {
                              if (!d) return
                              const y = d.getFullYear()
                              const m = String(d.getMonth() + 1).padStart(2, '0')
                              const day = String(d.getDate()).padStart(2, '0')
                              setDraftLimitDate(`${y}-${m}-${day}`)
                            }}
                          />
                        </PopoverContent>
                      </Popover>
                      <div className="flex gap-1.5">
                        <Button
                          size="sm"
                          className="h-6 text-xs px-2"
                          onClick={() =>
                            save({
                              limitDate: draftLimitDate
                                ? `${draftLimitDate}T00:00:00Z`
                                : undefined,
                            })
                          }
                          disabled={isPending}
                        >
                          {t('detail.save')}
                        </Button>
                        <Button
                          size="sm"
                          variant="ghost"
                          className="h-6 text-xs px-2"
                          onClick={cancelEdit}
                          disabled={isPending}
                        >
                          {t('detail.cancel')}
                        </Button>
                      </div>
                    </div>
                  ) : (
                    <p className="text-sm font-medium text-foreground">
                      {detail.limitDate ? (
                        formatDate(detail.limitDate)
                      ) : (
                        <span className="text-muted-foreground font-normal">
                          {t('detail.noLimitDate')}
                        </span>
                      )}
                    </p>
                  )}
                </div>
              </div>

              {/* Parent */}
              <div className="rounded-lg border border-border bg-muted/20 px-3 py-2.5 space-y-1.5">
                <div className="flex items-center justify-between">
                  <p className="text-xs text-muted-foreground flex items-center gap-1.5">
                    <GitBranch size={11} />
                    {t('detail.parent')}
                  </p>
                  {editingField !== 'parent' && (
                    <button
                      type="button"
                      onClick={() => startEdit('parent')}
                      className="text-muted-foreground hover:text-foreground transition-colors"
                    >
                      <Pencil size={11} />
                    </button>
                  )}
                </div>
                {editingField === 'parent' ? (
                  <div className="space-y-1.5">
                    <Select
                      value={draftParentId ?? 'none'}
                      onValueChange={(v) => setDraftParentId(v === 'none' ? null : v)}
                      disabled={isPending}
                    >
                      <SelectTrigger className="h-7 text-xs">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="none">{t('detail.noParent')}</SelectItem>
                        {workItems
                          .filter((w) => w.id !== itemId && !forbiddenParentIds.has(w.id))
                          .map((w) => (
                            <SelectItem key={w.id} value={w.id}>
                              {w.code} — {w.title}
                            </SelectItem>
                          ))}
                      </SelectContent>
                    </Select>
                    <div className="flex gap-1.5">
                      <Button
                        size="sm"
                        className="h-6 text-xs px-2"
                        onClick={() => save({ parentId: draftParentId, updateParent: true })}
                        disabled={isPending}
                      >
                        {t('detail.save')}
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="h-6 text-xs px-2"
                        onClick={cancelEdit}
                        disabled={isPending}
                      >
                        {t('detail.cancel')}
                      </Button>
                    </div>
                  </div>
                ) : parentItem ? (
                  <p className="text-sm font-medium text-indigo-400">
                    {parentItem.code} — {parentItem.title}
                  </p>
                ) : (
                  <p className="text-sm text-muted-foreground">{t('detail.noParent')}</p>
                )}
              </div>

              {/* Metrics: Estimation | Dedication | Progress */}
              <div className="grid grid-cols-3 gap-3">
                {/* Estimation */}
                <div className="rounded-lg border border-border bg-muted/20 px-3 py-2.5 space-y-1">
                  <div className="flex items-center justify-between">
                    <p className="text-xs text-muted-foreground flex items-center gap-1.5">
                      <Clock size={11} />
                      {t('detail.estimation')}
                    </p>
                    {editingField !== 'estimation' && (
                      <button
                        type="button"
                        onClick={() => startEdit('estimation')}
                        className="text-muted-foreground hover:text-foreground transition-colors"
                      >
                        <Pencil size={11} />
                      </button>
                    )}
                  </div>
                  {editingField === 'estimation' ? (
                    <div className="space-y-1.5 pt-0.5">
                      <Input
                        type="number"
                        min={0.5}
                        step={0.5}
                        value={draftEstimation}
                        onChange={(e) => setDraftEstimation(e.target.value)}
                        className="h-7 text-xs"
                        autoFocus
                        disabled={isPending}
                      />
                      <div className="flex gap-1">
                        <Button
                          size="sm"
                          className="h-6 text-xs px-2"
                          onClick={() => save({ estimation: parseFloat(draftEstimation) })}
                          disabled={isPending || !draftEstimation || parseFloat(draftEstimation) <= 0}
                        >
                          {t('detail.save')}
                        </Button>
                        <Button
                          size="sm"
                          variant="ghost"
                          className="h-6 text-xs px-2"
                          onClick={cancelEdit}
                          disabled={isPending}
                        >
                          {t('detail.cancel')}
                        </Button>
                      </div>
                    </div>
                  ) : (
                    <p className="text-sm font-semibold text-foreground">
                      {estimationH > 0 ? t('detail.hours', { count: estimationH }) : '—'}
                    </p>
                  )}
                </div>

                {/* Dedication */}
                <div className="rounded-lg border border-border bg-muted/20 px-3 py-2.5 space-y-1">
                  <p className="text-xs text-muted-foreground flex items-center gap-1.5">
                    <Activity size={11} />
                    {t('detail.dedication')}
                  </p>
                  <p className="text-sm font-semibold text-foreground">
                    {dedicationTotal > 0
                      ? t('detail.hours', { count: dedicationTotal })
                      : '—'}
                  </p>
                  {dedicationPercent !== null && (
                    <>
                      <p
                        className={cn(
                          'text-xs font-medium',
                          dedicationPercent > 0 ? 'text-red-400' : 'text-green-400'
                        )}
                      >
                        {dedicationPercent > 0
                          ? `+${dedicationPercent}%`
                          : `${dedicationPercent}%`}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        {t('detail.registers', { count: dedicationRegisters })}
                      </p>
                    </>
                  )}
                </div>

                {/* Progress */}
                <div className="rounded-lg border border-border bg-muted/20 px-3 py-2.5 space-y-1">
                  <div className="flex items-center justify-between">
                    <p className="text-xs text-muted-foreground">{t('detail.progress')}</p>
                    {editingField !== 'progress' && (
                      <button
                        type="button"
                        onClick={() => startEdit('progress')}
                        className="text-muted-foreground hover:text-foreground transition-colors"
                        aria-label={t('detail.editProgress')}
                      >
                        <Pencil size={11} />
                      </button>
                    )}
                  </div>
                  {editingField === 'progress' ? (
                    <div className="space-y-2 pt-0.5">
                      <div className="flex items-center gap-2">
                        <input
                          type="range"
                          min={0}
                          max={100}
                          value={draftProgress}
                          onChange={(e) => setDraftProgress(Number(e.target.value))}
                          className="w-full accent-indigo-500"
                          disabled={isPending}
                        />
                        <span className="text-xs font-semibold text-foreground w-8 text-right shrink-0">
                          {draftProgress}%
                        </span>
                      </div>
                      <div className="flex gap-1">
                        <Button
                          size="sm"
                          className="h-6 text-xs px-2"
                          onClick={() => save({ progress: draftProgress })}
                          disabled={isPending}
                        >
                          {t('detail.save')}
                        </Button>
                        <Button
                          size="sm"
                          variant="ghost"
                          className="h-6 text-xs px-2"
                          onClick={cancelEdit}
                          disabled={isPending}
                        >
                          {t('detail.cancel')}
                        </Button>
                      </div>
                    </div>
                  ) : (
                    <p className="text-sm font-semibold text-green-400">
                      {progressPercent}%
                    </p>
                  )}
                </div>
              </div>

              {/* Work progress bar */}
              <section className="space-y-2">
                <div className="flex items-center justify-between text-xs">
                  <span className="text-muted-foreground font-medium">
                    {t('detail.workProgress')}
                  </span>
                  <span className="font-semibold text-green-400">{progressPercent}%</span>
                </div>
                <div className="h-2 w-full rounded-full bg-muted overflow-hidden">
                  <div
                    className="h-full rounded-full bg-green-500 transition-all duration-500"
                    style={{ width: `${progressPercent}%` }}
                  />
                </div>
              </section>

              {/* Custom fields */}
              {detail.customFields.length > 0 && (
                <section className="space-y-2">
                  <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                    {t('detail.customFields')}
                  </p>
                  <div className="space-y-1.5">
                    {detail.customFields.map((cf) => (
                      <div
                        key={cf.fieldId}
                        className="flex items-center justify-between rounded-lg border border-border bg-muted/20 px-3 py-2"
                      >
                        <span className="text-xs text-muted-foreground">{cf.name}</span>
                        <span className="text-xs font-medium text-foreground">{cf.value}</span>
                      </div>
                    ))}
                  </div>
                </section>
              )}
            </div>

            {/* Delete */}
            <div className="flex-none px-6 py-3 border-t border-border">
              {confirmingDelete ? (
                <div className="flex items-center gap-2">
                  <span className="text-xs text-muted-foreground flex-1">
                    {t('detail.deleteConfirm')}
                  </span>
                  <Button
                    size="sm"
                    variant="destructive"
                    className="h-7 text-xs px-3"
                    onClick={handleDelete}
                    disabled={isPending}
                  >
                    {deleteMutation.isPending ? (
                      <Loader2 size={12} className="animate-spin" />
                    ) : (
                      t('detail.deleteConfirmYes')
                    )}
                  </Button>
                  <Button
                    size="sm"
                    variant="ghost"
                    className="h-7 text-xs px-3"
                    onClick={() => setConfirmingDelete(false)}
                    disabled={isPending}
                  >
                    {t('detail.cancel')}
                  </Button>
                </div>
              ) : (
                <button
                  type="button"
                  onClick={() => setConfirmingDelete(true)}
                  className="flex items-center gap-1.5 text-xs text-muted-foreground hover:text-red-400 transition-colors"
                  disabled={isPending}
                >
                  <Trash2 size={12} />
                  {t('detail.delete')}
                </button>
              )}
            </div>

            {/* Tabs */}
            <div className="flex-none border-t border-border">
              <div className="flex">
                <button
                  type="button"
                  onClick={() => setActiveTab('comments')}
                  className={cn(
                    'flex-1 flex items-center justify-center gap-2 py-3 text-sm font-medium transition-colors border-b-2',
                    activeTab === 'comments'
                      ? 'text-foreground border-indigo-500'
                      : 'text-muted-foreground border-transparent hover:text-foreground'
                  )}
                >
                  <MessageSquare size={14} />
                  {t('detail.comments')}
                </button>
                <button
                  type="button"
                  onClick={() => setActiveTab('activity')}
                  className={cn(
                    'flex-1 flex items-center justify-center gap-2 py-3 text-sm font-medium transition-colors border-b-2',
                    activeTab === 'activity'
                      ? 'text-foreground border-indigo-500'
                      : 'text-muted-foreground border-transparent hover:text-foreground'
                  )}
                >
                  <Activity size={14} />
                  {t('detail.activityLog')}
                </button>
              </div>

              <div className="h-50 overflow-y-auto px-6 py-4">
                {activeTab === 'comments' ? (
                  detail.comments.length === 0 ? (
                    <p className="text-sm text-muted-foreground text-center py-6">
                      {t('detail.noComments')}
                    </p>
                  ) : (
                    <div className="space-y-4">
                      {detail.comments.map((comment) => (
                        <div key={comment.id} className="flex gap-3">
                          <span className="shrink-0 inline-flex size-7 items-center justify-center rounded-full bg-indigo-500/20 text-indigo-400 text-[10px] font-semibold">
                            {getInitials(comment.author.fullName)}
                          </span>
                          <div className="space-y-0.5">
                            <div className="flex items-center gap-2">
                              <span className="text-xs font-semibold text-foreground">
                                {comment.author.fullName}
                              </span>
                              <span className="text-xs text-muted-foreground">
                                {formatRelative(comment.createdAt, i18n.language)}
                              </span>
                            </div>
                            <p className="text-sm text-foreground">{comment.content}</p>
                          </div>
                        </div>
                      ))}
                    </div>
                  )
                ) : detail.log.length === 0 ? (
                  <p className="text-sm text-muted-foreground text-center py-6">
                    {t('detail.noActivity')}
                  </p>
                ) : (
                  <div className="space-y-3">
                    {detail.log.map((entry, i) => (
                      <div key={i} className="flex gap-3">
                        <span className="shrink-0 inline-flex size-6 items-center justify-center rounded-full bg-muted text-muted-foreground text-[9px] font-semibold">
                          {getInitials(entry.user.fullName)}
                        </span>
                        <div className="space-y-0.5">
                          <span className="text-xs text-muted-foreground">
                            {formatRelative(entry.timestamp, i18n.language)}
                          </span>
                          <p className="text-xs text-foreground">
                            <span className="font-medium">{entry.user.fullName}</span>
                            {' '}
                            {t('detail.changed')}
                            {' '}
                            <span className="text-muted-foreground">
                              {fieldDisplayNames[entry.fieldName] ?? entry.fieldName}
                            </span>
                            {' '}
                            {t('detail.from')}
                            {' '}
                            <span className="line-through text-muted-foreground">
                              {resolveActivityValue(entry.fieldName, entry.oldValue.value)}
                            </span>
                            {' '}
                            {t('detail.to')}
                            {' '}
                            <span className="font-medium">
                              {resolveActivityValue(entry.fieldName, entry.newValue.value)}
                            </span>
                          </p>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  )
}
