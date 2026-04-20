import { useAppTranslation } from '@/core/i18n'
import {
  OrganizationSettingsDangerZone,
  OrganizationSettingsGeneralForm,
  OrganizationSettingsMembers,
  OrganizationSettingsRolesPermissions,
  type OrganizationSettingsTab,
  OrganizationSettingsTabs,
} from '@/modules/organization/pages/settings/components'
import { useOrganizationSettingsPage } from '@/modules/organization/pages/settings/hooks/useOrganizationSettingsPage'
import { useState } from 'react'

export function OrganizationSettingsPage() {
  const { t } = useAppTranslation('organizations')
  const [activeTab, setActiveTab] = useState<OrganizationSettingsTab>('general')
  const {
    detail,
    handleDelete,
    handleSubmit,
    isBusy,
    isDeleting,
    hasConfigureOrganizationPermission,
    hasInviteMembersPermission,
    hasRemoveMembersPermission,
    isConfigureOrganizationSupervised,
    isSubmitting,
    metrics,
    organizationId,
  } = useOrganizationSettingsPage()

  if (!organizationId) {
    return null
  }

  return (
    <main className="mx-auto flex w-full max-w-6xl flex-col gap-6">
      <header className="space-y-2">
        <h1 className="text-3xl font-semibold tracking-tight">
          {t('management.settings.general.title')}
        </h1>
        <p className="text-muted-foreground">
          {t('management.settings.general.subtitle')}
        </p>
      </header>

      <OrganizationSettingsTabs
        activeTab={activeTab}
        onTabChange={setActiveTab}
      />

      {activeTab === 'general' ? (
        <>
          <OrganizationSettingsGeneralForm
            detail={detail}
            hasConfigureOrganizationPermission={
              hasConfigureOrganizationPermission
            }
            isConfigureOrganizationSupervised={
              isConfigureOrganizationSupervised
            }
            isSubmitting={isSubmitting}
            isBusy={isBusy}
            metrics={metrics}
            onSubmit={handleSubmit}
          />

          <OrganizationSettingsDangerZone
            hasConfigureOrganizationPermission={
              hasConfigureOrganizationPermission
            }
            isDeleting={isDeleting}
            onDelete={handleDelete}
          />
        </>
      ) : activeTab === 'roles-permissions' ? (
        <OrganizationSettingsRolesPermissions
          organizationId={organizationId}
          canManageRoles={hasConfigureOrganizationPermission}
        />
      ) : (
        <OrganizationSettingsMembers
          organizationId={organizationId}
          canManageMemberRoles={hasConfigureOrganizationPermission}
          canInviteMembers={hasInviteMembersPermission}
          canRemoveMembers={hasRemoveMembersPermission}
        />
      )}
    </main>
  )
}
