import { useForm } from '@tanstack/react-form'
import { ChevronDown, ChevronRight } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'

import type {
  ProjectRoleData,
  ProjectRolePermissionAssignmentData,
  ProjectRolePermissionLevel,
  ProjectRolesPermissionData,
} from '@/core/api/project/roles'
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
} from '@/modules/project/pages/settings/utils/rolesPermissions.utils'

type RoleEditorMode = 'create' | 'edit'

interface ProjectRoleEditorDialogProps {
  open: boolean
  mode: RoleEditorMode
  role: ProjectRoleData | null
  catalogPermissions: ProjectRolesPermissionData[]
  isSubmitting: boolean
  onClose: () => void
  onSubmit: (payload: {
    roleId?: string
    name: string
    permissions: ProjectRolePermissionAssignmentData[]
  }) => Promise<void>
  t: (key: string, options?: Record<string, unknown>) => string
  tr: (key: string, defaultValue: string) => string
}

function getGroupLevel(
  groupPermissions: ProjectRolesPermissionData[],
  formPermissions: ProjectRolePermissionAssignmentData[]
): string {
  const levels = groupPermissions.map(
    (gp) =>
      formPermissions.find((fp) => fp.permissionId === gp.id)?.level ?? 0
  )
  return levels.length > 0 && levels.every((l) => l === levels[0])
    ? String(levels[0])
    : ''
}

