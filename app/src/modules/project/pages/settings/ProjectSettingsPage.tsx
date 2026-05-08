import { useState } from 'react'

import {
  ProjectSettingsDangerZone,
  ProjectSettingsGeneralForm,
  ProjectSettingsMembers,
  ProjectSettingsRolesPermissions,
  type ProjectSettingsTab,
  ProjectSettingsTabs,
} from '@/modules/project/pages/settings/components'
import { useProjectSettingsPage } from '@/modules/project/pages/settings/hooks/useProjectSettingsPage'

export function ProjectSettingsPage() {
  const [activeTab, setActiveTab] = useState<ProjectSettingsTab>('general')
  const {
    detail,
    projectId,
    handleDelete,
    handleSubmit,
    isBusy,
    isDeleting,
    isSubmitting,
    canUpdate,
    canDelete,
    t,
  } = useProjectSettingsPage()

  if (!projectId) {
    return null
  }

  return (
    <main className="mx-auto flex w-full max-w-6xl flex-col gap-6">
      <header className="space-y-2">
        <h1 className="text-3xl font-semibold tracking-tight">
          {t('management.settings.title')}
        </h1>
        <p className="text-muted-foreground">
          {t('management.settings.subtitle')}
        </p>
      </header>

      <ProjectSettingsTabs activeTab={activeTab} onTabChange={setActiveTab} />

      {activeTab === 'general' && (
        <>
          <ProjectSettingsGeneralForm
            detail={detail}
            canUpdate={canUpdate}
            isSubmitting={isSubmitting}
            isBusy={isBusy}
            onSubmit={handleSubmit}
            t={t}
          />

          <ProjectSettingsDangerZone
            canDelete={canDelete}
            isDeleting={isDeleting}
            onDelete={handleDelete}
            t={t}
          />
        </>
      )}

      {activeTab === 'roles-permissions' && (
        <ProjectSettingsRolesPermissions
          projectId={projectId}
          canManageRoles={canUpdate}
        />
      )}

      {activeTab === 'members' && (
        <ProjectSettingsMembers
          projectId={projectId}
          canManageMembers={canUpdate}
        />
      )}
    </main>
  )
}
