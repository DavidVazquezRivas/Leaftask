import { Calendar as CalendarIcon } from 'lucide-react'
import { Button } from '@/shared/components/ui/button'
import { Calendar } from '@/shared/components/ui/calendar'
import { Popover, PopoverContent, PopoverTrigger } from '@/shared/components/ui/popover'

interface DateFieldInputProps {
  value: string
  onChange: (v: string) => void
  onBlur?: () => void
  disabled?: boolean
}

function formatDate(iso: string): string {
  return new Date(iso + 'T12:00:00').toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

export function DateFieldInput({ value, onChange, onBlur, disabled }: DateFieldInputProps) {
  const handleSelect = (d: Date | undefined) => {
    if (!d) return
    const y = d.getFullYear()
    const m = String(d.getMonth() + 1).padStart(2, '0')
    const day = String(d.getDate()).padStart(2, '0')
    const iso = `${y}-${m}-${day}`
    onChange(iso)
    onBlur?.()
  }

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          type="button"
          variant="outline"
          className="w-full justify-start font-normal"
          disabled={disabled}
        >
          <CalendarIcon size={13} className="mr-2 shrink-0 opacity-60" />
          {value ? (
            formatDate(value)
          ) : (
            <span className="text-muted-foreground">—</span>
          )}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0" align="start">
        <Calendar
          mode="single"
          selected={value ? new Date(value + 'T12:00:00') : undefined}
          onSelect={handleSelect}
        />
      </PopoverContent>
    </Popover>
  )
}
