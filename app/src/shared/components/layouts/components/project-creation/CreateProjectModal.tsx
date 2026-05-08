import { useForm } from '@tanstack/react-form'
import { useEffect } from 'react'

import { ProjectPrivacy } from '@/core/api/project/management'
import { useAppTranslation } from '@/core/i18n'
import { useCreateProjectMutation } from '@/core/query/project'
import { Button } from '@/shared/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

interface CreateProjectModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  organizationId: string | null
}

type CreateProjectFormValues = {
  name: string
  abbreviation: string
  privacyLevel: ProjectPrivacy
}

export function CreateProjectModal({
  open,
  onOpenChange,
  organizationId,
}: CreateProjectModalProps) {
  const { t } = useAppTranslation('projects')
  const createProjectMutation = useCreateProjectMutation()
  const isSubmitting = createProjectMutation.isPending

  const form = useForm({
    defaultValues: {
      name: '',
      abbreviation: '',
      privacyLevel: ProjectPrivacy.Public,
    } as CreateProjectFormValues,
    onSubmit: async ({ value }) => {
      await createProjectMutation.mutateAsync({
        name: value.name,
        abbreviation: value.abbreviation,
        privacyLevel: value.privacyLevel,
        organizationId: organizationId ?? undefined,
      })
      onOpenChange(false)
      form.reset()
    },
  })

  useEffect(() => {
    if (!open) {
      form.reset()
    }
  }, [form, open])

  return (
    <Dialog
      open={open}
      onOpenChange={(nextOpen) => {
        if (isSubmitting) return
        onOpenChange(nextOpen)
      }}
    >
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>{t('management.create.modal.title')}</DialogTitle>
          <DialogDescription>
            {t('management.create.modal.description')}
          </DialogDescription>
        </DialogHeader>

        <form
          className="space-y-4"
          noValidate
          onSubmit={(event) => {
            event.preventDefault()
            event.stopPropagation()
            void form.handleSubmit()
          }}
        >
          <form.Field
            name="name"
            validators={{
              onSubmit: ({ value }) =>
                value.trim().length > 0
                  ? undefined
                  : t('management.create.validation.required'),
            }}
          >
            {(field) => (
              <div className="space-y-2">
                <Label htmlFor="create-project-name">
                  {t('management.create.fields.name.label')}
                </Label>
                <Input
                  id="create-project-name"
                  aria-invalid={field.state.meta.errors.length > 0}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(event) => {
                    field.handleChange(event.target.value)
                  }}
                  placeholder={t('management.create.fields.name.placeholder')}
                />
                {field.state.meta.isTouched && field.state.meta.errors[0] ? (
                  <p className="text-xs text-destructive">
                    {field.state.meta.errors[0]}
                  </p>
                ) : null}
              </div>
            )}
          </form.Field>

          <form.Field
            name="abbreviation"
            validators={{
              onSubmit: ({ value }) => {
                if (value.trim().length === 0) {
                  return t('management.create.validation.required')
                }
                if (value.trim().length > 3) {
                  return t('management.create.validation.abbreviationMaxLength')
                }
                return undefined
              },
            }}
          >
            {(field) => (
              <div className="space-y-2">
                <Label htmlFor="create-project-abbreviation">
                  {t('management.create.fields.abbreviation.label')}
                </Label>
                <Input
                  id="create-project-abbreviation"
                  aria-invalid={field.state.meta.errors.length > 0}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(event) => {
                    field.handleChange(event.target.value)
                  }}
                  placeholder={t(
                    'management.create.fields.abbreviation.placeholder'
                  )}
                />
                {field.state.meta.isTouched && field.state.meta.errors[0] ? (
                  <p className="text-xs text-destructive">
                    {field.state.meta.errors[0]}
                  </p>
                ) : null}
                <p className="text-xs text-muted-foreground">
                  {t('management.create.fields.abbreviation.hint')}
                </p>
              </div>
            )}
          </form.Field>

          <form.Field name="privacyLevel">
            {(field) => (
              <div className="space-y-2">
                <Label htmlFor="create-project-privacy">
                  {t('management.create.fields.privacyLevel.label')}
                </Label>
                <Select
                  value={String(field.state.value)}
                  onValueChange={(value) => {
                    field.handleChange(Number(value) as ProjectPrivacy)
                  }}
                >
                  <SelectTrigger id="create-project-privacy">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value={String(ProjectPrivacy.Public)}>
                      {t('management.create.privacyOptions.public')}
                    </SelectItem>
                    <SelectItem value={String(ProjectPrivacy.Restricted)}>
                      {t('management.create.privacyOptions.restricted')}
                    </SelectItem>
                    <SelectItem value={String(ProjectPrivacy.Private)}>
                      {t('management.create.privacyOptions.private')}
                    </SelectItem>
                  </SelectContent>
                </Select>
              </div>
            )}
          </form.Field>

          <DialogFooter>
            <Button
              type="button"
              variant="ghost"
              disabled={isSubmitting}
              onClick={() => {
                onOpenChange(false)
              }}
            >
              {t('management.create.modal.cancel')}
            </Button>

            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting
                ? t('management.create.modal.submitting')
                : t('management.create.modal.submit')}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
