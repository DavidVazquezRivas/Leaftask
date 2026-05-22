import { Minus, Plus } from 'lucide-react'
import { cn } from '@/shared/lib/utils'
import { Button } from './button'
import { Input } from './input'

interface NumberInputProps {
  id?: string
  value: number
  onChange: (value: number) => void
  onBlur?: () => void
  min?: number
  max?: number
  step?: number
  disabled?: boolean
  className?: string
}

export function NumberInput({
  id,
  value,
  onChange,
  onBlur,
  min,
  max,
  step = 1,
  disabled,
  className,
}: NumberInputProps) {
  const decrement = () => {
    const next = Math.round((value - step) * 1e10) / 1e10
    onChange(min !== undefined ? Math.max(min, next) : next)
  }

  const increment = () => {
    const next = Math.round((value + step) * 1e10) / 1e10
    onChange(max !== undefined ? Math.min(max, next) : next)
  }

  return (
    <div className={cn('flex gap-2', className)}>
      <Button
        type="button"
        variant="outline"
        size="icon"
        onClick={decrement}
        disabled={disabled || (min !== undefined && value <= min)}
      >
        <Minus className="h-4 w-4" />
      </Button>
      <Input
        id={id}
        type="number"
        value={value}
        min={min}
        max={max}
        step={step}
        disabled={disabled}
        onBlur={onBlur}
        className="text-center [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
        onChange={(e) => {
          const v = parseFloat(e.target.value)
          if (!isNaN(v)) onChange(v)
        }}
      />
      <Button
        type="button"
        variant="outline"
        size="icon"
        onClick={increment}
        disabled={disabled || (max !== undefined && value >= max)}
      >
        <Plus className="h-4 w-4" />
      </Button>
    </div>
  )
}
