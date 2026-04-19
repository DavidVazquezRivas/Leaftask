import { GoogleOAuthProvider } from '@react-oauth/google'
import type { PropsWithChildren } from 'react'
import { Toaster } from 'sonner'

import '@/core/i18n'
import { AuthBootstrapProvider } from '@/core/auth'
import { AppQueryClientProvider } from '@/core/query'
import { Environment } from '@/shared/constants/Environment'

export function AppProviders({ children }: PropsWithChildren) {
  const appContent = (
    <AppQueryClientProvider>
      <AuthBootstrapProvider>
        {children}
        <Toaster richColors position="top-right" />
      </AuthBootstrapProvider>
    </AppQueryClientProvider>
  )

  if (!Environment.GOOGLE_CLIENT_ID) {
    return appContent
  }

  return (
    <GoogleOAuthProvider clientId={Environment.GOOGLE_CLIENT_ID}>
      {appContent}
    </GoogleOAuthProvider>
  )
}
