import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom'

import {
  PrivateGuard,
  PublicOnlyGuard,
  RootRedirect,
} from '@/core/router/guards'
import { AppPaths } from '@/core/router/paths'
import { PrivateLayout, PublicLayout } from '@/shared/components/layouts'
import { LoginPage } from '@/modules/user/pages/login'
import { OrganizationPage } from '@/modules/organization/pages/organization'
import { OrganizationSettingsPage } from '@/modules/organization/pages/settings'
import { ProjectTreePage } from '@/modules/project/pages/tree'
import { ProjectSettingsPage } from '@/modules/project/pages/settings'
import { ProfilePage } from '@/modules/user/pages/profile'
import { ChatPage } from '@/modules/chat'
import { NotificationsPage } from '@/modules/notification'
import { NotFoundPage } from '@/shared/pages/not-found'

const router = createBrowserRouter([
  {
    path: AppPaths.ROOT,
    element: <RootRedirect />,
  },
  {
    element: <PublicLayout />,
    children: [
      {
        path: AppPaths.LOGIN,
        element: (
          <PublicOnlyGuard>
            <LoginPage />
          </PublicOnlyGuard>
        ),
      },
      {
        path: '*',
        element: <NotFoundPage />,
      },
    ],
  },
  {
    element: (
      <PrivateGuard>
        <PrivateLayout />
      </PrivateGuard>
    ),
    children: [
      {
        path: AppPaths.APP_HOME,
        element: <Navigate to={AppPaths.APP_PROFILE} replace />,
      },
      {
        path: AppPaths.APP_PROFILE,
        element: <ProfilePage />,
      },
      {
        path: AppPaths.APP_ORGANIZATION,
        element: <OrganizationPage />,
      },
      {
        path: AppPaths.APP_ORGANIZATION_SETTINGS,
        element: <OrganizationSettingsPage />,
      },
      {
        path: AppPaths.APP_PROJECT,
        element: <ProjectTreePage />,
      },
      {
        path: AppPaths.APP_PROJECT_SETTINGS,
        element: <ProjectSettingsPage />,
      },
      {
        path: AppPaths.APP_NOTIFICATIONS,
        element: <NotificationsPage />,
      },
      {
        path: AppPaths.APP_CHAT,
        element: <ChatPage />,
      },
      {
        path: AppPaths.APP_CHAT_DETAIL,
        element: <ChatPage />,
      },
    ],
  },
])

export function AppRouter() {
  return <RouterProvider router={router} />
}
