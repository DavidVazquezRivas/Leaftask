import { ArrowLeft, House } from 'lucide-react'
import { Link } from 'react-router-dom'
import { useNavigate } from 'react-router-dom'

import { useAppTranslation } from '@/core/i18n'
import { AppPaths } from '@/core/router/paths'
import { Button } from '@/shared/components/ui/button'

export function NotFoundPage() {
  const navigate = useNavigate()
  const { t } = useAppTranslation('global')

  return (
    <main className="mx-auto flex min-h-screen w-full max-w-2xl flex-col items-center justify-center p-6">
      <section className="w-full rounded-2xl border bg-card p-8 text-center shadow-sm sm:p-10">
        <p className="text-sm font-medium tracking-wider text-muted-foreground">
          {t('notFound.code')}
        </p>

        <h1 className="mt-3 text-3xl font-semibold tracking-tight sm:text-4xl">
          {t('notFound.title')}
        </h1>

        <p className="mx-auto mt-3 max-w-xl text-muted-foreground">
          {t('notFound.description')}
        </p>

        <div className="mt-8 flex flex-col items-stretch justify-center gap-3 sm:flex-row">
          <Button
            variant="outline"
            onClick={() => navigate(-1)}
            data-icon="inline-start"
          >
            <ArrowLeft />
            {t('notFound.back')}
          </Button>

          <Button asChild data-icon="inline-start">
            <Link to={AppPaths.ROOT}>
              <House />
              {t('notFound.home')}
            </Link>
          </Button>
        </div>
      </section>
    </main>
  )
}
