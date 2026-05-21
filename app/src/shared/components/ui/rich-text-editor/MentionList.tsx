import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react'
import type { SuggestionKeyDownProps } from '@tiptap/suggestion'
import { cn } from '@/shared/lib/utils'

export interface MentionUser {
  id: string
  name: string
}

interface MentionListProps {
  items: MentionUser[]
  command: (item: { id: string; label: string }) => void
}

export interface MentionListHandle {
  onKeyDown: (props: SuggestionKeyDownProps) => boolean
}

export const MentionList = forwardRef<MentionListHandle, MentionListProps>(
  ({ items, command }, ref) => {
    const [selectedIndex, setSelectedIndex] = useState(0)
    const listRef = useRef<HTMLDivElement>(null)

    useEffect(() => setSelectedIndex(0), [items])

    useImperativeHandle(ref, () => ({
      onKeyDown: ({ event }: SuggestionKeyDownProps) => {
        if (event.key === 'ArrowUp') {
          setSelectedIndex((i) => (i - 1 + Math.max(items.length, 1)) % Math.max(items.length, 1))
          return true
        }
        if (event.key === 'ArrowDown') {
          setSelectedIndex((i) => (i + 1) % Math.max(items.length, 1))
          return true
        }
        if (event.key === 'Enter') {
          selectItem(selectedIndex)
          return true
        }
        return false
      },
    }))

    const selectItem = (index: number) => {
      const item = items[index]
      if (item) command({ id: item.id, label: item.name })
    }

    if (!items.length) return null

    return (
      <div
        ref={listRef}
        className="z-50 min-w-[160px] max-w-[240px] rounded-md border border-border bg-popover p-1 shadow-md"
      >
        {items.map((item, index) => (
          <button
            key={item.id}
            type="button"
            onClick={() => selectItem(index)}
            className={cn(
              'flex w-full items-center gap-2 rounded px-2 py-1.5 text-sm text-left transition-colors',
              index === selectedIndex
                ? 'bg-accent text-accent-foreground'
                : 'text-popover-foreground hover:bg-accent/50'
            )}
          >
            <span className="flex h-5 w-5 shrink-0 items-center justify-center rounded-full bg-primary/20 text-[10px] font-semibold text-primary uppercase">
              {item.name.charAt(0)}
            </span>
            <span className="truncate">{item.name}</span>
          </button>
        ))}
      </div>
    )
  }
)

MentionList.displayName = 'MentionList'
