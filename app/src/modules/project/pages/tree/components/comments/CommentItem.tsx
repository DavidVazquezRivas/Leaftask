import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Pencil, Trash2, Check, X } from 'lucide-react'
import { Button } from '@/shared/components/ui/button'
import { RichTextEditor, RichTextContent } from '@/shared/components/ui/rich-text-editor'
import type { MentionUser } from '@/shared/components/ui/rich-text-editor'
import type { WorkItemCommentData } from '@/core/api/workitems'
import { formatRelative } from '@/shared/lib/date'
import { getInitials } from '@/shared/lib/text'
import { ApiGateway } from '@/core/api/ApiGateway'

interface CommentItemProps {
  comment: WorkItemCommentData
  projectId: string
  itemId: string
  currentUserId: string
  mentionUsers: MentionUser[]
  onUpdate: (commentId: string, content: string) => Promise<void>
  onDelete: (commentId: string) => Promise<void>
  locale: string
}

export function CommentItem({
  comment,
  projectId,
  itemId,
  currentUserId,
  mentionUsers,
  onUpdate,
  onDelete,
  locale,
}: CommentItemProps) {
  const { t } = useTranslation('workitems')
  const [editing, setEditing] = useState(false)
  const [draft, setDraft] = useState(comment.content)
  const [isPending, setIsPending] = useState(false)

  const isOwner = comment.author.id === currentUserId

  const handleSave = async () => {
    const trimmed = draft.replace(/<[^>]*>/g, '').trim()
    if (!trimmed) return
    setIsPending(true)
    try {
      await onUpdate(comment.id, draft)
      setEditing(false)
    } finally {
      setIsPending(false)
    }
  }

  const handleDelete = async () => {
    setIsPending(true)
    try {
      await onDelete(comment.id)
    } finally {
      setIsPending(false)
    }
  }

  const handleCancel = () => {
    setDraft(comment.content)
    setEditing(false)
  }

  return (
    <div className="flex gap-3 group">
      <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-muted text-xs font-semibold text-muted-foreground uppercase">
        {getInitials(comment.author.fullName)}
      </span>
      <div className="flex-1 min-w-0 space-y-1">
        <div className="flex items-center gap-2">
          <span className="text-xs font-semibold text-foreground">{comment.author.fullName}</span>
          <span className="text-xs text-muted-foreground">{formatRelative(comment.createdAt, locale)}</span>
          {isOwner && !editing && (
            <div className="ml-auto flex items-center gap-0.5 opacity-0 group-hover:opacity-100 transition-opacity">
              <Button
                variant="ghost"
                size="icon"
                className="h-5 w-5"
                onClick={() => setEditing(true)}
                disabled={isPending}
              >
                <Pencil size={11} />
              </Button>
              <Button
                variant="ghost"
                size="icon"
                className="h-5 w-5 text-destructive hover:text-destructive"
                onClick={handleDelete}
                disabled={isPending}
              >
                <Trash2 size={11} />
              </Button>
            </div>
          )}
        </div>

        {editing ? (
          <div className="space-y-1.5">
            <RichTextEditor
              value={draft}
              onChange={setDraft}
              disabled={isPending}
              mentionUsers={mentionUsers}
              onImageUpload={(file) =>
                ApiGateway.workItem.attachments.presignAndUpload(projectId, itemId, file)
              }
            />
            <div className="flex gap-1.5 justify-end">
              <Button variant="ghost" size="sm" onClick={handleCancel} disabled={isPending}>
                <X size={13} className="mr-1" />
                {t('comments.cancel')}
              </Button>
              <Button size="sm" onClick={handleSave} disabled={isPending || !draft.replace(/<[^>]*>/g, '').trim()}>
                <Check size={13} className="mr-1" />
                {t('comments.save')}
              </Button>
            </div>
          </div>
        ) : (
          <div className="rounded-md border border-border bg-muted/20 px-3 py-2">
            <RichTextContent html={comment.content} />
          </div>
        )}
      </div>
    </div>
  )
}
