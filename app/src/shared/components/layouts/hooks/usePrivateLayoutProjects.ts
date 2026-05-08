import { useMemo } from 'react'

import { hasOrganizationPermissionAtLevel } from '@/core/permissions/organizationPermissions'
import { useOrganizationManagementPermissionsQuery } from '@/core/query/organization/management'
import {
  useMyProjectsQuery,
  useOrganizationProjectsQuery,
} from '@/core/query/project'

const CREATE_PROJECTS_PERMISSION = { name: 'Create Projects' } as const

export const usePrivateLayoutProjects = (
  isOrganizationContext: boolean,
  organizationId: string | null
) => {
  const myProjectsQuery = useMyProjectsQuery({
    limit: 50,
    sort: ['name:asc'],
  })

  const orgProjectsQuery = useOrganizationProjectsQuery(
    isOrganizationContext ? organizationId : null,
    { limit: 50, sort: ['name:asc'] }
  )

  const permissionsQuery = useOrganizationManagementPermissionsQuery(
    isOrganizationContext ? organizationId : null
  )

  const activeQuery = isOrganizationContext ? orgProjectsQuery : myProjectsQuery

  const projects = useMemo(
    () => activeQuery.data?.data ?? [],
    [activeQuery.data]
  )

  const canCreateProject = useMemo(() => {
    if (!isOrganizationContext) return true

    const permissions = permissionsQuery.data?.data ?? []
    return hasOrganizationPermissionAtLevel(
      permissions,
      CREATE_PROJECTS_PERMISSION,
      1
    )
  }, [isOrganizationContext, permissionsQuery.data])

  return {
    projects,
    isLoading: activeQuery.isLoading,
    canCreateProject,
  }
}
