import { cn } from '@/shared/lib/utils'
import type { CustomFieldOptionData } from '@/core/api/project/customFields'

interface MultiSelectFieldInputProps {
  value: string
  onChange: (v: string) => void
  options: CustomFieldOptionData[]
  disabled?: boolean
}

export function MultiSelectFieldInput({ value, onChange, options, disabled }: MultiSelectFieldInputProps) {
  const selected = value ? value.split(',').filter(Boolean) : []

  const toggle = (optId: string) => {
    const next = selected.includes(optId)
      ? selected.filter((id) => id !== optId)
      : [...selected, optId]
    onChange(next.join(','))
  }

  return (
    <div className="flex flex-col gap-1 w-full">
      {options.map((opt) => {
        const isSelected = selected.includes(opt.id)
        return (
          <button
            key={opt.id}
            type="button"
            disabled={disabled}
            onClick={() => toggle(opt.id)}
            className={cn(
              'w-full flex items-center justify-between rounded-md border px-3 py-1 text-xs font-medium transition-colors',
              isSelected
                ? 'border-primary bg-primary text-primary-foreground'
                : 'border-border bg-muted/40 text-muted-foreground hover:border-primary/50 hover:text-foreground'
            )}
          >
            <span>{opt.name}</span>
            {isSelected && <span className="opacity-75">✓</span>}
          </button>
        )
      })}
    </div>
  )
}
