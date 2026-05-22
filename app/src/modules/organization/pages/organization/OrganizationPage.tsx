import { useAppTranslation } from '@/core/i18n'

export function OrganizationPage() {
  const { t } = useAppTranslation('organizations')

  return (
    <main className="mx-auto flex w-full max-w-6xl flex-col gap-6">
      <header className="space-y-2">
        <h1 className="text-3xl font-semibold tracking-tight">
          {t('management.workspace.projectsTitle')}
        </h1>
        <p className="text-muted-foreground">
          {t('management.workspace.placeholder')}
        </p>
      </header>
    </main>
  )
}
