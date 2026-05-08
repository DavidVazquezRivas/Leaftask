import { useForm } from '@tanstack/react-form'
import { useEffect } from 'react'

import { ProjectPrivacy } from '@/core/api/project/management'
import type { ProjectData } from '@/core/api/project/management'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

import type { ProjectSettingsFormValues } from '@/modules/project/pages/settings/hooks/useProjectSettingsPage'

interface ProjectSettingsGeneralFormProps {
  detail: ProjectData | null
  canUpdate: boolean
  isSubmitting: boolean
  isBusy: boolean
  onSubmit: (values: ProjectSettingsFormValues) => Promise<void>
  t: (key: string) => string
}

export function ProjectSettingsGeneralForm({
  detail,
  canUpdate,
  isSubmitting,
  isBusy,
  onSubmit,
  t,
}: ProjectSettingsGeneralFormProps) {
  const form = useForm({
    defaultValues: {
      name: '',
      abbreviation: '',
      privacyLevel: ProjectPrivacy.Public,
    } as ProjectSettingsFormValues,
    onSubmit: async ({ value }) => {
      await onSubmit(value)
    },
  })

  useEffect(() => {
    if (!detail) return

    form.setFieldValue('name', detail.name ?? '')
    form.setFieldValue('abbreviation', detail.abbreviation ?? '')
    form.setFieldValue('privacyLevel', detail.privacyLevel)
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
        {!canUpdate ? (
          <p className="text-sm text-muted-foreground">
            {t('management.settings.general.permissions.noConfigure')}
          </p>
        ) : null}

        <form.Field
          name="name"
          validators={{
            onSubmit: ({ value }) =>
              value.trim().length > 0
                ? undefined
                : t('management.settings.general.validation.required'),
          }}
        >
          {(field) => (
            <div className="space-y-2">
              <Label htmlFor="project-settings-name">
                {t('management.settings.general.fields.name.label')}
              </Label>
              <Input
                id="project-settings-name"
                disabled={isBusy || !canUpdate}
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
          name="abbreviation"
          validators={{
            onSubmit: ({ value }) => {
              if (value.trim().length === 0) {
                return t('management.settings.general.validation.required')
              }
              if (value.trim().length > 3) {
                return t(
                  'management.settings.general.validation.abbreviationMaxLength'
                )
              }
              return undefined
            },
          }}
        >
          {(field) => (
            <div className="space-y-2">
              <Label htmlFor="project-settings-abbreviation">
                {t('management.settings.general.fields.abbreviation.label')}
              </Label>
              <Input
                id="project-settings-abbreviation"
                disabled={isBusy || !canUpdate}
                aria-invalid={field.state.meta.errors.length > 0}
                value={field.state.value}
                onBlur={field.handleBlur}
                onChange={(event) => {
                  field.handleChange(event.target.value)
                }}
                placeholder={t(
                  'management.settings.general.fields.abbreviation.placeholder'
                )}
              />
              {field.state.meta.isTouched && field.state.meta.errors[0] ? (
                <p className="text-xs text-destructive">
                  {field.state.meta.errors[0]}
                </p>
              ) : null}
              <p className="text-xs text-muted-foreground">
                {t('management.settings.general.fields.abbreviation.hint')}
              </p>
            </div>
          )}
        </form.Field>

        <form.Field name="privacyLevel">
          {(field) => (
            <div className="space-y-2">
              <Label htmlFor="project-settings-privacy">
                {t('management.settings.general.fields.privacyLevel.label')}
              </Label>
              <Select
                disabled={isBusy || !canUpdate}
                value={String(field.state.value)}
                onValueChange={(value) => {
                  field.handleChange(Number(value) as ProjectPrivacy)
                }}
              >
                <SelectTrigger id="project-settings-privacy">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={String(ProjectPrivacy.Public)}>
                    {t('management.settings.general.privacyOptions.public')}
                  </SelectItem>
                  <SelectItem value={String(ProjectPrivacy.Restricted)}>
                    {t('management.settings.general.privacyOptions.restricted')}
                  </SelectItem>
                  <SelectItem value={String(ProjectPrivacy.Private)}>
                    {t('management.settings.general.privacyOptions.private')}
                  </SelectItem>
                </SelectContent>
              </Select>
              <p className="text-xs text-muted-foreground">
                {t('management.settings.general.fields.privacyLevel.hint')}
              </p>
            </div>
          )}
        </form.Field>

        <div className="flex items-center gap-3">
          <Button type="submit" disabled={isBusy || !canUpdate}>
            {isSubmitting
              ? t('management.settings.general.actions.saving')
              : t('management.settings.general.actions.save')}
          </Button>

          <Button
            type="button"
            variant="outline"
            disabled={isBusy}
            onClick={() => {
              if (!detail) return

              form.setFieldValue('name', detail.name ?? '')
              form.setFieldValue('abbreviation', detail.abbreviation ?? '')
              form.setFieldValue('privacyLevel', detail.privacyLevel)
            }}
          >
            {t('management.settings.general.actions.cancel')}
          </Button>
        </div>
      </section>
    </form>
  )
}
