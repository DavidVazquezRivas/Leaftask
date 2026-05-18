import { Pencil, Plus, Trash2, X } from 'lucide-react'
import { useMemo, useState } from 'react'
import { toast } from 'sonner'

import { useAppTranslation } from '@/core/i18n'
import { cn } from '@/shared/lib/utils'
import type { CustomFieldData } from '@/core/api/project/customFields'
import {
  useCreateProjectCustomFieldMutation,
  useDeleteProjectCustomFieldMutation,
  usePatchProjectCustomFieldMutation,
  useProjectCustomFieldsQuery,
  useProjectFieldTypesQuery,
} from '@/core/query/project'
import { useWorkItemTypesQuery } from '@/core/query/workitems'
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

interface ProjectSettingsCustomFieldsProps {
  projectId: string
  canManage: boolean
}

type DialogMode =
  | { mode: 'create' }
  | { mode: 'edit'; field: CustomFieldData }
  | null

const isSelectionType = (typeName: string) =>
  typeName.toLowerCase().includes('selec')

export function ProjectSettingsCustomFields({
  projectId,
  canManage,
}: ProjectSettingsCustomFieldsProps) {
  const { t } = useAppTranslation('projects')
  const tr = (key: string, fallback: string) => t(key, { defaultValue: fallback })

  const [dialogMode, setDialogMode] = useState<DialogMode>(null)
  const [formName, setFormName] = useState('')
  const [formTypeId, setFormTypeId] = useState('')
  const [formRequired, setFormRequired] = useState(false)
  const [formOptions, setFormOptions] = useState<string[]>([])
  const [formAppliesTo, setFormAppliesTo] = useState<string[]>([])
  const [newOption, setNewOption] = useState('')

  const fieldsQuery = useProjectCustomFieldsQuery(projectId)
  const fieldTypesQuery = useProjectFieldTypesQuery()
  const workItemTypesQuery = useWorkItemTypesQuery()
  const createMutation = useCreateProjectCustomFieldMutation(projectId)
  const patchMutation = usePatchProjectCustomFieldMutation(projectId)
  const deleteMutation = useDeleteProjectCustomFieldMutation(projectId)

  const fields = fieldsQuery.data?.data ?? []
  const fieldTypes = fieldTypesQuery.data?.data ?? []
  const workItemTypes = workItemTypesQuery.data?.data ?? []

  const fieldTypeById = useMemo(
    () => new Map(fieldTypes.map((ft) => [ft.id, ft])),
    [fieldTypes]
  )

  const selectedFieldType = fieldTypeById.get(formTypeId)
  const showOptions = Boolean(
    selectedFieldType && isSelectionType(selectedFieldType.name)
  )

  const isMutating =
    createMutation.isPending ||
    patchMutation.isPending ||
    deleteMutation.isPending

  const isDialogOpen = dialogMode !== null
  const isEditMode = dialogMode?.mode === 'edit'

  const openCreate = () => {
    const firstTypeId = fieldTypes[0]?.id ?? ''
    setFormName('')
    setFormTypeId(firstTypeId)
    setFormRequired(false)
    setFormOptions([])
    setFormAppliesTo([])
    setNewOption('')
    setDialogMode({ mode: 'create' })
  }

  const openEdit = (field: CustomFieldData) => {
    setFormName(field.name)
    setFormTypeId(field.type)
    setFormRequired(field.required)
    setFormOptions(field.options.map((o) => o.name))
    setFormAppliesTo(field.appliesTo.map((t) => t.id))
    setNewOption('')
    setDialogMode({ mode: 'edit', field })
  }

  const toggleAppliesTo = (typeId: string) => {
    setFormAppliesTo((prev) =>
      prev.includes(typeId) ? prev.filter((id) => id !== typeId) : [...prev, typeId]
    )
  }

  const closeDialog = () => {
    if (!isMutating) setDialogMode(null)
  }

  const handleTypeChange = (typeId: string) => {
    setFormTypeId(typeId)
    const type = fieldTypeById.get(typeId)
    if (!type || !isSelectionType(type.name)) {
      setFormOptions([])
    }
  }

  const handleAddOption = () => {
    const trimmed = newOption.trim()
    if (!trimmed) return
    if (formOptions.includes(trimmed)) {
      toast.error(trimmed)
      return
    }
    setFormOptions((prev) => [...prev, trimmed])
    setNewOption('')
  }

  const handleRemoveOption = (index: number) => {
    setFormOptions((prev) => prev.filter((_, i) => i !== index))
  }

  const validateForm = (): string | null => {
    if (!formName.trim()) {
      return tr(
        'management.customFields.validation.nameRequired',
        'Name is required'
      )
    }
    if (!formTypeId) {
      return tr(
        'management.customFields.validation.typeRequired',
        'Field type is required'
      )
    }
    if (showOptions && formOptions.length === 0) {
      return tr(
        'management.customFields.validation.optionsRequired',
        'At least one option is required for selection fields'
      )
    }
    return null
  }

  const handleSubmit = async () => {
    const validationError = validateForm()
    if (validationError) {
      toast.error(validationError)
      return
    }

    const options = showOptions ? formOptions : []

    try {
      if (dialogMode?.mode === 'create') {
        await createMutation.mutateAsync({
          name: formName.trim(),
          type: formTypeId,
          options,
          required: formRequired,
          appliesTo: formAppliesTo,
        })
      } else if (dialogMode?.mode === 'edit') {
        await patchMutation.mutateAsync({
          fieldId: dialogMode.field.id,
          data: {
            name: formName.trim(),
            type: formTypeId,
            options,
            required: formRequired,
            appliesTo: formAppliesTo,
          },
        })
      }

      setDialogMode(null)
    } catch {
      // Error already handled in mutation's onError
    }
  }

  const isLoading = fieldsQuery.isLoading || fieldTypesQuery.isLoading
  const isError = fieldsQuery.isError

  return (
    <section className="space-y-6">
      <header className="flex flex-wrap items-start justify-between gap-4 rounded-lg border bg-card p-6">
        <div className="space-y-1">
          <h2 className="text-lg font-semibold tracking-tight">
            {t('management.customFields.title')}
          </h2>
          <p className="text-sm text-muted-foreground">
            {t('management.customFields.subtitle')}
          </p>
        </div>

        <Button
          type="button"
          data-icon="inline-start"
          disabled={!canManage || fieldTypes.length === 0}
          title={
            !canManage
              ? t('management.customFields.permissions.noManage')
              : undefined
          }
          onClick={openCreate}
        >
          <Plus />
          {t('management.customFields.addField')}
        </Button>
      </header>

      {isLoading ? (
        <p className="text-sm text-muted-foreground">
          {t('management.customFields.states.loading')}
        </p>
      ) : null}

      {isError ? (
        <p className="text-sm text-destructive">
          {t('management.customFields.states.error')}
        </p>
      ) : null}

      {!isLoading && !isError && fields.length === 0 ? (
        <p className="text-sm text-muted-foreground">
          {t('management.customFields.states.empty')}
        </p>
      ) : null}

      {!isLoading && !isError && fields.length > 0 ? (
        <div className="space-y-3">
          {fields.map((field) => {
            const typeName =
              fieldTypeById.get(field.type)?.name ??
              t('management.customFields.field.typeFallback')

            return (
              <article
                key={field.id}
                className="rounded-lg border bg-card p-4"
              >
                <div className="flex items-start justify-between gap-3">
                  <div className="space-y-1">
                    <div className="flex flex-wrap items-center gap-2">
                      <p className="text-sm font-semibold">{field.name}</p>
                      {field.required ? (
                        <span className="inline-flex rounded-full border border-destructive/30 bg-destructive/10 px-2 py-0.5 text-xs font-medium text-destructive">
                          {tr('management.customFields.field.required', 'Required')}
                        </span>
                      ) : null}
                    </div>
                    <p className="text-xs text-muted-foreground">{typeName}</p>
                  </div>

                  <div className="flex shrink-0 items-center gap-1">
                    <Button
                      type="button"
                      size="icon-sm"
                      variant="ghost"
                      disabled={!canManage || isMutating}
                      onClick={() => { openEdit(field) }}
                      title={tr(
                        'management.customFields.actions.editField',
                        'Edit field'
                      )}
                    >
                      <Pencil className="size-4" />
                    </Button>

                    <Button
                      type="button"
                      size="icon-sm"
                      variant="ghost"
                      disabled={!canManage || isMutating}
                      onClick={() => { deleteMutation.mutate(field.id) }}
                      title={tr(
                        'management.customFields.actions.deleteField',
                        'Delete field'
                      )}
                    >
                      <Trash2 className="size-4" />
                    </Button>
                  </div>
                </div>

                {field.options.length > 0 ? (
                  <div className="mt-3 space-y-1.5">
                    <p className="text-xs font-medium text-muted-foreground">
                      {t('management.customFields.field.options')}
                    </p>
                    <div className="flex flex-wrap gap-1.5">
                      {field.options.map((option) => (
                        <span
                          key={option.id}
                          className="inline-flex rounded-md border bg-muted/50 px-2 py-0.5 text-xs"
                        >
                          {option.name}
                        </span>
                      ))}
                    </div>
                  </div>
                ) : null}

                <div className="mt-3 space-y-1.5">
                  <p className="text-xs font-medium text-muted-foreground">
                    {t('management.customFields.field.appliesTo')}
                  </p>
                  <div className="flex flex-wrap gap-1.5">
                    {field.appliesTo.length === 0 ? (
                      <span className="inline-flex rounded-full border bg-muted/50 px-2.5 py-0.5 text-xs text-muted-foreground">
                        {tr('management.customFields.dialog.appliesToAll', 'All types')}
                      </span>
                    ) : (
                      field.appliesTo.map((wt) => (
                        <span
                          key={wt.id}
                          className="inline-flex rounded-full border border-primary/30 bg-primary/10 px-2.5 py-0.5 text-xs font-medium text-primary"
                        >
                          {wt.name}
                        </span>
                      ))
                    )}
                  </div>
                </div>
              </article>
            )
          })}
        </div>
      ) : null}

      {!isLoading && fieldTypes.length > 0 ? (
        <section className="rounded-lg border bg-card p-6">
          <h3 className="mb-4 text-sm font-semibold">
            {t('management.customFields.fieldTypesSection.title')}
          </h3>
          <div className="grid grid-cols-1 gap-x-8 gap-y-3 sm:grid-cols-2">
            {fieldTypes.map((ft) => (
              <div key={ft.id}>
                <p className="text-sm font-medium">{ft.name}</p>
                <p className="text-xs text-muted-foreground">{ft.description}</p>
              </div>
            ))}
          </div>
        </section>
      ) : null}

      <Dialog open={isDialogOpen} onOpenChange={closeDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {isEditMode
                ? tr(
                    'management.customFields.dialog.editTitle',
                    'Edit Custom Field'
                  )
                : tr(
                    'management.customFields.dialog.createTitle',
                    'Add Custom Field'
                  )}
            </DialogTitle>
            <DialogDescription>
              {tr(
                'management.customFields.dialog.description',
                'Define the field name, type, and options.'
              )}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="cf-name">
                {tr('management.customFields.dialog.nameLabel', 'Name')}
              </Label>
              <Input
                id="cf-name"
                value={formName}
                onChange={(e) => { setFormName(e.target.value) }}
                placeholder={tr(
                  'management.customFields.dialog.namePlaceholder',
                  'Field name'
                )}
                disabled={isMutating}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="cf-type">
                {tr('management.customFields.dialog.typeLabel', 'Field Type')}
              </Label>
              <Select
                value={formTypeId}
                onValueChange={handleTypeChange}
                disabled={isMutating || fieldTypes.length === 0}
              >
                <SelectTrigger id="cf-type">
                  <SelectValue
                    placeholder={tr(
                      'management.customFields.dialog.typePlaceholder',
                      'Select a type'
                    )}
                  />
                </SelectTrigger>
                <SelectContent>
                  {fieldTypes.map((ft) => (
                    <SelectItem key={ft.id} value={ft.id}>
                      {ft.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="flex items-center gap-3">
              <input
                id="cf-required"
                type="checkbox"
                className="size-4 rounded border-input accent-primary"
                checked={formRequired}
                disabled={isMutating}
                onChange={(e) => { setFormRequired(e.target.checked) }}
              />
              <div>
                <Label htmlFor="cf-required">
                  {tr(
                    'management.customFields.dialog.requiredLabel',
                    'Required field'
                  )}
                </Label>
                <p className="text-xs text-muted-foreground">
                  {tr(
                    'management.customFields.dialog.requiredHint',
                    'Work items must fill this field'
                  )}
                </p>
              </div>
            </div>

            {workItemTypes.length > 0 ? (
              <div className="space-y-2">
                <div>
                  <Label>{t('management.customFields.dialog.appliesToLabel')}</Label>
                  <p className="mt-0.5 text-xs text-muted-foreground">
                    {t('management.customFields.dialog.appliesToHint')}
                  </p>
                </div>
                <div className="flex flex-wrap gap-2">
                  {workItemTypes.map((wt) => {
                    const selected = formAppliesTo.includes(wt.id)
                    return (
                      <button
                        key={wt.id}
                        type="button"
                        disabled={isMutating}
                        onClick={() => { toggleAppliesTo(wt.id) }}
                        className={cn(
                          'inline-flex items-center rounded-full border px-3 py-1 text-xs font-medium transition-colors',
                          selected
                            ? 'border-primary bg-primary text-primary-foreground'
                            : 'border-border bg-muted/40 text-muted-foreground hover:border-primary/50 hover:text-foreground'
                        )}
                      >
                        {wt.name}
                      </button>
                    )
                  })}
                </div>
              </div>
            ) : null}

            {showOptions ? (
              <div className="space-y-2">
                <Label>
                  {tr(
                    'management.customFields.dialog.optionsLabel',
                    'Options'
                  )}
                </Label>

                {formOptions.length > 0 ? (
                  <div className="flex flex-wrap gap-1.5">
                    {formOptions.map((option, index) => (
                      <span
                        key={index}
                        className="inline-flex items-center gap-1 rounded-md border bg-muted/50 px-2 py-0.5 text-xs"
                      >
                        {option}
                        <button
                          type="button"
                          onClick={() => { handleRemoveOption(index) }}
                          disabled={isMutating}
                          aria-label={`Remove ${option}`}
                          className="ml-0.5 rounded text-muted-foreground hover:text-foreground"
                        >
                          <X className="size-3" />
                        </button>
                      </span>
                    ))}
                  </div>
                ) : null}

                <div className="flex gap-2">
                  <Input
                    value={newOption}
                    onChange={(e) => { setNewOption(e.target.value) }}
                    placeholder={tr(
                      'management.customFields.dialog.optionPlaceholder',
                      'Option name'
                    )}
                    disabled={isMutating}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') {
                        e.preventDefault()
                        handleAddOption()
                      }
                    }}
                  />
                  <Button
                    type="button"
                    variant="outline"
                    disabled={isMutating || !newOption.trim()}
                    onClick={handleAddOption}
                  >
                    {tr('management.customFields.dialog.addOption', 'Add')}
                  </Button>
                </div>
              </div>
            ) : null}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="ghost"
              disabled={isMutating}
              onClick={closeDialog}
            >
              {tr('management.customFields.dialog.cancel', 'Cancel')}
            </Button>

            <Button
              type="button"
              disabled={isMutating || !formName.trim() || !formTypeId}
              onClick={() => { void handleSubmit() }}
            >
              {isMutating
                ? tr('management.customFields.dialog.submitting', 'Saving...')
                : isEditMode
                  ? tr(
                      'management.customFields.dialog.updateSubmit',
                      'Save changes'
                    )
                  : tr(
                      'management.customFields.dialog.createSubmit',
                      'Add field'
                    )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </section>
  )
}
