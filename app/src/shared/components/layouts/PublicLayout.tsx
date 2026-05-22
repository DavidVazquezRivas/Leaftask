import { type ReactNode } from 'react'
import { Outlet } from 'react-router-dom'

import { useAppTranslation } from '@/core/i18n'
import {
  LanguageSwitcher,
  ThemeSwitcher,
} from '@/shared/components/layouts/components'

interface PublicLayoutProps {
  children?: ReactNode
}

export function PublicLayout({ children }: PublicLayoutProps) {
  const { t } = useAppTranslation('global')

  return (
    <div className="relative flex min-h-screen flex-col overflow-hidden bg-background">
      <div className="pointer-events-none absolute inset-0 -z-10 bg-[radial-gradient(circle_at_20%_20%,oklch(0.97_0_0),transparent_45%),radial-gradient(circle_at_80%_0%,oklch(0.9_0_0),transparent_35%)] dark:bg-[radial-gradient(circle_at_20%_20%,oklch(0.22_0_0),transparent_45%),radial-gradient(circle_at_80%_0%,oklch(0.26_0_0),transparent_35%)]" />

      <header className="mx-auto flex w-full max-w-6xl items-center justify-between gap-4 px-6 py-6">
        <div>
          <p className="text-lg font-semibold tracking-tight">
            {t('publicLayout.brand')}
          </p>
          <p className="text-xs text-muted-foreground">
            {t('publicLayout.subtitle')}
          </p>
        </div>

        <div className="flex items-center gap-3">
          <LanguageSwitcher />
          <ThemeSwitcher />
        </div>
      </header>

      <main className="mx-auto flex w-full max-w-6xl flex-1 px-6 py-8">
        {children ?? <Outlet />}
      </main>

      <footer className="mx-auto flex w-full max-w-6xl items-center justify-between px-6 pb-8 text-xs text-muted-foreground">
        <span>{t('publicLayout.footer')}</span>
        <span>© {new Date().getFullYear()} Leaftask</span>
      </footer>
    </div>
  )
}
