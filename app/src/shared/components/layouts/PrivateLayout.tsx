import { Outlet } from 'react-router-dom'

import { AppPaths } from '@/core/router'
import {
  PrimarySidebar,
  PrivateContextPanel,
} from '@/shared/components/layouts/components'
import {
  usePrivateLayoutOrganizations,
  usePrivateLayoutSession,
} from '@/shared/components/layouts/hooks'

export function PrivateLayout() {
  const {
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
  } = usePrivateLayoutOrganizations()
  const { displayName } = usePrivateLayoutSession()

  return (
    <div className="h-screen bg-background">
      <div className="flex h-full w-full">
        <PrimarySidebar
          organizations={organizations}
          selectedOrganizationId={selectedOrganizationId}
          onSelectPersonal={selectPersonal}
          onSelectOrganization={selectOrganization}
          onPersonalLabel={tGlobal('privateLayout.user')}
          onNotificationsLabel={tGlobal('privateLayout.notifications')}
          onChatLabel={tGlobal('privateLayout.chat')}
          onOrganizationsLabel={organizationSidebarLabel}
          isLoading={organizationsQuery.isLoading}
        />

        <PrivateContextPanel
          title={panelTitle}
          subtitle={panelSubtitle}
          isOrganizationContext={isOrganizationContext}
          organizationSettingsPath={
            selectedOrganizationId
              ? AppPaths.organizationSettings(selectedOrganizationId)
              : null
          }
          personalPlaceholderLabel={tGlobal('privatePanel.placeholder')}
          organizationPlaceholderLabel={organizationPlaceholderLabel}
          personalActionLabel={tGlobal('privatePanel.newPersonalProject')}
          personalSettingsLabel={tGlobal('privatePanel.settings')}
          organizationSettingsLabel={organizationSettingsLabel}
          displayName={displayName}
          rolePlaceholderLabel={tGlobal('privatePanel.userRolePlaceholder')}
        />

        <main className="min-w-0 flex-1 overflow-y-auto px-6 py-8">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
