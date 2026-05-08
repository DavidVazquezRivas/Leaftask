import { FolderKanban, Settings } from 'lucide-react'
import { Link } from 'react-router-dom'

import type { SimpleProjectData } from '@/core/api/project/management'
import { AppPaths } from '@/core/router'
import { CreateProjectButton } from '@/shared/components/layouts/components/project-creation'
import { ProjectsList } from '@/shared/components/layouts/components/ProjectsList'
import { SecondaryPanel } from '@/shared/components/layouts/components/SecondaryPanel'
import { Button } from '@/shared/components/ui/button'

interface PrivateContextPanelProps {
  title: string
  subtitle: string
  isOrganizationContext: boolean
  organizationSettingsPath: string | null
  organizationId: string | null
  projects: SimpleProjectData[]
  isProjectsLoading: boolean
  projectsEmptyLabel: string
  canCreateProject: boolean
  personalSettingsLabel: string
  organizationSettingsLabel: string
  displayName: string
  rolePlaceholderLabel: string
}

export function PrivateContextPanel({
  title,
  subtitle,
  isOrganizationContext,
  organizationSettingsPath,
  organizationId,
  projects,
  isProjectsLoading,
  projectsEmptyLabel,
  canCreateProject,
  personalSettingsLabel,
  organizationSettingsLabel,
  displayName,
  rolePlaceholderLabel,
}: PrivateContextPanelProps) {
  const userFooter = (
    <div className="flex items-center justify-between gap-3">
      <div className="min-w-0">
        <p className="truncate text-sm font-semibold">{displayName}</p>
        <p className="truncate text-xs text-muted-foreground">
          {rolePlaceholderLabel}
        </p>
      </div>

      <Button
        asChild
        size="icon-sm"
        variant="ghost"
        aria-label={personalSettingsLabel}
      >
        <Link to={AppPaths.APP_PROFILE} title={personalSettingsLabel}>
          <Settings />
        </Link>
      </Button>
    </div>
  )

  return (
    <SecondaryPanel
      title={title}
      subtitle={subtitle}
      titleIcon={<FolderKanban className="size-4" />}
      content={
        <ProjectsList
          projects={projects}
          isLoading={isProjectsLoading}
          emptyLabel={projectsEmptyLabel}
        />
      }
      primaryAction={
        canCreateProject ? (
          <CreateProjectButton organizationId={organizationId} />
        ) : null
      }
      footer={
        isOrganizationContext ? (
          <div className="space-y-3">
            <Button
              asChild={Boolean(organizationSettingsPath)}
              className="w-full"
              size="sm"
              variant="outline"
              data-icon="inline-start"
              aria-label={organizationSettingsLabel}
              title={organizationSettingsLabel}
              disabled={!organizationSettingsPath}
            >
              {organizationSettingsPath ? (
                <Link to={organizationSettingsPath}>
                  <Settings />
                  {organizationSettingsLabel}
                </Link>
              ) : (
                <>
                  <Settings />
                  {organizationSettingsLabel}
                </>
              )}
            </Button>

            <div className="-mx-5 border-t" />

            {userFooter}
          </div>
        ) : (
          userFooter
        )
      }
    />
  )
}
