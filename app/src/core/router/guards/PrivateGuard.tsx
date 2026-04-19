import type { PropsWithChildren } from 'react'
import { Navigate } from 'react-router-dom'

import { AppPaths } from '@/core/router/paths'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export function PrivateGuard({ children }: PropsWithChildren) {
  const accessToken = useAuthStore((state) => state.accessToken)

  if (!accessToken) {
    return <Navigate to={AppPaths.LOGIN} replace />
  }

  return children
}
