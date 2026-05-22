import { AlertTriangle } from 'lucide-react'

import {
  Alert,
  AlertDescription,
  AlertTitle,
} from '@/shared/components/ui/alert'
import { Button } from '@/shared/components/ui/button'

interface ProjectSettingsDangerZoneProps {
  canDelete: boolean
  isDeleting: boolean
  onDelete: () => Promise<void>
  t: (key: string) => string
}

export function ProjectSettingsDangerZone({
  canDelete,
  isDeleting,
  onDelete,
  t,
}: ProjectSettingsDangerZoneProps) {
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
          {canDelete
            ? t('management.settings.general.dangerZone.description')
            : t('management.settings.general.permissions.noDelete')}
        </p>
        <div className="mt-4">
          <Button
            variant="destructive"
            disabled={isDeleting || !canDelete}
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
