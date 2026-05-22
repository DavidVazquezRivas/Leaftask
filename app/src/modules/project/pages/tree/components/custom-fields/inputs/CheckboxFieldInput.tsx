import { useTranslation } from 'react-i18next'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

interface CheckboxFieldInputProps {
  value: string
  onChange: (v: string) => void
  disabled?: boolean
}

export function CheckboxFieldInput({ value, onChange, disabled }: CheckboxFieldInputProps) {
  const { t } = useTranslation('workitems')
  return (
    <Select value={value || 'false'} onValueChange={onChange} disabled={disabled}>
      <SelectTrigger>
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="false">{t('fieldValues.no')}</SelectItem>
        <SelectItem value="true">{t('fieldValues.yes')}</SelectItem>
      </SelectContent>
    </Select>
  )
}
