import { Building2, Shield, Users } from 'lucide-react'

import { useAppTranslation } from '@/core/i18n'
import { cn } from '@/shared/lib/utils'

export type OrganizationSettingsTab =
  | 'general'
  | 'roles-permissions'
  | 'members'

interface OrganizationSettingsTabsProps {
  activeTab: OrganizationSettingsTab
  onTabChange: (tab: OrganizationSettingsTab) => void
}

export function OrganizationSettingsTabs({
  activeTab,
  onTabChange,
}: OrganizationSettingsTabsProps) {
  const { t } = useAppTranslation('organizations')

  return (
    <div className="grid grid-cols-1 overflow-hidden rounded-lg border bg-card md:grid-cols-3">
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
        <Building2 className="size-4" />
        {t('management.settings.tabs.general')}
      </button>

      <button
        type="button"
        className={cn(
          'flex items-center justify-center gap-2 border-b px-4 py-3 text-sm font-medium md:border-r md:border-b-0',
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
          'flex items-center justify-center gap-2 px-4 py-3 text-sm font-medium',
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
    </div>
  )
}
