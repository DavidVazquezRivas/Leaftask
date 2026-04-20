import { useForm } from '@tanstack/react-form'
import { useEffect, useMemo } from 'react'

import type {
  OrganizationRoleData,
  OrganizationRolePermissionAssignmentData,
  OrganizationRolePermissionLevel,
  OrganizationRolesPermissionData,
} from '@/core/api/organization/roles'
import { Button } from '@/shared/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

import {
  getPermissionLevelForRole,
  isOwnerRole,
} from '@/modules/organization/pages/settings/utils/rolesPermissions.utils'

type RoleEditorMode = 'create' | 'edit'

interface OrganizationRoleEditorDialogProps {
  open: boolean
  mode: RoleEditorMode
  role: OrganizationRoleData | null
  catalogPermissions: OrganizationRolesPermissionData[]
  isSubmitting: boolean
  onClose: () => void
  onSubmit: (payload: {
    roleId?: string
    name: string
    permissions: OrganizationRolePermissionAssignmentData[]
  }) => Promise<void>
  t: (key: string, options?: Record<string, unknown>) => string
  tr: (key: string, defaultValue: string) => string
}

export function OrganizationRoleEditorDialog({
  open,
  mode,
  role,
  catalogPermissions,
  isSubmitting,
  onClose,
  onSubmit,
  t,
  tr,
}: OrganizationRoleEditorDialogProps) {
  const form = useForm({
    defaultValues: {
      name: '',
      permissions: [] as OrganizationRolePermissionAssignmentData[],
    },
    onSubmit: async ({ value }) => {
      const permissions =
        ownerRole && role
          ? value.permissions.map((permission) => {
              const currentLevel = getPermissionLevelForRole(
                role,
                permission.id
              )

              return {
                ...permission,
                level:
                  permission.level < currentLevel
                    ? currentLevel
                    : permission.level,
              }
            })
          : value.permissions

      await onSubmit({
        roleId: role?.id,
        name: value.name.trim(),
        permissions,
      })

      form.reset()
      onClose()
    },
  })

  const defaultPermissionAssignments = useMemo(() => {
    return catalogPermissions.map((permission) => ({
      id: permission.id,
      level: 0 as OrganizationRolePermissionLevel,
    }))
  }, [catalogPermissions])

  const ownerRole = role ? isOwnerRole(role) : false

  useEffect(() => {
    if (!open) {
      return
    }

    if (mode === 'create') {
      form.setFieldValue('name', '')
      form.setFieldValue('permissions', defaultPermissionAssignments)
      return
    }

    if (!role) {
      return
    }

    form.setFieldValue('name', role.name)
    form.setFieldValue(
      'permissions',
      catalogPermissions.map((permission) => ({
        id: permission.id,
        level: getPermissionLevelForRole(role, permission.id),
      }))
    )
  }, [catalogPermissions, defaultPermissionAssignments, form, mode, open, role])

  const editorTitle =
    mode === 'create'
      ? tr(
          'management.settings.rolesPermissions.editor.createTitle',
          'Create role'
        )
      : tr('management.settings.rolesPermissions.editor.editTitle', 'Edit role')

  const submitLabel =
    mode === 'create'
      ? tr('management.settings.rolesPermissions.editor.createSubmit', 'Create')
      : tr(
          'management.settings.rolesPermissions.editor.updateSubmit',
          'Save changes'
        )

  return (
    <Dialog
      open={open}
      onOpenChange={(nextOpen) => {
        if (!nextOpen && !isSubmitting) {
          form.reset()
          onClose()
        }
      }}
    >
      <DialogContent className="max-h-[85vh] overflow-y-auto sm:max-w-3xl">
        <DialogHeader>
          <DialogTitle>{editorTitle}</DialogTitle>
          <DialogDescription>
            {tr(
              'management.settings.rolesPermissions.editor.description',
              'Define the role name and permission levels.'
            )}
          </DialogDescription>
        </DialogHeader>

        <form
          className="space-y-4"
          noValidate
          onSubmit={(event) => {
            event.preventDefault()
            event.stopPropagation()
            void form.handleSubmit()
          }}
        >
          <form.Field
            name="name"
            validators={{
              onSubmit: ({ value }) => {
                return value.trim().length > 0
                  ? undefined
                  : t('management.create.validation.required')
              },
            }}
          >
            {(field) => (
              <div className="space-y-2">
                <Label htmlFor="role-name">
                  {tr(
                    'management.settings.rolesPermissions.editor.roleNameLabel',
                    'Role name'
                  )}
                </Label>
                <Input
                  id="role-name"
                  disabled={isSubmitting}
                  aria-invalid={field.state.meta.errors.length > 0}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(event) => {
                    field.handleChange(event.target.value)
                  }}
                  placeholder={t(
                    'management.settings.rolesPermissions.editor.roleNamePlaceholder',
                    {
                      defaultValue: 'Project Manager',
                    }
                  )}
                />
                {field.state.meta.isTouched && field.state.meta.errors[0] ? (
                  <p className="text-xs text-destructive">
                    {field.state.meta.errors[0]}
                  </p>
                ) : null}
              </div>
            )}
          </form.Field>

          <form.Field name="permissions">
            {(field) => (
              <div className="space-y-3">
                <p className="text-sm font-semibold">
                  {tr(
                    'management.settings.rolesPermissions.editor.permissionsTitle',
                    'Permissions'
                  )}
                </p>

                {catalogPermissions.map((permission) => {
                  const selectedLevel =
                    field.state.value.find(
                      (value) => value.id === permission.id
                    )?.level ?? 0

                  return (
                    <div
                      key={permission.id}
                      className="flex flex-col gap-3 rounded-md border p-3 sm:flex-row sm:items-start sm:justify-between"
                    >
                      <div className="min-w-0">
                        <p className="text-sm font-semibold">
                          {permission.name}
                        </p>
                        <p className="text-xs text-muted-foreground">
                          {permission.description}
                        </p>
                      </div>

                      <Select
                        value={String(selectedLevel)}
                        onValueChange={(value) => {
                          const nextLevel = Number(
                            value
                          ) as OrganizationRolePermissionLevel

                          if (ownerRole && nextLevel < selectedLevel) {
                            return
                          }

                          const nextPermissions = field.state.value.map(
                            (item) => {
                              if (item.id !== permission.id) {
                                return item
                              }

                              return {
                                ...item,
                                level: nextLevel,
                              }
                            }
                          )

                          field.handleChange(nextPermissions)
                        }}
                        disabled={isSubmitting}
                      >
                        <SelectTrigger className="w-full sm:w-36">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem
                            value="0"
                            disabled={ownerRole && selectedLevel > 0}
                          >
                            {t(
                              'management.settings.rolesPermissions.levels.none'
                            )}
                          </SelectItem>
                          <SelectItem
                            value="1"
                            disabled={ownerRole && selectedLevel > 1}
                          >
                            {t(
                              'management.settings.rolesPermissions.levels.supervised'
                            )}
                          </SelectItem>
                          <SelectItem value="2">
                            {t(
                              'management.settings.rolesPermissions.levels.full'
                            )}
                          </SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                  )
                })}
              </div>
            )}
          </form.Field>

          <DialogFooter>
            <Button
              type="button"
              variant="ghost"
              disabled={isSubmitting}
              onClick={() => {
                form.reset()
                onClose()
              }}
            >
              {tr(
                'management.settings.rolesPermissions.editor.cancel',
                'Cancel'
              )}
            </Button>

            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting
                ? tr(
                    'management.settings.rolesPermissions.editor.submitting',
                    'Saving...'
                  )
                : submitLabel}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
