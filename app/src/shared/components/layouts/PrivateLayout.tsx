import {
  Bell,
  FolderKanban,
  MessageSquare,
  Plus,
  Settings,
  UserRound,
} from 'lucide-react'
import { Link, Outlet } from 'react-router-dom'

import { useAppTranslation } from '@/core/i18n'
import { useSessionMeQuery } from '@/core/query/user/session'
import { AppPaths } from '@/core/router'
import {
  PrivateLanguageSwitcher,
  SecondaryPanel,
  PrivateThemeToggle,
} from '@/shared/components/layouts/components'
import { Button } from '@/shared/components/ui/button'

export function PrivateLayout() {
  const { t } = useAppTranslation('global')
  const sessionMe = useSessionMeQuery()

  const userName =
    sessionMe.data?.name ??
    `${sessionMe.data?.firstName ?? ''} ${sessionMe.data?.lastName ?? ''}`.trim() ??
    ''

  const displayName =
    userName || sessionMe.data?.email || t('privatePanel.userUnknown')

  const panelContent = (
    <div className="space-y-5">
      <p className="text-sm text-muted-foreground">
        {t('privatePanel.placeholder')}
      </p>
    </div>
  )

  const panelPrimaryAction = (
    <Button className="w-full" variant="outline" data-icon="inline-start">
      <Plus />
      {t('privatePanel.newPersonalProject')}
    </Button>
  )

  const panelFooter = (
    <div className="flex items-center justify-between gap-3">
      <div className="min-w-0">
        <p className="truncate text-sm font-semibold">{displayName}</p>
        <p className="truncate text-xs text-muted-foreground">
          {t('privatePanel.userRolePlaceholder')}
        </p>
      </div>

      <Button
        asChild
        size="icon-sm"
        variant="ghost"
        aria-label={t('privatePanel.settings')}
      >
        <Link to={AppPaths.APP_PROFILE} title={t('privatePanel.settings')}>
          <Settings />
        </Link>
      </Button>
    </div>
  )

  return (
    <div className="h-screen bg-background">
      <div className="flex h-full w-full">
        <aside className="flex h-full w-16 shrink-0 flex-col items-center border-r bg-card/60 py-4 backdrop-blur-sm">
          <Button
            size="icon-sm"
            variant="outline"
            aria-label={t('privateLayout.user')}
            title={t('privateLayout.user')}
          >
            <UserRound />
          </Button>

          <div className="mt-auto flex flex-col items-center gap-2">
            <PrivateLanguageSwitcher />
            <PrivateThemeToggle />

            <Button
              size="icon-sm"
              variant="ghost"
              aria-label={t('privateLayout.notifications')}
              title={t('privateLayout.notifications')}
            >
              <Bell />
            </Button>

            <Button
              size="icon-sm"
              variant="ghost"
              aria-label={t('privateLayout.chat')}
              title={t('privateLayout.chat')}
            >
              <MessageSquare />
            </Button>
          </div>
        </aside>

        <SecondaryPanel
          title={t('privatePanel.title')}
          titleIcon={<FolderKanban className="size-4" />}
          content={panelContent}
          primaryAction={panelPrimaryAction}
          footer={panelFooter}
        />

        <main className="min-w-0 flex-1 overflow-y-auto px-6 py-8">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
