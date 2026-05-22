import { ChevronDown, ChevronRight, Pencil, Trash2 } from 'lucide-react'
import { useMemo, useState } from 'react'

import type {
  ProjectRoleData,
  ProjectRolePermissionLevel,
  ProjectRolesPermissionData,
} from '@/core/api/project/roles'
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
} from '@/modules/project/pages/settings/utils/rolesPermissions.utils'

interface ProjectRoleCardProps {
  role: ProjectRoleData
  catalogPermissions: ProjectRolesPermissionData[]
  canManageRoles: boolean
  isUpdatingRole: boolean
  t: (key: string, options?: Record<string, unknown>) => string
  onEditRole: (role: ProjectRoleData) => void
  onDeleteRole: (role: ProjectRoleData) => void
  onInlinePermissionLevelChange: (
    role: ProjectRoleData,
    permissionId: string,
    level: ProjectRolePermissionLevel
  ) => void
}

export function ProjectRoleCard({
  role,
  catalogPermissions,
  canManageRoles,
  isUpdatingRole,
  t,
  onEditRole,
  onDeleteRole,
  onInlinePermissionLevelChange,
}: ProjectRoleCardProps) {
  const [isRoleCollapsed, setIsRoleCollapsed] = useState(false)
  const [collapsedGroups, setCollapsedGroups] = useState<Set<string>>(
    new Set()
  )

  const ownerRole = isOwnerRole(role)

  const groupedPermissions = useMemo(() => {
    const map = new Map<string, ProjectRolesPermissionData[]>()
    for (const p of catalogPermissions) {
      if (!map.has(p.permissionType)) map.set(p.permissionType, [])
      map.get(p.permissionType)!.push(p)
    }
    return [...map.entries()]
  }, [catalogPermissions])

  const toggleGroup = (groupType: string) => {
    setCollapsedGroups((prev) => {
      const next = new Set(prev)
      if (next.has(groupType)) {
        next.delete(groupType)
      } else {
        next.add(groupType)
      }
      return next
    })
  }

  return (
    <article className="overflow-hidden rounded-lg border bg-card">
      <header className="flex items-center justify-between gap-3 px-4 py-3">
        <button
          type="button"
          className="flex min-w-0 items-center gap-2 text-left"
          aria-expanded={!isRoleCollapsed}
          onClick={() => {
            setIsRoleCollapsed((v) => !v)
          }}
        >
          {isRoleCollapsed ? (
            <ChevronRight className="size-4 shrink-0 text-muted-foreground" />
          ) : (
            <ChevronDown className="size-4 shrink-0 text-muted-foreground" />
          )}
          <span className="size-2 shrink-0 rounded-full bg-emerald-500" />
          <p className="text-sm font-semibold">{role.name}</p>
          <span className="rounded-md bg-muted px-2 py-0.5 text-xs text-muted-foreground">
            {t('management.rolesPermissions.membersCount', {
              count: role.totalMembers,
            })}
          </span>
        </button>

        <div className="flex shrink-0 items-center gap-1">
          {canManageRoles ? (
            <Button
              type="button"
              size="icon-sm"
              variant="ghost"
              disabled={isUpdatingRole}
              onClick={() => {
                onEditRole(role)
              }}
              aria-label={t('management.rolesPermissions.actions.editRole')}
              title={t('management.rolesPermissions.actions.editRole')}
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
            aria-label={t('management.rolesPermissions.actions.deleteRole')}
            title={t('management.rolesPermissions.actions.deleteRole')}
          >
            <Trash2 className="size-4" />
          </Button>
        </div>
      </header>

      {!isRoleCollapsed && (
        <div className="border-t">
          {groupedPermissions.map(([groupType, groupPerms], groupIndex) => {
            const isGroupCollapsed = collapsedGroups.has(groupType)

            return (
              <div
                key={groupType}
                className={cn(groupIndex > 0 && 'border-t')}
              >
                <button
                  type="button"
                  className="flex w-full items-center gap-2 bg-muted/30 px-4 py-2 text-left"
                  onClick={() => {
                    toggleGroup(groupType)
                  }}
                >
                  {isGroupCollapsed ? (
                    <ChevronRight className="size-3.5 shrink-0 text-muted-foreground" />
                  ) : (
                    <ChevronDown className="size-3.5 shrink-0 text-muted-foreground" />
                  )}
                  <span className="text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                    {groupType}
                  </span>
                </button>

                {!isGroupCollapsed && (
                  <ul>
                    {groupPerms.map((permission) => {
                      const level = getPermissionLevelForRole(
                        role,
                        permission.id
                      )
                      const levelMeta = getPermissionLevelMeta(level, t)

                      return (
                        <li
                          key={`${role.id}-${permission.id}`}
                          className="flex flex-col gap-3 border-t px-4 py-3 sm:flex-row sm:items-center sm:justify-between"
                        >
                          <p className="text-sm">{permission.description}</p>

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
                                    Number(value) as ProjectRolePermissionLevel
                                  )
                                }}
                                disabled={isUpdatingRole}
                              >
                                <SelectTrigger className="ml-auto w-full sm:ml-2 sm:w-40">
                                  <SelectValue />
                                </SelectTrigger>
                                <SelectContent>
                                  <SelectItem
                                    value="0"
                                    disabled={ownerRole && level > 0}
                                  >
                                    {t('management.rolesPermissions.levels.none')}
                                  </SelectItem>
                                  <SelectItem
                                    value="1"
                                    disabled={ownerRole && level > 1}
                                  >
                                    {t(
                                      'management.rolesPermissions.levels.supervised'
                                    )}
                                  </SelectItem>
                                  <SelectItem value="2">
                                    {t('management.rolesPermissions.levels.full')}
                                  </SelectItem>
                                </SelectContent>
                              </Select>
                            ) : null}
                          </div>
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
    </article>
  )
}
