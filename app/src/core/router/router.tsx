import { lazy, Suspense } from 'react'
import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom'

import {
  PrivateGuard,
  PublicOnlyGuard,
  RootRedirect,
} from '@/core/router/guards'
import { AppPaths } from '@/core/router/paths'
import { PrivateLayout, PublicLayout } from '@/shared/components/layouts'

const LoginPage = lazy(() =>
  import('@/modules/user/pages/login').then((m) => ({ default: m.LoginPage }))
)
const ProfilePage = lazy(() =>
  import('@/modules/user/pages/profile').then((m) => ({ default: m.ProfilePage }))
)
const OrganizationPage = lazy(() =>
  import('@/modules/organization/pages/organization').then((m) => ({ default: m.OrganizationPage }))
)
const OrganizationSettingsPage = lazy(() =>
  import('@/modules/organization/pages/settings').then((m) => ({ default: m.OrganizationSettingsPage }))
)
const ProjectTreePage = lazy(() =>
  import('@/modules/project/pages/tree').then((m) => ({ default: m.ProjectTreePage }))
)
const ProjectSettingsPage = lazy(() =>
  import('@/modules/project/pages/settings').then((m) => ({ default: m.ProjectSettingsPage }))
)
const NotificationsPage = lazy(() =>
  import('@/modules/notification').then((m) => ({ default: m.NotificationsPage }))
)
const ChatPage = lazy(() =>
  import('@/modules/chat').then((m) => ({ default: m.ChatPage }))
)
const NotFoundPage = lazy(() =>
  import('@/shared/pages/not-found').then((m) => ({ default: m.NotFoundPage }))
)

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
  return (
    <Suspense>
      <RouterProvider router={router} />
    </Suspense>
  )
}
