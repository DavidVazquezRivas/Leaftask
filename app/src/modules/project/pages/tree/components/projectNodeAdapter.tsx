import { Ban } from 'lucide-react'
import type { NodeVisual } from '@/shared/components/OrbitalTree'
import type { WorkItemData, WorkItemTypeData, WorkItemStatusData } from '@/core/api/workitems'

const STATUS_COLORS: Record<string, string> = {
  'Por hacer': '#6b7280',
  'En progreso': '#3b82f6',
  Hecho: '#22c55e',
  Bloqueado: '#ef4444',
}

const DEFAULT_COLOR = '#6b7280'

const getInitials = (firstName: string, lastName: string): string =>
  ((firstName[0] ?? '') + (lastName[0] ?? '')).toUpperCase()

export const makeProjectNodeAdapter = (
  types: WorkItemTypeData[],
  statuses: WorkItemStatusData[]
) => {
  return (raw: WorkItemData): NodeVisual => {
    const typeName = types.find((t) => t.id === raw.typeId)?.name ?? 'Task'
    const statusName = statuses.find((s) => s.id === raw.statusId)?.name ?? ''
    const isBug = typeName === 'Bug'
    const color = STATUS_COLORS[statusName] ?? DEFAULT_COLOR

    return {
      title: raw.code,
      subtitle: raw.title,
      color,
      shape: isBug ? 'spike' : 'circle',
      filled: Math.round(raw.progress * 100),
      size: Math.min((raw.estimation ?? 0) / 80, 1),
      orbits: 0,  // driven by registered work — not yet implemented
      avatar: raw.assignee
        ? getInitials(raw.assignee.firstName, raw.assignee.lastName)
        : '?',
      over: statusName === 'Bloqueado' ? <Ban size={28} color="white" strokeWidth={2.5} /> : null,
    }
  }
}
