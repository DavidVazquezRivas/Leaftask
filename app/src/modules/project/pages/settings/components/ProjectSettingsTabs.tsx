import { Settings, Shield, SlidersHorizontal, Users } from 'lucide-react'

import { useAppTranslation } from '@/core/i18n'
import { cn } from '@/shared/lib/utils'

export type ProjectSettingsTab =
  | 'general'
  | 'roles-permissions'
  | 'members'
  | 'custom-fields'

interface ProjectSettingsTabsProps {
  activeTab: ProjectSettingsTab
  onTabChange: (tab: ProjectSettingsTab) => void
}

export function ProjectSettingsTabs({
  activeTab,
  onTabChange,
}: ProjectSettingsTabsProps) {
  const { t } = useAppTranslation('projects')

  return (
    <div className="grid grid-cols-1 overflow-hidden rounded-lg border bg-card md:grid-cols-4">
      <button
        type="button"
        className={cn(
          'flex items-center justify-center gap-2 border-b px-4 py-3 text-sm font-medium md:border-r md:border-b-0',
          activeTab === 'general'
            ? 'bg-muted/50 text-foreground'
            : 'text-muted-foreground'
        )}
        onClick={() => {
          onTabChange('general')
        }}
        aria-pressed={activeTab === 'general'}
      >
        <Settings className="size-4" />
        {t('management.settings.tabs.general')}
      </button>

      <button
        type="button"
        className={cn(
          'flex items-center justify-center gap-2 border-b px-4 py-3 text-sm font-medium md:border-b-0 md:border-r',
          activeTab === 'roles-permissions'
            ? 'bg-muted/50 text-foreground'
            : 'text-muted-foreground'
        )}
        onClick={() => {
          onTabChange('roles-permissions')
        }}
        aria-pressed={activeTab === 'roles-permissions'}
      >
        <Shield className="size-4" />
        {t('management.settings.tabs.rolesPermissions')}
      </button>

      <button
        type="button"
        className={cn(
          'flex items-center justify-center gap-2 border-b px-4 py-3 text-sm font-medium md:border-b-0 md:border-r',
          activeTab === 'members'
            ? 'bg-muted/50 text-foreground'
            : 'text-muted-foreground'
        )}
        onClick={() => {
          onTabChange('members')
        }}
        aria-pressed={activeTab === 'members'}
      >
        <Users className="size-4" />
        {t('management.settings.tabs.members')}
      </button>

      <button
        type="button"
        className={cn(
          'flex items-center justify-center gap-2 px-4 py-3 text-sm font-medium',
          activeTab === 'custom-fields'
            ? 'bg-muted/50 text-foreground'
            : 'text-muted-foreground'
        )}
        onClick={() => {
          onTabChange('custom-fields')
        }}
        aria-pressed={activeTab === 'custom-fields'}
      >
        <SlidersHorizontal className="size-4" />
        {t('management.settings.tabs.customFields')}
      </button>
    </div>
  )
}
