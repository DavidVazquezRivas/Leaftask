import { Plus } from 'lucide-react'
import { useMemo, useState } from 'react'

import type {
  OrganizationRoleData,
  OrganizationRolePermissionLevel,
} from '@/core/api/organization/roles'
import { useAppTranslation } from '@/core/i18n'
import {
  useCreateOrganizationRoleMutation,
  useDeleteOrganizationRoleMutation,
  useOrganizationRolesPermissionsQuery,
  useOrganizationRolesQuery,
  useUpdateOrganizationRoleMutation,
} from '@/core/query/organization/roles'
import {
  OrganizationRoleCard,
  OrganizationRoleDeleteDialog,
  OrganizationRoleEditorDialog,
  RolePermissionsLegend,
} from '@/modules/organization/pages/settings/components/rolesPermissions'
import { getPermissionLevelForRole } from '@/modules/organization/pages/settings/utils/rolesPermissions.utils'
import { Button } from '@/shared/components/ui/button'

interface OrganizationSettingsRolesPermissionsProps {
  organizationId: string
  canManageRoles: boolean
}

type RoleEditorMode = 'create' | 'edit'

interface RoleEditorState {
  mode: RoleEditorMode
  role: OrganizationRoleData | null
}

export function OrganizationSettingsRolesPermissions({
  organizationId,
  canManageRoles,
}: OrganizationSettingsRolesPermissionsProps) {
  const { t } = useAppTranslation('organizations')
  const tr = (key: string, defaultValue: string) =>
    t(key, {
      defaultValue,
    })

  const permissionsQuery = useOrganizationRolesPermissionsQuery()
  const rolesQuery = useOrganizationRolesQuery(organizationId)
  const createRoleMutation = useCreateOrganizationRoleMutation(organizationId)
  const updateRoleMutation = useUpdateOrganizationRoleMutation(organizationId)
  const deleteRoleMutation = useDeleteOrganizationRoleMutation(organizationId)

  const [editorState, setEditorState] = useState<RoleEditorState | null>(null)
  const [roleToDelete, setRoleToDelete] = useState<OrganizationRoleData | null>(
    null
  )

  const catalogPermissions = useMemo(
    () => permissionsQuery.data?.data ?? [],
    [permissionsQuery.data?.data]
  )

  const roles = rolesQuery.data?.data ?? []

  const isMutatingRole =
    createRoleMutation.isPending ||
    updateRoleMutation.isPending ||
    deleteRoleMutation.isPending

  const handleInlinePermissionLevelChange = async (
    role: OrganizationRoleData,
    permissionId: string,
    nextLevel: OrganizationRolePermissionLevel
  ) => {
    if (!canManageRoles || isMutatingRole) {
      return
    }

    const nextPermissions = catalogPermissions.map((permission) => ({
      id: permission.id,
      level:
        permission.id === permissionId
          ? nextLevel
          : getPermissionLevelForRole(role, permission.id),
    }))

    try {
      await updateRoleMutation.mutateAsync({
        roleId: role.id,
        data: {
          name: role.name,
          permissions: nextPermissions,
        },
      })
    } catch {
      // Mutation-level error handling (toasts) is already centralized in the hook.
    }
  }

  return (
    <section className="space-y-6">
      <header className="flex flex-wrap items-start justify-between gap-4 rounded-lg border bg-card p-6">
        <div className="space-y-1">
          <h2 className="text-lg font-semibold tracking-tight">
            {t('management.settings.rolesPermissions.title')}
          </h2>
          <p className="text-sm text-muted-foreground">
            {t('management.settings.rolesPermissions.subtitle')}
          </p>
        </div>

        <Button
          type="button"
          data-icon="inline-start"
          disabled={!canManageRoles || isMutatingRole}
          onClick={() => {
            setEditorState({
              mode: 'create',
              role: null,
            })
          }}
        >
          <Plus />
          {t('management.settings.rolesPermissions.createRole')}
        </Button>
      </header>

      <RolePermissionsLegend t={t} />

      {permissionsQuery.isLoading || rolesQuery.isLoading ? (
        <p className="text-sm text-muted-foreground">
          {t('management.settings.rolesPermissions.states.loading')}
        </p>
      ) : null}

      {permissionsQuery.isError || rolesQuery.isError ? (
        <p className="text-sm text-destructive">
          {t('management.settings.rolesPermissions.states.error')}
        </p>
      ) : null}

      {!permissionsQuery.isLoading &&
      !rolesQuery.isLoading &&
      !permissionsQuery.isError &&
      !rolesQuery.isError &&
      roles.length === 0 ? (
        <p className="text-sm text-muted-foreground">
          {t('management.settings.rolesPermissions.states.emptyRoles')}
        </p>
      ) : null}

      {!permissionsQuery.isLoading &&
      !rolesQuery.isLoading &&
      !permissionsQuery.isError &&
      !rolesQuery.isError ? (
        <div className="space-y-4">
          {roles.map((role) => (
            <OrganizationRoleCard
              key={role.id}
              role={role}
              catalogPermissions={catalogPermissions}
              canManageRoles={canManageRoles}
              isUpdatingRole={isMutatingRole}
              t={t}
              onEditRole={(selectedRole) => {
                setEditorState({
                  mode: 'edit',
                  role: selectedRole,
                })
              }}
              onDeleteRole={(selectedRole) => {
                setRoleToDelete(selectedRole)
              }}
              onInlinePermissionLevelChange={(
                selectedRole,
                permissionId,
                level
              ) => {
                void handleInlinePermissionLevelChange(
                  selectedRole,
                  permissionId,
                  level
                )
              }}
            />
          ))}
        </div>
      ) : null}

      <OrganizationRoleEditorDialog
        open={Boolean(editorState)}
        mode={editorState?.mode ?? 'create'}
        role={editorState?.role ?? null}
        catalogPermissions={catalogPermissions}
        isSubmitting={
          createRoleMutation.isPending || updateRoleMutation.isPending
        }
        onClose={() => {
          setEditorState(null)
        }}
        onSubmit={async ({ roleId, name, permissions }) => {
          if (editorState?.mode === 'create') {
            await createRoleMutation.mutateAsync({
              name,
              permissions,
            })
            return
          }

          const targetRoleId = roleId ?? editorState?.role?.id
          if (!targetRoleId) {
            return
          }

          await updateRoleMutation.mutateAsync({
            roleId: targetRoleId,
            data: {
              name,
              permissions,
            },
          })
        }}
        t={t}
        tr={tr}
      />

      <OrganizationRoleDeleteDialog
        roleToDelete={roleToDelete}
        isDeleting={deleteRoleMutation.isPending}
        onClose={() => {
          setRoleToDelete(null)
        }}
        onConfirmDelete={async (roleId) => {
          await deleteRoleMutation.mutateAsync(roleId)
        }}
        t={t}
      />
    </section>
  )
}
