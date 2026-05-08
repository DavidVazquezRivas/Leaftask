import { useState } from 'react'
import { ChevronDown, Network, Settings } from 'lucide-react'
import { Link, useMatch } from 'react-router-dom'

import type { SimpleProjectData } from '@/core/api/project/management'
import { useAppTranslation } from '@/core/i18n'
import { AppPaths } from '@/core/router'
import { cn } from '@/shared/lib/utils'

interface ProjectsListProps {
  projects: SimpleProjectData[]
  isLoading: boolean
  emptyLabel: string
}

interface ProjectItemProps {
  project: SimpleProjectData
  isExpanded: boolean
  isActive: boolean
  activeSubPath: 'tree' | 'settings' | null
  treeViewLabel: string
  settingsLabel: string
  onToggle: () => void
}

function ProjectItem({
  project,
  isExpanded,
  isActive,
  activeSubPath,
  treeViewLabel,
  settingsLabel,
  onToggle,
}: ProjectItemProps) {
  return (
    <div>
      <button
        type="button"
        onClick={onToggle}
        className={cn(
          'flex w-full items-center justify-between rounded-md px-2 py-1.5 text-sm transition-colors',
          isActive
            ? 'bg-accent font-semibold text-accent-foreground'
            : 'font-medium text-foreground hover:bg-accent/60 hover:text-accent-foreground'
        )}
      >
        <span className="truncate">{project.name}</span>
        <ChevronDown
          className={cn(
            'ml-2 size-3.5 shrink-0 text-muted-foreground transition-transform duration-200',
            isExpanded && 'rotate-180'
          )}
        />
      </button>

      {isExpanded && (
        <div className="mt-0.5 flex flex-col gap-0.5 pl-2">
          <Link
            to={AppPaths.project(project.id)}
            className={cn(
              'flex items-center gap-2 rounded-md px-2 py-1.5 text-sm transition-colors',
              activeSubPath === 'tree'
                ? 'bg-primary font-medium text-primary-foreground'
                : 'text-muted-foreground hover:bg-accent hover:text-accent-foreground'
            )}
          >
            <Network className="size-3.5 shrink-0" />
            {treeViewLabel}
          </Link>

          <Link
            to={AppPaths.projectSettings(project.id)}
            className={cn(
              'flex items-center gap-2 rounded-md px-2 py-1.5 text-sm transition-colors',
              activeSubPath === 'settings'
                ? 'bg-primary font-medium text-primary-foreground'
                : 'text-muted-foreground hover:bg-accent hover:text-accent-foreground'
            )}
          >
            <Settings className="size-3.5 shrink-0" />
            {settingsLabel}
          </Link>
        </div>
      )}
    </div>
  )
}

export function ProjectsList({
  projects,
  isLoading,
  emptyLabel,
}: ProjectsListProps) {
  const { t } = useAppTranslation('global')
  const projectMatch = useMatch(AppPaths.APP_PROJECT)
  const settingsMatch = useMatch(AppPaths.APP_PROJECT_SETTINGS)
  const activeProjectId =
    projectMatch?.params.projectId ?? settingsMatch?.params.projectId ?? null

  // Track which project the user manually expanded/collapsed.
  // null = fall back to auto-behavior (active project is expanded).
  const [manualExpandedId, setManualExpandedId] = useState<string | null>(null)
  const [manualCollapsedId, setManualCollapsedId] = useState<string | null>(
    null
  )

  const isProjectExpanded = (projectId: string): boolean => {
    if (manualExpandedId === projectId) return true
    if (manualCollapsedId === projectId) return false
    return projectId === activeProjectId
  }

  const toggleProject = (projectId: string) => {
    if (isProjectExpanded(projectId)) {
      setManualExpandedId(null)
      setManualCollapsedId(projectId)
    } else {
      setManualExpandedId(projectId)
      setManualCollapsedId(null)
    }
  }

  if (isLoading) {
    return (
      <div className="flex flex-col gap-1">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-8 animate-pulse rounded-md bg-muted" />
        ))}
      </div>
    )
  }

  if (projects.length === 0) {
    return <p className="text-sm text-muted-foreground">{emptyLabel}</p>
  }

  return (
    <div className="flex flex-col gap-0.5">
      {projects.map((project) => {
        const isExpanded = isProjectExpanded(project.id)
        const isActive = activeProjectId === project.id
        const activeSubPath: 'tree' | 'settings' | null = isActive
          ? settingsMatch
            ? 'settings'
            : 'tree'
          : null

        return (
          <ProjectItem
            key={project.id}
            project={project}
            isExpanded={isExpanded}
            isActive={isActive}
            activeSubPath={activeSubPath}
            treeViewLabel={t('projectsPanel.treeView')}
            settingsLabel={t('projectsPanel.settings')}
            onToggle={() => toggleProject(project.id)}
          />
        )
      })}
    </div>
  )
}
