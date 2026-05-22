import { NumberInput } from '@/shared/components/ui/number-input'

interface NumberFieldInputProps {
  value: string
  onChange: (v: string) => void
  onBlur?: () => void
  disabled?: boolean
}

export function NumberFieldInput({ value, onChange, onBlur, disabled }: NumberFieldInputProps) {
  const numValue = parseFloat(value)
  return (
    <NumberInput
      value={isNaN(numValue) ? 0 : numValue}
      onChange={(v) => onChange(String(v))}
      onBlur={onBlur}
      step={1}
      disabled={disabled}
    />
  )
}
