import { useNavigate } from 'react-router-dom'

import { ApiGateway } from '@/core/api'
import { clearSession } from '@/core/auth/sessionSelectors'
import { useAppTranslation } from '@/core/i18n'
import { AppPaths } from '@/core/router'
import { Button } from '@/shared/components/ui/button'

export function ProfilePage() {
  const navigate = useNavigate()
  const { t } = useAppTranslation('user')

  return (
    <main className="mx-auto flex w-full max-w-3xl flex-col gap-6">
      <header className="space-y-2">
        <h1 className="text-3xl font-semibold tracking-tight">
          {t('profile.title')}
        </h1>
        <p className="text-muted-foreground">{t('profile.subtitle')}</p>
      </header>

      <section className="rounded-lg border bg-card p-6">
        <p className="mb-4 text-sm text-muted-foreground">
          {t('profile.logoutDescription')}
        </p>
        <Button
          variant="destructive"
          onClick={async () => {
            try {
              await ApiGateway.user.session.logoutSession()
            } catch {
              // Local logout should continue even if backend already considers session invalid.
            } finally {
              clearSession()
              navigate(AppPaths.LOGIN, { replace: true })
            }
          }}
        >
          {t('profile.logout')}
        </Button>
      </section>
    </main>
  )
}
