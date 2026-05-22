import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Send, X } from 'lucide-react'
import { Button } from '@/shared/components/ui/button'
import { RichTextEditor } from '@/shared/components/ui/rich-text-editor'
import type { MentionUser } from '@/shared/components/ui/rich-text-editor'
import { ApiGateway } from '@/core/api/ApiGateway'

interface CommentInputProps {
  projectId: string
  itemId: string
  mentionUsers: MentionUser[]
  onSubmit: (content: string) => Promise<void>
  isSubmitting: boolean
}

export function CommentInput({ projectId, itemId, mentionUsers, onSubmit, isSubmitting }: CommentInputProps) {
  const { t } = useTranslation('workitems')
  const [expanded, setExpanded] = useState(false)
  const [content, setContent] = useState('')

  const handleSubmit = async () => {
    const trimmed = content.replace(/<[^>]*>/g, '').trim()
    if (!trimmed) return
    await onSubmit(content)
    setContent('')
    setExpanded(false)
  }

  const handleCancel = () => {
    setContent('')
    setExpanded(false)
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) {
      e.preventDefault()
      handleSubmit()
    }
    if (e.key === 'Escape') {
      handleCancel()
    }
  }

  if (!expanded) {
    return (
      <button
        type="button"
        onClick={() => setExpanded(true)}
        className="w-full text-left rounded-md border border-input bg-muted/30 px-3 py-2 text-sm text-muted-foreground hover:bg-muted/50 hover:border-input/80 transition-colors"
      >
        {t('comments.placeholder')}
      </button>
    )
  }

  return (
    <div className="space-y-2" onKeyDown={handleKeyDown}>
      <RichTextEditor
        value={content}
        onChange={setContent}
        placeholder={t('comments.placeholder')}
        disabled={isSubmitting}
        mentionUsers={mentionUsers}
        onImageUpload={(file) =>
          ApiGateway.workItem.attachments.presignAndUpload(projectId, itemId, file)
        }
        className="[&_.ProseMirror]:max-h-36 [&_.ProseMirror]:overflow-y-auto"
      />
      <div className="flex justify-end gap-1.5">
        <Button variant="ghost" size="sm" onClick={handleCancel} disabled={isSubmitting}>
          <X size={13} className="mr-1" />
          {t('comments.cancel')}
        </Button>
        <Button
          size="sm"
          onClick={handleSubmit}
          disabled={isSubmitting || !content.replace(/<[^>]*>/g, '').trim()}
        >
          <Send size={14} className="mr-1.5" />
          {t('comments.send')}
        </Button>
      </div>
    </div>
  )
}
