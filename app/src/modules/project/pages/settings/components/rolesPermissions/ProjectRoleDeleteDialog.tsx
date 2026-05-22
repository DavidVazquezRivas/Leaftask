import type { ProjectRoleData } from '@/core/api/project/roles'
import { Button } from '@/shared/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog'

interface ProjectRoleDeleteDialogProps {
  roleToDelete: ProjectRoleData | null
  isDeleting: boolean
  onClose: () => void
  onConfirmDelete: (roleId: string) => Promise<void>
  t: (key: string, options?: Record<string, unknown>) => string
}

export function ProjectRoleDeleteDialog({
  roleToDelete,
  isDeleting,
  onClose,
  onConfirmDelete,
  t,
}: ProjectRoleDeleteDialogProps) {
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
            {t('management.rolesPermissions.deleteDialog.title')}
          </DialogTitle>
          <DialogDescription>
            {t('management.rolesPermissions.deleteDialog.description', {
              name: roleToDelete?.name ?? '',
            })}
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
            {t('management.rolesPermissions.deleteDialog.cancel')}
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
              ? t('management.rolesPermissions.deleteDialog.deleting')
              : t('management.rolesPermissions.deleteDialog.confirm')}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
