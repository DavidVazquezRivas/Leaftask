import { AlertTriangle } from 'lucide-react'

import { useAppTranslation } from '@/core/i18n'
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from '@/shared/components/ui/alert'
import { Button } from '@/shared/components/ui/button'

interface OrganizationSettingsDangerZoneProps {
  hasConfigureOrganizationPermission: boolean
  isDeleting: boolean
  onDelete: () => Promise<void>
}

export function OrganizationSettingsDangerZone({
  hasConfigureOrganizationPermission,
  isDeleting,
  onDelete,
}: OrganizationSettingsDangerZoneProps) {
  const { t } = useAppTranslation('organizations')

  return (
    <Alert
      variant="destructive"
      className="border-destructive/60 bg-destructive/5"
    >
      <AlertTriangle className="size-4" />
      <AlertTitle>
        {t('management.settings.general.dangerZone.title')}
      </AlertTitle>
      <AlertDescription>
        <p>
          {hasConfigureOrganizationPermission
            ? t('management.settings.general.dangerZone.description')
            : t('management.settings.general.permissions.noDelete')}
        </p>
        <div className="mt-4">
          <Button
            variant="destructive"
            disabled={isDeleting || !hasConfigureOrganizationPermission}
            onClick={async () => {
              await onDelete()
            }}
          >
            {isDeleting
              ? t('management.settings.general.dangerZone.deleting')
              : t('management.settings.general.dangerZone.deleteAction')}
          </Button>
        </div>
      </AlertDescription>
    </Alert>
  )
}
