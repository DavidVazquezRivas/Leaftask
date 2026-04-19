import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom'

import {
  PrivateGuard,
  PublicOnlyGuard,
  RootRedirect,
} from '@/core/router/guards'
import { AppPaths } from '@/core/router/paths'
import { PrivateLayout, PublicLayout } from '@/shared/components/layouts'
import { LoginPage } from '@/modules/user/pages/login'
import { ProfilePage } from '@/modules/user/pages/profile'
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
    ],
  },
])

export function AppRouter() {
  return <RouterProvider router={router} />
}
