import { Building2, Shield, Users } from 'lucide-react'

import { useAppTranslation } from '@/core/i18n'

export function OrganizationSettingsTabs() {
  const { t } = useAppTranslation('organizations')

  return (
    <div className="grid grid-cols-1 overflow-hidden rounded-lg border bg-card md:grid-cols-3">
      <button
        type="button"
        className="flex items-center justify-center gap-2 border-b bg-muted/50 px-4 py-3 text-sm font-medium md:border-r md:border-b-0"
      >
        <Building2 className="size-4" />
        {t('management.settings.tabs.general')}
      </button>

      <button
        type="button"
        disabled
        className="flex items-center justify-center gap-2 border-b px-4 py-3 text-sm text-muted-foreground md:border-r md:border-b-0"
      >
        <Shield className="size-4" />
        {t('management.settings.tabs.rolesPermissions')}
      </button>

      <button
        type="button"
        disabled
        className="flex items-center justify-center gap-2 px-4 py-3 text-sm text-muted-foreground"
      >
        <Users className="size-4" />
        {t('management.settings.tabs.members')}
      </button>
    </div>
  )
}
