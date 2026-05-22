import DOMPurify from 'dompurify'
import { cn } from '@/shared/lib/utils'
import { richTextClass } from './styles'

interface RichTextContentProps {
  html: string | null
  emptyLabel?: string
  className?: string
}

export function RichTextContent({ html, emptyLabel = 'Sin descripción', className }: RichTextContentProps) {
  if (!html || html === '<p></p>') {
    return <p className={cn('text-sm text-muted-foreground italic', className)}>{emptyLabel}</p>
  }

  const sanitized = DOMPurify.sanitize(html)
  const fixed = sanitized.replace(
    /<span([^>]*data-type="mention"[^>]*data-label="([^"]*)"[^>]*)>[^<]*<\/span>/g,
    (_, attrs, label) => `<span${attrs}>@${label}</span>`
  )

  return (
    <div
      className={cn(richTextClass, className)}
      dangerouslySetInnerHTML={{ __html: fixed }}
    />
  )
}
