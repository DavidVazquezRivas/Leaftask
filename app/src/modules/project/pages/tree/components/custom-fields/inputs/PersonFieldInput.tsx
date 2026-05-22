import { useTranslation } from 'react-i18next'
import type { ProjectMemberData } from '@/core/api/project/members'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

interface PersonFieldInputProps {
  value: string
  onChange: (v: string) => void
  members: ProjectMemberData[]
  disabled?: boolean
}

export function PersonFieldInput({ value, onChange, members, disabled }: PersonFieldInputProps) {
  const { t } = useTranslation('workitems')
  return (
    <Select
      value={value || 'none'}
      onValueChange={(v) => onChange(v === 'none' ? '' : v)}
      disabled={disabled}
    >
      <SelectTrigger>
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="none">{t('fieldValues.none')}</SelectItem>
        {members.map((m) => (
          <SelectItem key={m.id} value={m.id}>
            {m.name}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  )
}
