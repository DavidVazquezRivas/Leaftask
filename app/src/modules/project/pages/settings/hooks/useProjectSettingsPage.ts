import { useMemo } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import { useAppTranslation } from '@/core/i18n'
import { hasOrganizationPermissionAtLevel } from '@/core/permissions/organizationPermissions'
import {
  useDeleteProjectMutation,
  usePatchProjectMutation,
  useProjectDetailQuery,
} from '@/core/query/project'
import { useOrganizationManagementPermissionsQuery } from '@/core/query/organization/management'
import { AppPaths } from '@/core/router'
import type { ProjectPrivacy } from '@/core/api/project/management'

export interface ProjectSettingsFormValues {
  name: string
  abbreviation: string
  privacyLevel: ProjectPrivacy
}

const CONFIGURE_PROJECTS_PERMISSION = { name: 'Configure Projects' } as const
const DELETE_PROJECTS_PERMISSION = { name: 'Delete Projects' } as const

export const useProjectSettingsPage = () => {
  const { projectId } = useParams<{ projectId: string }>()
  const navigate = useNavigate()
  const { t } = useAppTranslation('projects')

  const detailQuery = useProjectDetailQuery(projectId ?? null)
  const project = detailQuery.data?.data ?? null
  const organizationId = project?.organizationId ?? null
  const isPersonalProject = organizationId === null

  const permissionsQuery =
    useOrganizationManagementPermissionsQuery(organizationId)
  const permissions = useMemo(
    () => permissionsQuery.data?.data ?? [],
    [permissionsQuery.data]
  )

  const canUpdate = useMemo(
    () =>
      isPersonalProject ||
      hasOrganizationPermissionAtLevel(
        permissions,
        CONFIGURE_PROJECTS_PERMISSION,
        1
      ),
    [isPersonalProject, permissions]
  )

  const canDelete = useMemo(
    () =>
      isPersonalProject ||
      hasOrganizationPermissionAtLevel(
        permissions,
        DELETE_PROJECTS_PERMISSION,
        1
      ),
    [isPersonalProject, permissions]
  )

  const patchMutation = usePatchProjectMutation(projectId ?? '')
  const deleteMutation = useDeleteProjectMutation(projectId ?? '')

  const isBusy =
    detailQuery.isLoading || patchMutation.isPending || deleteMutation.isPending

  const handleSubmit = async (values: ProjectSettingsFormValues) => {
    if (!projectId) return

    await patchMutation.mutateAsync({
      name: values.name.trim() || null,
      abbreviation: values.abbreviation.trim() || null,
      privacyLevel: values.privacyLevel,
    })
  }

  const handleDelete = async () => {
    await deleteMutation.mutateAsync()

    if (organizationId) {
      navigate(AppPaths.organization(organizationId), { replace: true })
    } else {
      navigate(AppPaths.APP_PROFILE, { replace: true })
    }
  }

  return {
    detail: project,
    projectId: projectId ?? null,
    handleDelete,
    handleSubmit,
    isBusy,
    isDeleting: deleteMutation.isPending,
    isSubmitting: patchMutation.isPending,
    canUpdate,
    canDelete,
    t,
  }
}
