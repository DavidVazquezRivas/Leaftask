import type { PropsWithChildren } from 'react'
import { useEffect, useRef } from 'react'

import { SessionGateway } from '@/core/api/user/session'
import { useAuthStore } from '@/core/zustand/auth/authStore'

import '@/core/api/global/httpClient'

export function AuthBootstrapProvider({ children }: PropsWithChildren) {
  const didBootstrapRef = useRef(false)
  const isBootstrapped = useAuthStore((state) => state.isBootstrapped)
  const setAccessToken = useAuthStore((state) => state.setAccessToken)
  const clearSession = useAuthStore((state) => state.clearSession)
  const setBootstrapped = useAuthStore((state) => state.setBootstrapped)

  useEffect(() => {
    if (didBootstrapRef.current) {
      return
    }

    didBootstrapRef.current = true

    const bootstrapSession = async () => {
      try {
        const accessToken = await SessionGateway.refreshSession()
        setAccessToken(accessToken)
      } catch {
        clearSession()
      } finally {
        setBootstrapped(true)
      }
    }

    void bootstrapSession()
  }, [clearSession, setAccessToken, setBootstrapped])

  if (!isBootstrapped) {
    return null
  }

  return children
}
