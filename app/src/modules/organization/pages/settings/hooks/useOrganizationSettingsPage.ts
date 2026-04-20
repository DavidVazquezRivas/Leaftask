import { useMemo, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import {
  useDeleteOrganizationManagementMutation,
  useOrganizationManagementDetailQuery,
  useOrganizationManagementPermissionsQuery,
  useUpdateOrganizationManagementMutation,
} from '@/core/query/organization/management'
import {
  getOrganizationPermissionLevel,
  hasOrganizationPermissionAtLevel,
} from '@/core/permissions/organizationPermissions'
import { AppPaths } from '@/core/router'
import { useOrganizationStore } from '@/core/zustand/organization'
import { useAppTranslation } from '@/core/i18n'

export interface OrganizationSettingsFormValues {
  name: string
  description: string
  website: string
}

const normalizeNullableField = (value: string): string | null => {
  const normalized = value.trim()
  return normalized.length > 0 ? normalized : null
}

const CONFIGURE_ORGANIZATION_PERMISSION = {
  name: 'Configure Organization',
} as const

const INVITE_MEMBERS_PERMISSION = {
  name: 'Invite Members',
} as const

const REMOVE_MEMBERS_PERMISSION = {
  name: 'Remove members',
} as const

export const useOrganizationSettingsPage = () => {
  const { organizationId } = useParams<{ organizationId: string }>()
  const navigate = useNavigate()
  const { t } = useAppTranslation('organizations')
  const setSelectedOrganizationId = useOrganizationStore(
    (state) => state.setSelectedOrganizationId
  )

  const detailQuery = useOrganizationManagementDetailQuery(
    organizationId ?? null
  )
  const permissionsQuery = useOrganizationManagementPermissionsQuery(
    organizationId ?? null
  )
  const updateMutation = useUpdateOrganizationManagementMutation(
    organizationId ?? ''
  )
  const deleteMutation = useDeleteOrganizationManagementMutation(
    organizationId ?? ''
  )

  useEffect(() => {
    if (organizationId) {
      setSelectedOrganizationId(organizationId)
    }
  }, [organizationId, setSelectedOrganizationId])

  const metrics = useMemo(() => {
    const detail = detailQuery.data?.data

    return [
      {
        label: t('management.settings.general.metrics.totalMembers'),
        value: detail?.totalMembers ?? 0,
      },
      {
        label: t('management.settings.general.metrics.activeProjects'),
        value: detail?.activeProjects ?? 0,
      },
      {
        label: t('management.settings.general.metrics.customRoles'),
        value: detail?.customRoles ?? 0,
      },
    ]
  }, [detailQuery.data?.data, t])

  const configureOrganizationPermissionLevel = useMemo(() => {
    const permissions = permissionsQuery.data?.data ?? []

    return getOrganizationPermissionLevel(
      permissions,
      CONFIGURE_ORGANIZATION_PERMISSION
    )
  }, [permissionsQuery.data?.data])

  const hasConfigureOrganizationPermission = useMemo(() => {
    const permissions = permissionsQuery.data?.data ?? []

    return hasOrganizationPermissionAtLevel(
      permissions,
      CONFIGURE_ORGANIZATION_PERMISSION,
      1
    )
  }, [permissionsQuery.data?.data])

  const hasInviteMembersPermission = useMemo(() => {
    const permissions = permissionsQuery.data?.data ?? []

    return hasOrganizationPermissionAtLevel(
      permissions,
      INVITE_MEMBERS_PERMISSION,
      1
    )
  }, [permissionsQuery.data?.data])

  const hasRemoveMembersPermission = useMemo(() => {
    const permissions = permissionsQuery.data?.data ?? []

    return hasOrganizationPermissionAtLevel(
      permissions,
      REMOVE_MEMBERS_PERMISSION,
      1
    )
  }, [permissionsQuery.data?.data])

  const isConfigureOrganizationSupervised =
    configureOrganizationPermissionLevel === 1

  const isBusy =
    detailQuery.isLoading ||
    permissionsQuery.isLoading ||
    updateMutation.isPending ||
    deleteMutation.isPending

  const handleSubmit = async (values: OrganizationSettingsFormValues) => {
    if (!organizationId) {
      return
    }

    await updateMutation.mutateAsync({
      name: normalizeNullableField(values.name),
      description: normalizeNullableField(values.description),
      website: normalizeNullableField(values.website),
    })
  }

  const handleDelete = async () => {
    await deleteMutation.mutateAsync()
    setSelectedOrganizationId(null)
    navigate(AppPaths.APP_PROFILE, { replace: true })
  }

  return {
    detail: detailQuery.data?.data ?? null,
    handleDelete,
    handleSubmit,
    isBusy,
    isDeleting: deleteMutation.isPending,
    isSubmitting: updateMutation.isPending,
    hasConfigureOrganizationPermission,
    hasInviteMembersPermission,
    hasRemoveMembersPermission,
    isConfigureOrganizationSupervised,
    metrics,
    organizationId: organizationId ?? null,
    t,
  }
}
