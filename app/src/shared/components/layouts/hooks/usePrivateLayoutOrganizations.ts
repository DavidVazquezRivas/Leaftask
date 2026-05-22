import { useMemo } from 'react'
import { useNavigate } from 'react-router-dom'

import { useAppTranslation } from '@/core/i18n'
import { useOrganizationManagementInfiniteQuery } from '@/core/query/organization/management'
import { AppPaths } from '@/core/router'
import { useOrganizationStore } from '@/core/zustand/organization'

export const usePrivateLayoutOrganizations = () => {
  const navigate = useNavigate()
  const { t: tGlobal } = useAppTranslation('global')
  const { t: tOrganizations } = useAppTranslation('organizations')
  const selectedOrganizationId = useOrganizationStore(
    (state) => state.selectedOrganizationId
  )
  const setSelectedOrganizationId = useOrganizationStore(
    (state) => state.setSelectedOrganizationId
  )
  const organizationsQuery = useOrganizationManagementInfiniteQuery({
    limit: 10,
    sort: ['name:asc'],
  })

  const organizations = useMemo(
    () => organizationsQuery.data?.data ?? [],
    [organizationsQuery.data]
  )

  const selectedOrganization = useMemo(
    () =>
      organizations.find(
        (organization) => organization.id === selectedOrganizationId
      ) ?? null,
    [organizations, selectedOrganizationId]
  )

  const isOrganizationContext = selectedOrganizationId !== null

  const selectPersonal = () => {
    setSelectedOrganizationId(null)
  }

  const selectOrganization = (organizationId: string) => {
    setSelectedOrganizationId(organizationId)
    navigate(AppPaths.organization(organizationId))
  }

  const panelTitle = isOrganizationContext
    ? (selectedOrganization?.name ?? tOrganizations('management.title'))
    : tGlobal('privatePanel.title')

  const panelSubtitle = isOrganizationContext
    ? tOrganizations('management.workspace.subtitle')
    : tGlobal('privatePanel.subtitle')

  const organizationSidebarLabel = tOrganizations('management.sidebar.label')
  const organizationPlaceholderLabel = tOrganizations(
    'management.workspace.placeholder'
  )
  const organizationSettingsLabel = tOrganizations(
    'management.workspace.settings'
  )

  return {
    isOrganizationContext,
    organizations,
    organizationsQuery,
    organizationPlaceholderLabel,
    organizationSettingsLabel,
    organizationSidebarLabel,
    panelSubtitle,
    panelTitle,
    selectedOrganizationId,
    selectOrganization,
    selectPersonal,
    tGlobal,
  }
}
