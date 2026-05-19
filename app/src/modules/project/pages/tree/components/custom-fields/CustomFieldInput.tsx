import type { CustomFieldData } from '@/core/api/project/customFields'
import type { ProjectMemberData } from '@/core/api/project/members'
import { CheckboxFieldInput } from './inputs/CheckboxFieldInput'
import { DateFieldInput } from './inputs/DateFieldInput'
import { LinkFieldInput } from './inputs/LinkFieldInput'
import { MultiSelectFieldInput } from './inputs/MultiSelectFieldInput'
import { NumberFieldInput } from './inputs/NumberFieldInput'
import { PersonFieldInput } from './inputs/PersonFieldInput'
import { SelectFieldInput } from './inputs/SelectFieldInput'
import { TextFieldInput } from './inputs/TextFieldInput'

interface CustomFieldInputProps {
  field: CustomFieldData
  fieldTypeName: string
  value: string
  onChange: (v: string) => void
  onBlur?: () => void
  members: ProjectMemberData[]
  disabled?: boolean
}

export function CustomFieldInput({
  field,
  fieldTypeName,
  value,
  onChange,
  onBlur,
  members,
  disabled,
}: CustomFieldInputProps) {
  const base = { value, onChange, onBlur, disabled }

  switch (fieldTypeName) {
    case 'Número':
      return <NumberFieldInput {...base} />
    case 'Fecha':
      return <DateFieldInput {...base} />
    case 'Enlace':
      return <LinkFieldInput {...base} />
    case 'Selección':
      return <SelectFieldInput {...base} options={field.options} />
    case 'Selección Múltiple':
      return <MultiSelectFieldInput {...base} options={field.options} />
    case 'Casilla de Verificación':
      return <CheckboxFieldInput {...base} />
    case 'Persona':
      return <PersonFieldInput {...base} members={members} />
    default:
      return <TextFieldInput {...base} />
  }
}
