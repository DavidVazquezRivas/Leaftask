import type { OrganizationManagementPermissionData } from '@/core/api/organization/management'

export type OrganizationPermissionLookup =
  | { id: string; name?: never }
  | { id?: never; name: string }

const isLookupById = (
  lookup: OrganizationPermissionLookup
): lookup is { id: string } => {
  return typeof (lookup as { id?: string }).id === 'string'
}

const normalizePermissionValue = (value: string): string => {
  return value.trim().toLowerCase()
}

export const getOrganizationPermissionLevel = (
  permissions: OrganizationManagementPermissionData[],
  lookup: OrganizationPermissionLookup
): 0 | 1 | 2 => {
  const targetPermission = permissions.find((permission) => {
    if (isLookupById(lookup)) {
      return (
        normalizePermissionValue(permission.id) ===
        normalizePermissionValue(lookup.id)
      )
    }

    return (
      normalizePermissionValue(permission.name) ===
      normalizePermissionValue(lookup.name)
    )
  })

  return targetPermission?.level ?? 0
}

export const hasOrganizationPermissionAtLevel = (
  permissions: OrganizationManagementPermissionData[],
  lookup: OrganizationPermissionLookup,
  minimumLevel: 1 | 2 = 1
): boolean => {
  return getOrganizationPermissionLevel(permissions, lookup) >= minimumLevel
}
