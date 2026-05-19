import { useEffect, useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'

import type { WorkItemTypeData, WorkItemStatusData } from '@/core/api/workitems'
import type { ProjectMemberData } from '@/core/api/project/members'
import type { CustomFieldData } from '@/core/api/project/customFields'
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
import { NumberInput } from '@/shared/components/ui/number-input'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'
import { Textarea } from '@/shared/components/ui/textarea'
import { CustomFieldInput } from './custom-fields'

interface CreateWorkItemDialogProps {
  open: boolean
  isSubmitting: boolean
  types: WorkItemTypeData[]
  statuses: WorkItemStatusData[]
  members: ProjectMemberData[]
  customFields: CustomFieldData[]
  fieldTypeNameById: Map<string, string>
  onClose: () => void
  onSubmit: (payload: {
    title: string
    description: string
    typeId: string
    statusId: string
    assigneeId: string | null
    estimation: number
    customFields: Record<string, string>
  }) => void
}

export function CreateWorkItemDialog({
  open,
  isSubmitting,
  types,
  statuses,
  members,
  customFields,
  fieldTypeNameById,
  onClose,
  onSubmit,
}: CreateWorkItemDialogProps) {
  const { t } = useTranslation('workitems')
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [typeId, setTypeId] = useState('')
  const [statusId, setStatusId] = useState('')
  const [assigneeId, setAssigneeId] = useState<string | null>(null)
  const [estimation, setEstimation] = useState(8)
  const [customFieldValues, setCustomFieldValues] = useState<Record<string, string>>({})

  useEffect(() => {
    if (!open) return
    setTitle('')
    setDescription('')
    setTypeId(types[0]?.id ?? '')
    setStatusId(statuses[0]?.id ?? '')
    setAssigneeId(null)
    setEstimation(8)
    setCustomFieldValues({})
  }, [open, types, statuses])

  const applicableFields = useMemo(
    () =>
      customFields.filter(
        (cf) =>
          cf.appliesTo.length === 0 ||
          cf.appliesTo.some((t) => t.id === typeId)
      ),
    [customFields, typeId]
  )

  // When type changes, clear values for fields that no longer apply
  const handleTypeChange = (newTypeId: string) => {
    setTypeId(newTypeId)
    const nextApplicable = new Set(
      customFields
        .filter(
          (cf) =>
            cf.appliesTo.length === 0 ||
            cf.appliesTo.some((t) => t.id === newTypeId)
        )
        .map((cf) => cf.id)
    )
    setCustomFieldValues((prev) => {
      const next: Record<string, string> = {}
      for (const [id, val] of Object.entries(prev)) {
        if (nextApplicable.has(id)) next[id] = val
      }
      return next
    })
  }

  const allRequiredFilled = applicableFields
    .filter((cf) => cf.required)
    .every((cf) => (customFieldValues[cf.id] ?? '').trim().length > 0)

  const canSubmit =
    title.trim().length > 0 &&
    typeId.length > 0 &&
    statusId.length > 0 &&
    estimation > 0 &&
    allRequiredFilled

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!canSubmit) return

    const fieldValues: Record<string, string> = {}
    for (const cf of applicableFields) {
      const val = customFieldValues[cf.id] ?? ''
      if (val.trim().length > 0) {
        fieldValues[cf.id] = val
      }
    }

    onSubmit({
      title: title.trim(),
      description: description.trim(),
      typeId,
      statusId,
      assigneeId,
      estimation,
      customFields: fieldValues,
    })
  }

  return (
    <Dialog
      open={open}
      onOpenChange={(next) => {
        if (!next && !isSubmitting) onClose()
      }}
    >
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>{t('create.title')}</DialogTitle>
          <DialogDescription>{t('create.description')}</DialogDescription>
        </DialogHeader>

        <form
          className="space-y-4"
          noValidate
          onSubmit={handleSubmit}
        >
          {/* Title */}
          <div className="space-y-2">
            <Label htmlFor="wi-title">
              {t('create.fields.titleLabel')}
              <span className="ml-1 text-destructive">*</span>
            </Label>
            <Input
              id="wi-title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder={t('create.fields.titlePlaceholder')}
              disabled={isSubmitting}
              autoFocus
            />
          </div>

          {/* Description */}
          <div className="space-y-2">
            <Label htmlFor="wi-description">{t('create.fields.descriptionLabel')}</Label>
            <Textarea
              id="wi-description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder={t('create.fields.descriptionPlaceholder')}
              disabled={isSubmitting}
              rows={3}
            />
          </div>

          {/* Estimation */}
          <div className="space-y-2">
            <Label htmlFor="wi-estimation">
              {t('create.fields.estimationLabel')}
              <span className="ml-1 text-destructive">*</span>
            </Label>
            <NumberInput
              id="wi-estimation"
              min={0.5}
              step={0.5}
              value={estimation}
              onChange={setEstimation}
              disabled={isSubmitting}
            />
          </div>

          {/* Type + Status side by side */}
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-2">
              <Label htmlFor="wi-type">
                {t('create.fields.type')}
                <span className="ml-1 text-destructive">*</span>
              </Label>
              <Select
                value={typeId}
                onValueChange={handleTypeChange}
                disabled={isSubmitting || types.length === 0}
              >
                <SelectTrigger id="wi-type">
                  <SelectValue placeholder={t('create.fields.typePlaceholder')} />
                </SelectTrigger>
                <SelectContent>
                  {types.map((type) => (
                    <SelectItem key={type.id} value={type.id}>
                      {type.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="wi-status">
                {t('create.fields.status')}
                <span className="ml-1 text-destructive">*</span>
              </Label>
              <Select
                value={statusId}
                onValueChange={setStatusId}
                disabled={isSubmitting || statuses.length === 0}
              >
                <SelectTrigger id="wi-status">
                  <SelectValue placeholder={t('create.fields.statusPlaceholder')} />
                </SelectTrigger>
                <SelectContent>
                  {statuses.map((status) => (
                    <SelectItem key={status.id} value={status.id}>
                      {status.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Assignee */}
          <div className="space-y-2">
            <Label htmlFor="wi-assignee">{t('create.fields.assignee')}</Label>
            <Select
              value={assigneeId ?? 'none'}
              onValueChange={(v) => setAssigneeId(v === 'none' ? null : v)}
              disabled={isSubmitting}
            >
              <SelectTrigger id="wi-assignee">
                <SelectValue placeholder={t('create.fields.assigneeUnassigned')} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="none">{t('create.fields.assigneeUnassigned')}</SelectItem>
                {members.map((member) => (
                  <SelectItem key={member.id} value={member.id}>
                    {member.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Custom fields */}
          {applicableFields.length > 0 && (
            <div className="space-y-3 rounded-lg border bg-muted/20 p-3">
              <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                {t('create.customFieldsSection')}
              </p>
              {applicableFields.map((cf) => {
                const typeName = fieldTypeNameById.get(cf.type) ?? ''
                return (
                  <div key={cf.id} className="space-y-1.5">
                    <Label className="text-xs">
                      {cf.name}
                      {cf.required && (
                        <span className="ml-1 text-destructive">*</span>
                      )}
                    </Label>
                    <CustomFieldInput
                      field={cf}
                      fieldTypeName={typeName}
                      value={customFieldValues[cf.id] ?? ''}
                      onChange={(v) =>
                        setCustomFieldValues((prev) => ({ ...prev, [cf.id]: v }))
                      }
                      members={members}
                      disabled={isSubmitting}
                    />
                  </div>
                )
              })}
            </div>
          )}

          <DialogFooter>
            <Button
              type="button"
              variant="ghost"
              onClick={onClose}
              disabled={isSubmitting}
            >
              {t('create.actions.cancel')}
            </Button>
            <Button type="submit" disabled={!canSubmit || isSubmitting}>
              {t('create.actions.submit')}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
