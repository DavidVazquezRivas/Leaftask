import { useForm } from '@tanstack/react-form'
import { useEffect } from 'react'

import { useAppTranslation } from '@/core/i18n'
import type { OrganizationManagementDetailData } from '@/core/api/organization/management'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'
import { Textarea } from '@/shared/components/ui/textarea'

import type { OrganizationSettingsFormValues } from '@/modules/organization/pages/settings/hooks/useOrganizationSettingsPage'

interface OrganizationSettingsGeneralFormProps {
  detail: OrganizationManagementDetailData | null
  hasConfigureOrganizationPermission: boolean
  isConfigureOrganizationSupervised: boolean
  isSubmitting: boolean
  isBusy: boolean
  metrics: Array<{ label: string; value: number }>
  onSubmit: (values: OrganizationSettingsFormValues) => Promise<void>
}

export function OrganizationSettingsGeneralForm({
  detail,
  hasConfigureOrganizationPermission,
  isConfigureOrganizationSupervised,
  isSubmitting,
  isBusy,
  metrics,
  onSubmit,
}: OrganizationSettingsGeneralFormProps) {
  const { t } = useAppTranslation('organizations')

  const form = useForm({
    defaultValues: {
      name: '',
      description: '',
      website: '',
    } as OrganizationSettingsFormValues,
    onSubmit: async ({ value }) => {
      await onSubmit(value)
    },
  })

  useEffect(() => {
    if (!detail) {
      return
    }

    form.setFieldValue('name', detail.name ?? '')
    form.setFieldValue('description', detail.description ?? '')
    form.setFieldValue('website', detail.website ?? '')
  }, [detail, form])

  return (
    <form
      className="space-y-6"
      noValidate
      onSubmit={(event) => {
        event.preventDefault()
        event.stopPropagation()
        void form.handleSubmit()
      }}
    >
      <section className="space-y-5 rounded-lg border bg-card p-6">
        {!hasConfigureOrganizationPermission ? (
          <p className="text-sm text-muted-foreground">
            {t('management.settings.general.permissions.noConfigure')}
          </p>
        ) : null}

        {isConfigureOrganizationSupervised ? (
          <p className="text-sm text-muted-foreground">
            {t('management.settings.general.permissions.supervisedHint')}
          </p>
        ) : null}

        <form.Field
          name="name"
          validators={{
            onSubmit: ({ value }) => {
              return value.trim().length > 0
                ? undefined
                : t('management.settings.general.validation.required')
            },
          }}
        >
          {(field) => (
            <div className="space-y-2">
              <Label htmlFor="org-settings-name">
                {t('management.settings.general.fields.name.label')}
              </Label>
              <Input
                id="org-settings-name"
                disabled={isBusy || !hasConfigureOrganizationPermission}
                aria-invalid={field.state.meta.errors.length > 0}
                value={field.state.value}
                onBlur={field.handleBlur}
                onChange={(event) => {
                  field.handleChange(event.target.value)
                }}
                placeholder={t(
                  'management.settings.general.fields.name.placeholder'
                )}
              />
              {field.state.meta.isTouched && field.state.meta.errors[0] ? (
                <p className="text-xs text-destructive">
                  {field.state.meta.errors[0]}
                </p>
              ) : null}
              <p className="text-xs text-muted-foreground">
                {t('management.settings.general.fields.name.hint')}
              </p>
            </div>
          )}
        </form.Field>

        <form.Field
          name="description"
          validators={{
            onSubmit: ({ value }) => {
              return value.trim().length > 0
                ? undefined
                : t('management.settings.general.validation.required')
            },
          }}
        >
          {(field) => (
            <div className="space-y-2">
              <Label htmlFor="org-settings-description">
                {t('management.settings.general.fields.description.label')}
              </Label>
              <Textarea
                id="org-settings-description"
                disabled={isBusy || !hasConfigureOrganizationPermission}
                aria-invalid={field.state.meta.errors.length > 0}
                value={field.state.value}
                onBlur={field.handleBlur}
                onChange={(event) => {
                  field.handleChange(event.target.value)
                }}
                placeholder={t(
                  'management.settings.general.fields.description.placeholder'
                )}
              />
              {field.state.meta.isTouched && field.state.meta.errors[0] ? (
                <p className="text-xs text-destructive">
                  {field.state.meta.errors[0]}
                </p>
              ) : null}
              <p className="text-xs text-muted-foreground">
                {t('management.settings.general.fields.description.hint')}
              </p>
            </div>
          )}
        </form.Field>

        <form.Field name="website">
          {(field) => (
            <div className="space-y-2">
              <Label htmlFor="org-settings-website">
                {t('management.settings.general.fields.website.label')}
              </Label>
              <Input
                id="org-settings-website"
                disabled={isBusy || !hasConfigureOrganizationPermission}
                value={field.state.value}
                onBlur={field.handleBlur}
                onChange={(event) => {
                  field.handleChange(event.target.value)
                }}
                placeholder={t(
                  'management.settings.general.fields.website.placeholder'
                )}
              />
              <p className="text-xs text-muted-foreground">
                {t('management.settings.general.fields.website.hint')}
              </p>
            </div>
          )}
        </form.Field>

        <div className="grid gap-4 md:grid-cols-3">
          {metrics.map((metric) => (
            <div
              key={metric.label}
              className="rounded-md border bg-muted/30 p-4"
            >
              <p className="text-3xl font-semibold">{metric.value}</p>
              <p className="text-sm text-muted-foreground">{metric.label}</p>
            </div>
          ))}
        </div>

        <div className="flex items-center gap-3">
          <Button
            type="submit"
            disabled={isBusy || !hasConfigureOrganizationPermission}
          >
            {isSubmitting
              ? t('management.settings.general.actions.saving')
              : t('management.settings.general.actions.save')}
          </Button>

          <Button
            type="button"
            variant="outline"
            disabled={isBusy}
            onClick={() => {
              if (!detail) {
                return
              }

              form.setFieldValue('name', detail.name ?? '')
              form.setFieldValue('description', detail.description ?? '')
              form.setFieldValue('website', detail.website ?? '')
            }}
          >
            {t('management.settings.general.actions.cancel')}
          </Button>
        </div>
      </section>
    </form>
  )
}
