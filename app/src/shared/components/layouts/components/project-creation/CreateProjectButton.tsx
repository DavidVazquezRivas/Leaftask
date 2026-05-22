import { Plus } from 'lucide-react'
import { useState } from 'react'

import { useAppTranslation } from '@/core/i18n'
import { CreateProjectModal } from '@/shared/components/layouts/components/project-creation/CreateProjectModal'
import { Button } from '@/shared/components/ui/button'

interface CreateProjectButtonProps {
  organizationId: string | null
}

export function CreateProjectButton({
  organizationId,
}: CreateProjectButtonProps) {
  const { t } = useAppTranslation('projects')
  const [isOpen, setIsOpen] = useState(false)

  return (
    <>
      <Button
        className="w-full"
        variant="outline"
        data-icon="inline-start"
        aria-label={t('management.create.trigger')}
        onClick={() => {
          setIsOpen(true)
        }}
      >
        <Plus />
        {t('management.create.trigger')}
      </Button>

      <CreateProjectModal
        open={isOpen}
        onOpenChange={setIsOpen}
        organizationId={organizationId}
      />
    </>
  )
}