export function ProjectRoleEditorDialog({
  open,
  mode,
  role,
  catalogPermissions,
  isSubmitting,
  onClose,
  onSubmit,
  t,
  tr,
}: ProjectRoleEditorDialogProps) {
  const [collapsedGroups, setCollapsedGroups] = useState<Set<string>>(
    new Set()
  )

  const form = useForm({
    defaultValues: {
      name: '',
      permissions: [] as ProjectRolePermissionAssignmentData[],
    },
    onSubmit: async ({ value }) => {
      const ownerRole = role ? isOwnerRole(role) : false

      const permissions =
        ownerRole && role
          ? value.permissions.map((permission) => {
              const currentLevel = getPermissionLevelForRole(
                role,
                permission.permissionId
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
      permissionId: permission.id,
      level: 0 as ProjectRolePermissionLevel,
    }))
  }, [catalogPermissions])

  const groupedPermissions = useMemo(() => {
    const map = new Map<string, ProjectRolesPermissionData[]>()
    for (const p of catalogPermissions) {
      if (!map.has(p.permissionType)) map.set(p.permissionType, [])
      map.get(p.permissionType)!.push(p)
    }
    return [...map.entries()]
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
        permissionId: permission.id,
        level: getPermissionLevelForRole(role, permission.id),
      }))
    )
  }, [catalogPermissions, defaultPermissionAssignments, form, mode, open, role])

  const editorTitle =
    mode === 'create'
      ? tr('management.rolesPermissions.editor.createTitle', 'Create role')
      : tr('management.rolesPermissions.editor.editTitle', 'Edit role')

  const submitLabel =
    mode === 'create'
      ? tr('management.rolesPermissions.editor.createSubmit', 'Create')
      : tr('management.rolesPermissions.editor.updateSubmit', 'Save changes')

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
              'management.rolesPermissions.editor.description',
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
                    'management.rolesPermissions.editor.roleNameLabel',
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
                    'management.rolesPermissions.editor.roleNamePlaceholder',
                    { defaultValue: 'Project Manager' }
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
              <div className="space-y-2">
                <p className="text-sm font-semibold">
                  {tr(
                    'management.rolesPermissions.editor.permissionsTitle',
                    'Permissions'
                  )}
                </p>

                {groupedPermissions.map(([groupType, groupPerms]) => {
                  const isCollapsed = collapsedGroups.has(groupType)
                  const groupLevel = getGroupLevel(
                    groupPerms,
                    field.state.value
                  )

                  return (
                    <div
                      key={groupType}
                      className="overflow-hidden rounded-md border"
                    >
                      {/* Group header */}
                      <div className="flex items-center justify-between gap-3 bg-muted/40 px-3 py-2">
                        <button
                          type="button"
                          className="flex min-w-0 items-center gap-2 text-left"
                          onClick={() => {
                            setCollapsedGroups((prev) => {
                              const next = new Set(prev)
                              if (next.has(groupType)) {
                                next.delete(groupType)
                              } else {
                                next.add(groupType)
                              }
                              return next
                            })
                          }}
                        >
                          {isCollapsed ? (
                            <ChevronRight className="size-4 shrink-0 text-muted-foreground" />
                          ) : (
                            <ChevronDown className="size-4 shrink-0 text-muted-foreground" />
                          )}
                          <span className="text-sm font-semibold">
                            {groupType}
                          </span>
                        </button>

                        <Select
                          value={groupLevel}
                          onValueChange={(value) => {
                            const nextLevel =
                              Number(value) as ProjectRolePermissionLevel

                            const updated = field.state.value.map((item) => {
                              if (
                                !groupPerms.some(
                                  (gp) => gp.id === item.permissionId
                                )
                              ) {
                                return item
                              }

                              if (ownerRole && role) {
                                const current = getPermissionLevelForRole(
                                  role,
                                  item.permissionId
                                )
                                if (nextLevel < current) return item
                              }

                              return { ...item, level: nextLevel }
                            })

                            field.handleChange(updated)
                          }}
                          disabled={isSubmitting}
                        >
                          <SelectTrigger className="w-32 shrink-0">
                            <SelectValue
                              placeholder={tr(
                                'management.rolesPermissions.editor.mixedPlaceholder',
                                'Mixed'
                              )}
                            />
                          </SelectTrigger>
                          <SelectContent>
                            <SelectItem value="0">
                              {t('management.rolesPermissions.levels.none')}
                            </SelectItem>
                            <SelectItem value="1">
                              {t(
                                'management.rolesPermissions.levels.supervised'
                              )}
                            </SelectItem>
                            <SelectItem value="2">
                              {t('management.rolesPermissions.levels.full')}
                            </SelectItem>
                          </SelectContent>
                        </Select>
                      </div>

                      {/* Individual permissions */}
                      {!isCollapsed && (
                        <ul>
                          {groupPerms.map((permission) => {
                            const selectedLevel =
                              field.state.value.find(
                                (v) => v.permissionId === permission.id
                              )?.level ?? 0

                            return (
                              <li
                                key={permission.id}
                                className="flex items-center justify-between gap-3 border-t px-3 py-2"
                              >
                                <p className="min-w-0 text-sm">
                                  {permission.description}
                                </p>

                                <Select
                                  value={String(selectedLevel)}
                                  onValueChange={(value) => {
                                    const nextLevel =
                                      Number(
                                        value
                                      ) as ProjectRolePermissionLevel

                                    if (
                                      ownerRole &&
                                      nextLevel < selectedLevel
                                    ) {
                                      return
                                    }

                                    const updated = field.state.value.map(
                                      (item) =>
                                        item.permissionId !== permission.id
                                          ? item
                                          : { ...item, level: nextLevel }
                                    )

                                    field.handleChange(updated)
                                  }}
                                  disabled={isSubmitting}
                                >
                                  <SelectTrigger className="w-32 shrink-0">
                                    <SelectValue />
                                  </SelectTrigger>
                                  <SelectContent>
                                    <SelectItem
                                      value="0"
                                      disabled={
                                        ownerRole && selectedLevel > 0
                                      }
                                    >
                                      {t(
                                        'management.rolesPermissions.levels.none'
                                      )}
                                    </SelectItem>
                                    <SelectItem
                                      value="1"
                                      disabled={
                                        ownerRole && selectedLevel > 1
                                      }
                                    >
                                      {t(
                                        'management.rolesPermissions.levels.supervised'
                                      )}
                                    </SelectItem>
                                    <SelectItem value="2">
                                      {t(
                                        'management.rolesPermissions.levels.full'
                                      )}
                                    </SelectItem>
                                  </SelectContent>
                                </Select>
                              </li>
                            )
                          })}
                        </ul>
                      )}
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
              {tr('management.rolesPermissions.editor.cancel', 'Cancel')}
            </Button>

            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting
                ? tr(
                    'management.rolesPermissions.editor.submitting',
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
