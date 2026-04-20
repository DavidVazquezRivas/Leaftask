import { Check, ShieldAlert, X } from 'lucide-react'

interface RolePermissionsLegendProps {
  t: (key: string) => string
}

export function RolePermissionsLegend({ t }: RolePermissionsLegendProps) {
  return (
    <section className="space-y-4 rounded-lg border bg-card p-6">
      <h3 className="text-sm font-semibold">
        {t('management.settings.rolesPermissions.permissionLevels.title')}
      </h3>

      <div className="grid gap-3 text-sm md:grid-cols-3">
        <div className="flex items-start gap-2 text-muted-foreground">
          <Check className="mt-0.5 size-4 text-emerald-500" />
          <p>
            <span className="font-semibold text-foreground">
              {t(
                'management.settings.rolesPermissions.permissionLevels.full.label'
              )}
            </span>{' '}
            {t(
              'management.settings.rolesPermissions.permissionLevels.full.description'
            )}
          </p>
        </div>

        <div className="flex items-start gap-2 text-muted-foreground">
          <ShieldAlert className="mt-0.5 size-4 text-amber-500" />
          <p>
            <span className="font-semibold text-foreground">
              {t(
                'management.settings.rolesPermissions.permissionLevels.supervised.label'
              )}
            </span>{' '}
            {t(
              'management.settings.rolesPermissions.permissionLevels.supervised.description'
            )}
          </p>
        </div>

        <div className="flex items-start gap-2 text-muted-foreground">
          <X className="mt-0.5 size-4 text-slate-400" />
          <p>
            <span className="font-semibold text-foreground">
              {t(
                'management.settings.rolesPermissions.permissionLevels.none.label'
              )}
            </span>{' '}
            {t(
              'management.settings.rolesPermissions.permissionLevels.none.description'
            )}
          </p>
        </div>
      </div>
    </section>
  )
}
