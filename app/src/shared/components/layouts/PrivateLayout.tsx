import { Outlet, useNavigate, useLocation } from 'react-router-dom'

import { AppPaths } from '@/core/router'
import {
  PrimarySidebar,
  PrivateContextPanel,
} from '@/shared/components/layouts/components'
import {
  usePrivateLayoutOrganizations,
  usePrivateLayoutProjects,
  usePrivateLayoutSession,
} from '@/shared/components/layouts/hooks'
import { useChatsQuery, useChatPollingQuery } from '@/core/query/chat'
import { useNotificationsQuery, useApprovalsQuery } from '@/core/query/notification'

export function PrivateLayout() {
  const navigate = useNavigate()
  const location = useLocation()
  const isChatActive = location.pathname.startsWith('/app/chats')
  const isNotificationsActive = location.pathname.startsWith('/app/notifications')

  const {
    isOrganizationContext,
    organizations,
    organizationsQuery,
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
  const {
    projects,
    isLoading: isProjectsLoading,
    canCreateProject,
  } = usePrivateLayoutProjects(isOrganizationContext, selectedOrganizationId)

  const chatsQuery = useChatsQuery()
  useChatPollingQuery(isChatActive ? 5_000 : 30_000)

  const totalUnread = (chatsQuery.data ?? []).reduce((sum, c) => sum + c.unreadCount, 0)

  const notificationsQuery = useNotificationsQuery('unread')
  const approvalsQuery = useApprovalsQuery()

  const totalUnreadNotifications =
    (notificationsQuery.data?.pages ?? []).flatMap((p) => p.data).length +
    (approvalsQuery.data?.pages ?? []).flatMap((p) => p.data).filter((a) => a.status === 'pending').length

  const projectsEmptyLabel = isOrganizationContext
    ? tGlobal('organizationPanel.projectsEmpty')
    : tGlobal('privatePanel.projectsEmpty')

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
          isChatActive={isChatActive}
          isNotificationsActive={isNotificationsActive}
          onChatClick={() => navigate(AppPaths.chat())}
          onNotificationsClick={() => navigate(AppPaths.notifications())}
          unreadChatCount={totalUnread}
          unreadNotificationCount={totalUnreadNotifications}
        />

        {!isChatActive && !isNotificationsActive && (
          <PrivateContextPanel
            title={panelTitle}
            subtitle={panelSubtitle}
            isOrganizationContext={isOrganizationContext}
            organizationSettingsPath={
              selectedOrganizationId
                ? AppPaths.organizationSettings(selectedOrganizationId)
                : null
            }
            organizationId={selectedOrganizationId}
            projects={projects}
            isProjectsLoading={isProjectsLoading}
            projectsEmptyLabel={projectsEmptyLabel}
            canCreateProject={canCreateProject}
            personalSettingsLabel={tGlobal('privatePanel.settings')}
            organizationSettingsLabel={organizationSettingsLabel}
            displayName={displayName}
            rolePlaceholderLabel={tGlobal('privatePanel.userRolePlaceholder')}
          />
        )}

        <main className={`min-w-0 flex-1 ${isChatActive ? 'h-full overflow-hidden' : 'overflow-y-auto px-6 py-8'}`}>
          <Outlet />
        </main>
      </div>
    </div>
  )
}
