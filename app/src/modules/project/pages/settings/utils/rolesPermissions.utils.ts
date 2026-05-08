import { Check, ShieldAlert, X } from 'lucide-react'

import type {
  ProjectRoleData,
  ProjectRolePermissionLevel,
} from '@/core/api/project/roles'

export const getPermissionLevelMeta = (
  level: ProjectRolePermissionLevel,
  t: (key: string) => string
) => {
  if (level === 2) {
    return {
      Icon: Check,
      iconClassName: 'text-emerald-500',
      badgeClassName: 'bg-emerald-500/15 text-emerald-500',
      label: t('management.rolesPermissions.levels.full'),
    }
  }

  if (level === 1) {
    return {
      Icon: ShieldAlert,
      iconClassName: 'text-amber-500',
      badgeClassName: 'bg-amber-500/15 text-amber-500',
      label: t('management.rolesPermissions.levels.supervised'),
    }
  }

  return {
    Icon: X,
    iconClassName: 'text-slate-400',
    badgeClassName: 'bg-slate-500/15 text-slate-400',
    label: t('management.rolesPermissions.levels.none'),
  }
}

export const getPermissionLevelForRole = (
  role: ProjectRoleData,
  permissionId: string
): ProjectRolePermissionLevel => {
  return (
    role.permissions.find(
      (permission) => permission.permissionId === permissionId
    )?.level ?? 0
  )
}

export const isOwnerRole = (role: ProjectRoleData) => {
  return role.name.trim().toLowerCase() === 'owner'
}
