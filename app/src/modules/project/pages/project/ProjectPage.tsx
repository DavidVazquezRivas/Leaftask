import { useState } from 'react'
import { X } from 'lucide-react'

import { OrbitalTree } from '@/shared/components/OrbitalTree'

import { projectNodeAdapter } from './components/projectNodeAdapter'
import { type WorkItemMock, workItemsMock } from './components/projectTree.mock'

const statusLabel: Record<WorkItemMock['statusId'], string> = {
  backlog: 'Backlog',
  'in-progress': 'In Progress',
  done: 'Done',
  blocked: 'Blocked',
}

const priorityLabel: Record<WorkItemMock['priority'], string> = {
  low: 'Low',
  medium: 'Medium',
  high: 'High',
  critical: 'Critical',
}

const priorityColor: Record<WorkItemMock['priority'], string> = {
  low: 'text-indigo-400',
  medium: 'text-emerald-400',
  high: 'text-amber-400',
  critical: 'text-orange-400',
}

export function ProjectPage() {
  const [selected, setSelected] = useState<WorkItemMock | null>(null)

  return (
    <div className="relative h-full w-full overflow-hidden">
      <OrbitalTree
        data={workItemsMock}
        nodeAdapter={projectNodeAdapter}
        onClickNode={setSelected}
      />

      {selected && (
        <div
          className="absolute inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm"
          onClick={() => setSelected(null)}
        >
          <div
            className="w-80 rounded-2xl border border-white/10 bg-gray-950 p-6 shadow-2xl"
            onClick={(e) => e.stopPropagation()}
          >
            {/* Header */}
            <div className="mb-4 flex items-start justify-between gap-3">
              <div>
                <span className="font-mono text-xs text-gray-500">{selected.code}</span>
                <h2 className="mt-0.5 text-base font-semibold text-white leading-snug">
                  {selected.title}
                </h2>
              </div>
              <button
                type="button"
                onClick={() => setSelected(null)}
                className="mt-0.5 shrink-0 rounded-lg p-1 text-gray-500 transition-colors hover:bg-white/10 hover:text-white"
              >
                <X size={16} />
              </button>
            </div>

            {/* Badges */}
            <div className="mb-5 flex flex-wrap gap-2">
              <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ring-1 ring-white/10 ${selected.type === 'bug' ? 'bg-red-500/15 text-red-400' : 'bg-white/5 text-gray-300'}`}>
                {selected.type === 'bug' ? 'Bug' : 'Task'}
              </span>
              <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ring-1 ring-white/10 bg-white/5 ${priorityColor[selected.priority]}`}>
                {priorityLabel[selected.priority]}
              </span>
              <span className="rounded-full bg-white/5 px-2.5 py-0.5 text-xs font-medium text-gray-300 ring-1 ring-white/10">
                {statusLabel[selected.statusId]}
              </span>
            </div>

            {/* Progress */}
            <div className="mb-5">
              <div className="mb-1.5 flex items-center justify-between text-xs text-gray-500">
                <span>Progress</span>
                <span className="font-medium text-gray-300">{Math.round(selected.progress * 100)}%</span>
              </div>
              <div className="h-1.5 w-full overflow-hidden rounded-full bg-white/10">
                <div
                  className="h-full rounded-full bg-white/60 transition-all"
                  style={{ width: `${selected.progress * 100}%` }}
                />
              </div>
            </div>

            {/* Details */}
            <div className="space-y-2.5 text-sm">
              <div className="flex items-center justify-between">
                <span className="text-gray-500">Estimation</span>
                <span className="text-gray-200">{selected.estimation}h</span>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-gray-500">Assignee</span>
                <span className="text-gray-200">
                  {selected.assignee ? selected.assignee.fullName : '—'}
                </span>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
