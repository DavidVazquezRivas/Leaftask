import { Input } from '@/shared/components/ui/input'

interface TextFieldInputProps {
  value: string
  onChange: (v: string) => void
  onBlur?: () => void
  disabled?: boolean
}

export function TextFieldInput({ value, onChange, onBlur, disabled }: TextFieldInputProps) {
  return (
    <Input
      value={value}
      onChange={(e) => onChange(e.target.value)}
      onBlur={onBlur}
      disabled={disabled}
    />
  )
}
