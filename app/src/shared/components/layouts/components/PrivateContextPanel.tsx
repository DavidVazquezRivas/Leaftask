import { FolderKanban, Plus, Settings } from 'lucide-react'
import { Link } from 'react-router-dom'

import { AppPaths } from '@/core/router'
import { SecondaryPanel } from '@/shared/components/layouts/components'
import { Button } from '@/shared/components/ui/button'

interface PrivateContextPanelProps {
  title: string
  subtitle: string
  isOrganizationContext: boolean
  organizationSettingsPath: string | null
  personalPlaceholderLabel: string
  organizationPlaceholderLabel: string
  personalActionLabel: string
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
  personalPlaceholderLabel,
  organizationPlaceholderLabel,
  personalActionLabel,
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
        <div className="space-y-5">
          <p className="text-sm text-muted-foreground">
            {isOrganizationContext
              ? organizationPlaceholderLabel
              : personalPlaceholderLabel}
          </p>
        </div>
      }
      primaryAction={
        isOrganizationContext ? null : (
          <Button className="w-full" variant="outline" data-icon="inline-start">
            <Plus />
            {personalActionLabel}
          </Button>
        )
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
