import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { X, Pencil, Trash2, Loader2 } from 'lucide-react'

import type { WorkLogData } from '@/core/api/workitems'
import {
  useWorkLogsQuery,
  useLogWorkMutation,
  useUpdateWorkLogMutation,
  useDeleteWorkLogMutation,
} from '@/core/query/workitems'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'
import { Textarea } from '@/shared/components/ui/textarea'
import { NumberInput } from '@/shared/components/ui/number-input'
import { cn } from '@/shared/lib/utils'

interface WorkLogModalProps {
  open: boolean
  projectId: string
  itemId: string
  itemCode: string
  itemTitle: string
  estimation: number | null
  onClose: () => void
}

function today(): string {
  const d = new Date()
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${y}-${m}-${day}`
}

function formatDate(iso: string): string {
  const [y, m, day] = iso.split('-')
  return `${day}/${m}/${y}`
}

function fullName(log: WorkLogData): string {
  return `${log.user.firstName} ${log.user.lastName}`.trim()
}

export function WorkLogModal({
  open,
  projectId,
  itemId,
  itemCode,
  itemTitle,
  estimation,
  onClose,
}: WorkLogModalProps) {
  const { t } = useTranslation('workitems')

  const logsQuery = useWorkLogsQuery(open ? projectId : null, open ? itemId : null)
  const logWorkMutation = useLogWorkMutation(projectId, itemId)
  const updateMutation = useUpdateWorkLogMutation(projectId, itemId)
  const deleteMutation = useDeleteWorkLogMutation(projectId, itemId)

  const logs = logsQuery.data ?? []
  const totalH = logs.reduce((acc, l) => acc + l.dedication, 0)
  const diff = estimation != null ? totalH - estimation : null

  // New log form state
  const [newDate, setNewDate] = useState(today())
  const [newHours, setNewHours] = useState(1)
  const [newDescription, setNewDescription] = useState('')

  // Inline edit state
  const [editingId, setEditingId] = useState<string | null>(null)
  const [draftDate, setDraftDate] = useState('')
  const [draftHours, setDraftHours] = useState(0)
  const [draftDescription, setDraftDescription] = useState('')

  const startEdit = (log: WorkLogData) => {
    setEditingId(log.id)
    setDraftDate(log.date)
    setDraftHours(log.dedication)
    setDraftDescription(log.description)
  }

  const cancelEdit = () => setEditingId(null)

  const saveEdit = () => {
    if (!editingId) return
    updateMutation.mutate(
      {
        logId: editingId,
        payload: {
          dedication: draftHours,
          date: draftDate,
          description: draftDescription.trim() || undefined,
        },
      },
      { onSuccess: () => setEditingId(null) }
    )
  }

  const handleDelete = (logId: string) => {
    deleteMutation.mutate(logId)
  }

  const handleAdd = () => {
    if (newHours <= 0 || !newDate) return
    logWorkMutation.mutate(
      { dedication: newHours, date: newDate, description: newDescription.trim() },
      {
        onSuccess: () => {
          setNewHours(1)
          setNewDescription('')
          setNewDate(today())
        },
      }
    )
  }

  const canAdd = newHours > 0 && newDate.length > 0

  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/60" onClick={onClose} />
      <div className="relative z-10 w-full max-w-lg mx-4 bg-background border border-border rounded-xl shadow-2xl flex flex-col max-h-[90vh]">
        {/* Header */}
        <div className="flex items-start justify-between px-5 pt-5 pb-3 border-b border-border shrink-0">
          <div>
            <h2 className="text-base font-semibold">{t('workLog.title')}</h2>
            <p className="text-xs text-muted-foreground mt-0.5">
              {itemCode} • {itemTitle}
            </p>
          </div>
          <button
            type="button"
            onClick={onClose}
            className="text-muted-foreground hover:text-foreground transition-colors mt-0.5"
          >
            <X size={16} />
          </button>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-3 gap-px bg-border shrink-0">
          {[
            {
              label: t('workLog.totalLogged'),
              value: `${totalH.toFixed(1)}h`,
              className: 'text-violet-400',
            },
            {
              label: t('workLog.estimation'),
              value: estimation != null ? `${estimation}h` : '—',
              className: 'text-foreground',
            },
            {
              label: t('workLog.difference'),
              value: diff != null ? `${diff > 0 ? '+' : ''}${diff.toFixed(1)}h` : '—',
              className: diff == null ? 'text-muted-foreground' : diff > 0 ? 'text-red-400' : 'text-green-400',
            },
          ].map((stat) => (
            <div key={stat.label} className="bg-background px-4 py-3 text-center">
              <p className="text-xs text-muted-foreground mb-1">{stat.label}</p>
              <p className={cn('text-xl font-bold', stat.className)}>{stat.value}</p>
            </div>
          ))}
        </div>

        {/* Log list */}
        <div className="flex-1 overflow-y-auto">
          {logsQuery.isLoading ? (
            <div className="flex justify-center items-center py-10">
              <Loader2 size={18} className="animate-spin text-muted-foreground" />
            </div>
          ) : logs.length === 0 ? (
            <p className="text-sm text-muted-foreground text-center py-8">
              {t('workLog.empty')}
            </p>
          ) : (
            <div className="divide-y divide-border">
              {logs.map((log) =>
                editingId === log.id ? (
                  <div key={log.id} className="px-5 py-3 space-y-2 bg-muted/20">
                    <div className="grid grid-cols-2 gap-2">
                      <div className="space-y-1">
                        <Label className="text-xs">{t('workLog.date')}</Label>
                        <Input
                          type="date"
                          value={draftDate}
                          onChange={(e) => setDraftDate(e.target.value)}
                          className="h-8 text-xs"
                          disabled={updateMutation.isPending}
                        />
                      </div>
                      <div className="space-y-1">
                        <Label className="text-xs">{t('workLog.hours')}</Label>
                        <NumberInput
                          value={draftHours}
                          onChange={setDraftHours}
                          min={0.5}
                          step={0.5}
                          disabled={updateMutation.isPending}
                        />
                      </div>
                    </div>
                    <div className="space-y-1">
                      <Label className="text-xs">{t('workLog.description')}</Label>
                      <Input
                        value={draftDescription}
                        onChange={(e) => setDraftDescription(e.target.value)}
                        className="h-8 text-xs"
                        disabled={updateMutation.isPending}
                      />
                    </div>
                    <div className="flex gap-1 pt-0.5">
                      <Button
                        size="sm"
                        className="h-6 text-xs px-2"
                        onClick={saveEdit}
                        disabled={updateMutation.isPending || draftHours <= 0}
                      >
                        {t('detail.save')}
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        className="h-6 text-xs px-2"
                        onClick={cancelEdit}
                        disabled={updateMutation.isPending}
                      >
                        {t('detail.cancel')}
                      </Button>
                    </div>
                  </div>
                ) : (
                  <div key={log.id} className="px-5 py-3 flex items-start justify-between gap-3">
                    <div className="min-w-0">
                      <div className="flex items-center gap-2">
                        <span className="text-xs text-muted-foreground font-mono">
                          {formatDate(log.date)}
                        </span>
                        <span className="text-sm font-semibold text-violet-400">
                          {log.dedication}h
                        </span>
                      </div>
                      {log.description && (
                        <p className="text-sm mt-0.5 truncate">{log.description}</p>
                      )}
                      <p className="text-xs text-muted-foreground mt-0.5">{fullName(log)}</p>
                    </div>
                    <div className="flex items-center gap-1 shrink-0">
                      <button
                        type="button"
                        onClick={() => startEdit(log)}
                        className="p-1 text-muted-foreground hover:text-foreground transition-colors"
                      >
                        <Pencil size={13} />
                      </button>
                      <button
                        type="button"
                        onClick={() => handleDelete(log.id)}
                        disabled={deleteMutation.isPending}
                        className="p-1 text-muted-foreground hover:text-destructive transition-colors"
                      >
                        <Trash2 size={13} />
                      </button>
                    </div>
                  </div>
                )
              )}
            </div>
          )}
        </div>

        {/* New log form */}
        <div className="border-t border-border px-5 py-4 space-y-3 shrink-0 bg-muted/10">
          <p className="text-xs font-semibold">{t('workLog.newEntry')}</p>
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-1.5">
              <Label className="text-xs">{t('workLog.date')}</Label>
              <Input
                type="date"
                value={newDate}
                onChange={(e) => setNewDate(e.target.value)}
                disabled={logWorkMutation.isPending}
              />
            </div>
            <div className="space-y-1.5">
              <Label className="text-xs">{t('workLog.hours')}</Label>
              <NumberInput
                value={newHours}
                onChange={setNewHours}
                min={0.5}
                step={0.5}
                disabled={logWorkMutation.isPending}
              />
            </div>
          </div>
          <div className="space-y-1.5">
            <Label className="text-xs">{t('workLog.descriptionLabel')}</Label>
            <Textarea
              rows={2}
              value={newDescription}
              onChange={(e) => setNewDescription(e.target.value)}
              placeholder={t('workLog.descriptionPlaceholder')}
              disabled={logWorkMutation.isPending}
            />
          </div>
          <Button
            className="w-full"
            disabled={!canAdd || logWorkMutation.isPending}
            onClick={handleAdd}
          >
            {logWorkMutation.isPending ? (
              <Loader2 size={14} className="mr-2 animate-spin" />
            ) : (
              '+'
            )}{' '}
            {t('workLog.addEntry')}
          </Button>
        </div>
      </div>
    </div>
  )
}
