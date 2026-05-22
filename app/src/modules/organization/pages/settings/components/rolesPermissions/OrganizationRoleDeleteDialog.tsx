import type { OrganizationRoleData } from '@/core/api/organization/roles'
import { Button } from '@/shared/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog'

interface OrganizationRoleDeleteDialogProps {
  roleToDelete: OrganizationRoleData | null
  isDeleting: boolean
  onClose: () => void
  onConfirmDelete: (roleId: string) => Promise<void>
  t: (key: string, options?: Record<string, unknown>) => string
}

export function OrganizationRoleDeleteDialog({
  roleToDelete,
  isDeleting,
  onClose,
  onConfirmDelete,
  t,
}: OrganizationRoleDeleteDialogProps) {
  return (
    <Dialog
      open={Boolean(roleToDelete)}
      onOpenChange={(open) => {
        if (!open && !isDeleting) {
          onClose()
        }
      }}
    >
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {t('management.settings.rolesPermissions.deleteDialog.title')}
          </DialogTitle>
          <DialogDescription>
            {t(
              'management.settings.rolesPermissions.deleteDialog.description',
              {
                name: roleToDelete?.name ?? '',
              }
            )}
          </DialogDescription>
        </DialogHeader>

        <DialogFooter>
          <Button
            type="button"
            variant="ghost"
            disabled={isDeleting}
            onClick={() => {
              onClose()
            }}
          >
            {t('management.settings.rolesPermissions.deleteDialog.cancel')}
          </Button>

          <Button
            type="button"
            variant="destructive"
            disabled={isDeleting || !roleToDelete}
            onClick={async () => {
              if (!roleToDelete) {
                return
              }

              await onConfirmDelete(roleToDelete.id)
              onClose()
            }}
          >
            {isDeleting
              ? t('management.settings.rolesPermissions.deleteDialog.deleting')
              : t('management.settings.rolesPermissions.deleteDialog.confirm')}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
