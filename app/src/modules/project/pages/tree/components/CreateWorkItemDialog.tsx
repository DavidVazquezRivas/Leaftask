import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'

import type { WorkItemTypeData, WorkItemStatusData } from '@/core/api/workitems'
import type { ProjectMemberData } from '@/core/api/project/members'
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
import { Textarea } from '@/shared/components/ui/textarea'

interface CreateWorkItemDialogProps {
  open: boolean
  isSubmitting: boolean
  types: WorkItemTypeData[]
  statuses: WorkItemStatusData[]
  members: ProjectMemberData[]
  onClose: () => void
  onSubmit: (payload: {
    title: string
    description: string
    typeId: string
    statusId: string
    assigneeId: string | null
    estimation: number
  }) => void
}

export function CreateWorkItemDialog({
  open,
  isSubmitting,
  types,
  statuses,
  members,
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

  useEffect(() => {
    if (!open) return
    setTitle('')
    setDescription('')
    setTypeId(types[0]?.id ?? '')
    setStatusId(statuses[0]?.id ?? '')
    setAssigneeId(null)
    setEstimation(8)
  }, [open, types, statuses])

  const canSubmit =
    title.trim().length > 0 && typeId.length > 0 && statusId.length > 0 && estimation > 0

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!canSubmit) return
    onSubmit({
      title: title.trim(),
      description: description.trim(),
      typeId,
      statusId,
      assigneeId,
      estimation,
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
            <Label htmlFor="wi-title">{t('create.fields.titleLabel')}</Label>
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
            <Label htmlFor="wi-description">
              {t('create.fields.descriptionLabel')}{' '}
              <span className="text-xs font-normal text-muted-foreground">
                {t('create.fields.descriptionOptional')}
              </span>
            </Label>
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
            <Label htmlFor="wi-estimation">{t('create.fields.estimationLabel')}</Label>
            <Input
              id="wi-estimation"
              type="number"
              min={0.5}
              step={0.5}
              value={estimation}
              onChange={(e) => setEstimation(Number(e.target.value))}
              placeholder={t('create.fields.estimationPlaceholder')}
              disabled={isSubmitting}
            />
          </div>

          {/* Type + Status side by side */}
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-2">
              <Label htmlFor="wi-type">{t('create.fields.type')}</Label>
              <Select
                value={typeId}
                onValueChange={setTypeId}
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
              <Label htmlFor="wi-status">{t('create.fields.status')}</Label>
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
            <Label htmlFor="wi-assignee">
              {t('create.fields.assignee')}{' '}
              <span className="text-xs font-normal text-muted-foreground">
                {t('create.fields.assigneeOptional')}
              </span>
            </Label>
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
