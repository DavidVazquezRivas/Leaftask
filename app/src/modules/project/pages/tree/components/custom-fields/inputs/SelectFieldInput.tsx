import type { CustomFieldOptionData } from '@/core/api/project/customFields'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

interface SelectFieldInputProps {
  value: string
  onChange: (v: string) => void
  options: CustomFieldOptionData[]
  disabled?: boolean
}

export function SelectFieldInput({ value, onChange, options, disabled }: SelectFieldInputProps) {
  return (
    <Select value={value} onValueChange={onChange} disabled={disabled}>
      <SelectTrigger>
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        {options.map((opt) => (
          <SelectItem key={opt.id} value={opt.id}>
            {opt.name}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  )
}
