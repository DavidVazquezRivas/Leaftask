import { useAppTranslation } from '@/core/i18n'
import { GoogleOAuthLoginButton } from '@/modules/user/pages/login/components'

export function LoginPage() {
  const { t } = useAppTranslation('user')

  return (
    <main className="mx-auto grid w-full max-w-5xl gap-8 lg:grid-cols-[1.1fr_0.9fr] lg:items-center">
      <section className="space-y-5">
        <span className="inline-flex items-center rounded-full border bg-muted px-3 py-1 text-xs font-medium text-muted-foreground">
          {t('login.oauthBadge')}
        </span>

        <h1 className="text-4xl font-semibold tracking-tight sm:text-5xl">
          {t('login.title')}
        </h1>

        <p className="max-w-xl text-base text-muted-foreground sm:text-lg">
          {t('login.subtitle')}
        </p>
      </section>

      <section className="rounded-2xl border bg-card/95 p-7 shadow-sm backdrop-blur-sm sm:p-8">
        <h2 className="text-xl font-semibold tracking-tight">
          {t('login.cardTitle')}
        </h2>

        <p className="mt-2 text-sm text-muted-foreground">
          {t('login.cardDescription')}
        </p>

        <div className="mt-6 rounded-xl border border-dashed bg-muted/50 p-5">
          <GoogleOAuthLoginButton />
        </div>
      </section>
    </main>
  )
}
