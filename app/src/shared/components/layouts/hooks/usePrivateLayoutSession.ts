import { useMemo } from 'react'

import { useAppTranslation } from '@/core/i18n'
import { useSessionMeQuery } from '@/core/query/user/session'

export const usePrivateLayoutSession = () => {
  const { t } = useAppTranslation('global')
  const sessionMe = useSessionMeQuery()

  const displayName = useMemo(() => {
    const userName =
      sessionMe.data?.name ??
      `${sessionMe.data?.firstName ?? ''} ${sessionMe.data?.lastName ?? ''}`.trim() ??
      ''

    return userName || sessionMe.data?.email || t('privatePanel.userUnknown')
  }, [sessionMe.data, t])

  return {
    displayName,
    sessionMe,
  }
}
