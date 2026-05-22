import { Input } from '@/shared/components/ui/input'

interface LinkFieldInputProps {
  value: string
  onChange: (v: string) => void
  onBlur?: () => void
  disabled?: boolean
}

export function LinkFieldInput({ value, onChange, onBlur, disabled }: LinkFieldInputProps) {
  return (
    <Input
      type="url"
      value={value}
      onChange={(e) => onChange(e.target.value)}
      onBlur={onBlur}
      disabled={disabled}
    />
  )
}
