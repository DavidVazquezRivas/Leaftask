import { Check, ShieldAlert, X } from 'lucide-react'

import type {
  OrganizationRoleData,
  OrganizationRolePermissionLevel,
} from '@/core/api/organization/roles'

export const getPermissionLevelMeta = (
  level: OrganizationRolePermissionLevel,
  t: (key: string) => string
) => {
  if (level === 2) {
    return {
      Icon: Check,
      iconClassName: 'text-emerald-500',
      badgeClassName: 'bg-emerald-500/15 text-emerald-500',
      label: t('management.settings.rolesPermissions.levels.full'),
    }
  }

  if (level === 1) {
    return {
      Icon: ShieldAlert,
      iconClassName: 'text-amber-500',
      badgeClassName: 'bg-amber-500/15 text-amber-500',
      label: t('management.settings.rolesPermissions.levels.supervised'),
    }
  }

  return {
    Icon: X,
    iconClassName: 'text-slate-400',
    badgeClassName: 'bg-slate-500/15 text-slate-400',
    label: t('management.settings.rolesPermissions.levels.none'),
  }
}

export const getPermissionLevelForRole = (
  role: OrganizationRoleData,
  permissionId: string
): OrganizationRolePermissionLevel => {
  return (
    role.permissions.find((permission) => permission.id === permissionId)
      ?.level ?? 0
  )
}

export const isOwnerRole = (role: OrganizationRoleData) => {
  return role.name.trim().toLowerCase() === 'owner'
}
