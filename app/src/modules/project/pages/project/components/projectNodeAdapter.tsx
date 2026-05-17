import type { NodeVisual } from '@/shared/components/OrbitalTree'

import type { WorkItemMock, WorkItemPriority } from './projectTree.mock'

const priorityColors: Record<WorkItemPriority, string> = {
  low: '#6366f1',
  medium: '#10b981',
  high: '#f59e0b',
  critical: '#f97316',
}

const getInitials = (fullName: string): string =>
  fullName
    .split(' ')
    .slice(0, 2)
    .map((n) => n[0] ?? '')
    .join('')
    .toUpperCase()

const BlockedOverlay = (
  <svg
    width={52}
    height={52}
    viewBox="0 0 52 52"
    style={{ filter: 'drop-shadow(0 0 6px rgba(239,68,68,0.9))' }}
  >
    <circle
      cx="26"
      cy="26"
      r="22"
      fill="rgba(239,68,68,0.18)"
      stroke="#ef4444"
      strokeWidth="3.5"
    />
    <line
      x1="11"
      y1="11"
      x2="41"
      y2="41"
      stroke="#ef4444"
      strokeWidth="3.5"
      strokeLinecap="round"
    />
  </svg>
)

export const projectNodeAdapter = (raw: WorkItemMock): NodeVisual => {
  const isBug = raw.type === 'bug'
  const color = isBug ? '#ef4444' : priorityColors[raw.priority]
  const orbits = Math.max(0, raw.estimation / 8 - 1)

  return {
    title: raw.code,
    subtitle: raw.title,
    color,
    shape: isBug ? 'spike' : 'circle',
    filled: Math.round(raw.progress * 100),
    orbits,
    avatar: raw.assignee ? getInitials(raw.assignee.fullName) : '?',
    over: raw.statusId === 'blocked' ? BlockedOverlay : null,
  }
}
