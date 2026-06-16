import { useRef, type KeyboardEvent } from 'react'
import { Send } from 'lucide-react'
import { Button } from '@/shared/components/ui/button'

interface MessageInputProps {
  onSend: (content: string) => void
  disabled?: boolean
  placeholder?: string
}

export function MessageInput({ onSend, disabled, placeholder = 'Escribe un mensaje...' }: MessageInputProps) {
  const ref = useRef<HTMLTextAreaElement>(null)

  const handleKeyDown = (e: KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      submit()
    }
  }

  const submit = () => {
    const value = ref.current?.value.trim()
    if (!value || disabled) return
    onSend(value)
    if (ref.current) ref.current.value = ''
  }

  return (
    <div className="flex items-end gap-2 border-t border-border p-3">
      <textarea
        ref={ref}
        rows={1}
        placeholder={placeholder}
        disabled={disabled}
        onKeyDown={handleKeyDown}
        className="max-h-32 min-h-[36px] flex-1 resize-none rounded-md bg-transparent px-2 py-1.5 text-sm outline-none placeholder:text-muted-foreground disabled:opacity-50"
        style={{ fieldSizing: 'content' } as React.CSSProperties}
      />
      <Button size="icon" variant="ghost" onClick={submit} disabled={disabled} className="shrink-0">
        <Send size={16} />
      </Button>
    </div>
  )
}
