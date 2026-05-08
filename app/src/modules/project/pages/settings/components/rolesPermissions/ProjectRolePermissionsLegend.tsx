import { Check, ShieldAlert, X } from 'lucide-react'

interface ProjectRolePermissionsLegendProps {
  t: (key: string) => string
}

export function ProjectRolePermissionsLegend({
  t,
}: ProjectRolePermissionsLegendProps) {
  return (
    <section className="space-y-4 rounded-lg border bg-card p-6">
      <h3 className="text-sm font-semibold">
        {t('management.rolesPermissions.permissionLevels.title')}
      </h3>

      <div className="grid gap-3 text-sm md:grid-cols-3">
        <div className="flex items-start gap-2 text-muted-foreground">
          <Check className="mt-0.5 size-4 text-emerald-500" />
          <p>
            <span className="font-semibold text-foreground">
              {t('management.rolesPermissions.permissionLevels.full.label')}
            </span>{' '}
            {t(
              'management.rolesPermissions.permissionLevels.full.description'
            )}
          </p>
        </div>

        <div className="flex items-start gap-2 text-muted-foreground">
          <ShieldAlert className="mt-0.5 size-4 text-amber-500" />
          <p>
            <span className="font-semibold text-foreground">
              {t(
                'management.rolesPermissions.permissionLevels.supervised.label'
              )}
            </span>{' '}
            {t(
              'management.rolesPermissions.permissionLevels.supervised.description'
            )}
          </p>
        </div>

        <div className="flex items-start gap-2 text-muted-foreground">
          <X className="mt-0.5 size-4 text-slate-400" />
          <p>
            <span className="font-semibold text-foreground">
              {t('management.rolesPermissions.permissionLevels.none.label')}
            </span>{' '}
            {t(
              'management.rolesPermissions.permissionLevels.none.description'
            )}
          </p>
        </div>
      </div>
    </section>
  )
}
