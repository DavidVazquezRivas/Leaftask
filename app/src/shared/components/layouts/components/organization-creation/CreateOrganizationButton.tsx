import { Plus } from 'lucide-react'
import { useState } from 'react'

import { useAppTranslation } from '@/core/i18n'
import { CreateOrganizationModal } from '@/shared/components/layouts/components/organization-creation'
import { Button } from '@/shared/components/ui/button'

export function CreateOrganizationButton() {
  const { t } = useAppTranslation('organizations')
  const [isOpen, setIsOpen] = useState(false)

  return (
    <>
      <Button
        size="icon-sm"
        variant="outline"
        aria-label={t('management.create.trigger')}
        title={t('management.create.trigger')}
        onClick={() => {
          setIsOpen(true)
        }}
      >
        <Plus />
      </Button>

      <CreateOrganizationModal open={isOpen} onOpenChange={setIsOpen} />
    </>
  )
}
