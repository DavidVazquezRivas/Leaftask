import { Pencil, Trash2 } from 'lucide-react'

import type {
  OrganizationRoleData,
  OrganizationRolePermissionLevel,
  OrganizationRolesPermissionData,
} from '@/core/api/organization/roles'
import { Button } from '@/shared/components/ui/button'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'
import { cn } from '@/shared/lib/utils'

import {
  getPermissionLevelForRole,
  getPermissionLevelMeta,
  isOwnerRole,
} from '@/modules/organization/pages/settings/utils/rolesPermissions.utils'

interface OrganizationRoleCardProps {
  role: OrganizationRoleData
  catalogPermissions: OrganizationRolesPermissionData[]
  canManageRoles: boolean
  isUpdatingRole: boolean
  t: (key: string, options?: Record<string, unknown>) => string
  onEditRole: (role: OrganizationRoleData) => void
  onDeleteRole: (role: OrganizationRoleData) => void
  onInlinePermissionLevelChange: (
    role: OrganizationRoleData,
    permissionId: string,
    level: OrganizationRolePermissionLevel
  ) => void
}

export function OrganizationRoleCard({
  role,
  catalogPermissions,
  canManageRoles,
  isUpdatingRole,
  t,
  onEditRole,
  onDeleteRole,
  onInlinePermissionLevelChange,
}: OrganizationRoleCardProps) {
  return (
    <article className="overflow-hidden rounded-lg border bg-card">
      <header className="flex items-center justify-between gap-3 border-b px-4 py-3">
        <div className="flex items-center gap-2">
          <span className="size-2 rounded-full bg-emerald-500" />
          <p className="text-sm font-semibold">{role.name}</p>
          <span className="rounded-md bg-muted px-2 py-0.5 text-xs text-muted-foreground">
            {t('management.settings.rolesPermissions.membersCount', {
              count: role.totalMembers,
            })}
          </span>
        </div>

        <div className="flex items-center gap-1">
          {canManageRoles ? (
            <Button
              type="button"
              size="icon-sm"
              variant="ghost"
              disabled={isUpdatingRole}
              onClick={() => {
                onEditRole(role)
              }}
              aria-label={t(
                'management.settings.rolesPermissions.actions.editRole'
              )}
              title={t('management.settings.rolesPermissions.actions.editRole')}
            >
              <Pencil className="size-4" />
            </Button>
          ) : null}

          <Button
            type="button"
            size="icon-sm"
            variant="ghost"
            disabled={!canManageRoles || isUpdatingRole}
            onClick={() => {
              onDeleteRole(role)
            }}
            aria-label={t(
              'management.settings.rolesPermissions.actions.deleteRole'
            )}
            title={t('management.settings.rolesPermissions.actions.deleteRole')}
          >
            <Trash2 className="size-4" />
          </Button>
        </div>
      </header>

      <ul>
        {catalogPermissions.map((permission) => {
          const level = getPermissionLevelForRole(role, permission.id)
          const levelMeta = getPermissionLevelMeta(level, t)
          const ownerRole = isOwnerRole(role)

          return (
            <li
              key={`${role.id}-${permission.id}`}
              className="flex flex-col gap-3 border-b px-4 py-3 last:border-b-0 sm:flex-row sm:items-center sm:justify-between"
            >
              <div className="space-y-1">
                <p className="text-sm font-semibold">{permission.name}</p>
                <p className="text-sm text-muted-foreground">
                  {permission.description}
                </p>
              </div>

              <div className="flex w-full items-center gap-2 sm:w-auto">
                <levelMeta.Icon
                  className={cn('size-4', levelMeta.iconClassName)}
                />
                <span
                  className={cn(
                    'rounded px-2 py-0.5 text-xs font-semibold',
                    levelMeta.badgeClassName
                  )}
                >
                  {levelMeta.label}
                </span>

                {canManageRoles ? (
                  <Select
                    value={String(level)}
                    onValueChange={(value) => {
                      onInlinePermissionLevelChange(
                        role,
                        permission.id,
                        Number(value) as OrganizationRolePermissionLevel
                      )
                    }}
                    disabled={isUpdatingRole}
                  >
                    <SelectTrigger className="ml-auto w-full sm:ml-2 sm:w-40">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="0" disabled={ownerRole && level > 0}>
                        {t('management.settings.rolesPermissions.levels.none')}
                      </SelectItem>
                      <SelectItem value="1" disabled={ownerRole && level > 1}>
                        {t(
                          'management.settings.rolesPermissions.levels.supervised'
                        )}
                      </SelectItem>
                      <SelectItem value="2">
                        {t('management.settings.rolesPermissions.levels.full')}
                      </SelectItem>
                    </SelectContent>
                  </Select>
                ) : null}
              </div>
            </li>
          )
        })}
      </ul>
    </article>
  )
}
