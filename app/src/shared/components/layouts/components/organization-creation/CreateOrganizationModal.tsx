import { useForm } from '@tanstack/react-form'
import { useEffect } from 'react'

import { useAppTranslation } from '@/core/i18n'
import { useCreateOrganizationManagementMutation } from '@/core/query/organization/management'
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
import { Textarea } from '@/shared/components/ui/textarea'

interface CreateOrganizationModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

type CreateOrganizationFormValues = {
  name: string
  description: string
  website: string
}

export function CreateOrganizationModal({
  open,
  onOpenChange,
}: CreateOrganizationModalProps) {
  const { t } = useAppTranslation('organizations')
  const createOrganizationMutation = useCreateOrganizationManagementMutation()
  const isSubmitting = createOrganizationMutation.isPending

  const form = useForm({
    defaultValues: {
      name: '',
      description: '',
      website: '',
    } as CreateOrganizationFormValues,
    onSubmit: async ({ value }) => {
      await createOrganizationMutation.mutateAsync(value)
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
        if (isSubmitting) {
          return
        }

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
              onSubmit: ({ value }) => {
                return value.trim().length > 0
                  ? undefined
                  : t('management.create.validation.required')
              },
            }}
          >
            {(field) => (
              <div className="space-y-2">
                <Label htmlFor="organization-name">
                  {t('management.create.fields.name.label')}
                </Label>
                <Input
                  id="organization-name"
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
            name="description"
            validators={{
              onSubmit: ({ value }) => {
                return value.trim().length > 0
                  ? undefined
                  : t('management.create.validation.required')
              },
            }}
          >
            {(field) => (
              <div className="space-y-2">
                <Label htmlFor="organization-description">
                  {t('management.create.fields.description.label')}
                </Label>
                <Textarea
                  id="organization-description"
                  aria-invalid={field.state.meta.errors.length > 0}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(event) => {
                    field.handleChange(event.target.value)
                  }}
                  placeholder={t(
                    'management.create.fields.description.placeholder'
                  )}
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
            name="website"
            validators={{
              onSubmit: ({ value }) => {
                return value.trim().length > 0
                  ? undefined
                  : t('management.create.validation.required')
              },
            }}
          >
            {(field) => (
              <div className="space-y-2">
                <Label htmlFor="organization-website">
                  {t('management.create.fields.website.label')}
                </Label>
                <Input
                  id="organization-website"
                  aria-invalid={field.state.meta.errors.length > 0}
                  value={field.state.value}
                  onBlur={field.handleBlur}
                  onChange={(event) => {
                    field.handleChange(event.target.value)
                  }}
                  placeholder={t(
                    'management.create.fields.website.placeholder'
                  )}
                />
                {field.state.meta.isTouched && field.state.meta.errors[0] ? (
                  <p className="text-xs text-destructive">
                    {field.state.meta.errors[0]}
                  </p>
                ) : null}
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
