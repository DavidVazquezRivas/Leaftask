import { useMemo, useState } from 'react'
import { Bell, CheckSquare, Loader2 } from 'lucide-react'
import { useNotificationsQuery, useApprovalsQuery, useMarkAllNotificationsAsReadMutation } from '@/core/query/notification'
import { useOrganizationManagementInfiniteQuery } from '@/core/query/organization/management'
import { useMyProjectsQuery } from '@/core/query/project/management'
import { Button } from '@/shared/components/ui/button'
import { cn } from '@/shared/lib/utils'
import { NotificationItem } from './components/NotificationItem'
import { ApprovalItem } from './components/ApprovalItem'

type Tab = 'notifications' | 'approvals'

export function NotificationsPage() {
  const [activeTab, setActiveTab] = useState<Tab>('notifications')

  const markAllAsReadMutation = useMarkAllNotificationsAsReadMutation()
  const notificationsQuery = useNotificationsQuery()
  const approvalsQuery = useApprovalsQuery()
  const orgsQuery = useOrganizationManagementInfiniteQuery()
  const projectsQuery = useMyProjectsQuery()

  const notifications = (notificationsQuery.data?.pages ?? []).flatMap((p) => p.data)
  const approvals = (approvalsQuery.data?.pages ?? []).flatMap((p) => p.data)

  const orgMap = useMemo(
    () => Object.fromEntries((orgsQuery.data?.data ?? []).map((o) => [o.id, o.name])),
    [orgsQuery.data]
  )
  const projectMap = useMemo(
    () => Object.fromEntries((projectsQuery.data?.data ?? []).map((p) => [p.id, p.name])),
    [projectsQuery.data]
  )

  const tabs: { id: Tab; label: string; icon: React.ReactNode }[] = [
    { id: 'notifications', label: 'Notificaciones', icon: <Bell className="size-4" /> },
    { id: 'approvals', label: 'Solicitudes de aprobación', icon: <CheckSquare className="size-4" /> },
  ]

  return (
    <div className="mx-auto max-w-2xl">
      <div className="flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Notificaciones</h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Revisa tus notificaciones y solicitudes pendientes
          </p>
        </div>
        {activeTab === 'notifications' && notifications.some((n) => !n.read) && (
          <Button
            variant="ghost"
            size="sm"
            onClick={() => markAllAsReadMutation.mutate()}
            disabled={markAllAsReadMutation.isPending}
          >
            Marcar todo como leído
          </Button>
        )}
      </div>

      <div className="py-10">
      <div className="flex gap-1 rounded-lg border bg-card p-1">
        {tabs.map((tab) => (
          <button
            key={tab.id}
            type="button"
            className={cn(
              'flex flex-1 items-center justify-center gap-2 rounded-md px-4 py-2 text-sm font-medium transition-colors',
              activeTab === tab.id
                ? 'bg-background text-foreground shadow-sm'
                : 'text-muted-foreground hover:text-foreground'
            )}
            onClick={() => setActiveTab(tab.id)}
          >
            {tab.icon}
            {tab.label}
          </button>
        ))}
      </div>
      </div>

      {activeTab === 'notifications' && (
        <div className="space-y-3">
          {notificationsQuery.isLoading ? (
            <div className="flex justify-center py-12">
              <Loader2 className="size-6 animate-spin text-muted-foreground" />
            </div>
          ) : notifications.length === 0 ? (
            <div className="flex flex-col items-center gap-2 py-24 text-center">
              <Bell className="size-10 text-muted-foreground/40" />
              <p className="text-sm text-muted-foreground">No tienes notificaciones</p>
            </div>
          ) : (
            <>
              {notifications.map((n) => (
                <NotificationItem key={n.id} notification={n} orgMap={orgMap} projectMap={projectMap} />
              ))}
              {notificationsQuery.hasNextPage && (
                <div className="flex justify-center pt-2">
                  <button
                    type="button"
                    className="text-sm text-primary hover:underline disabled:opacity-50"
                    disabled={notificationsQuery.isFetchingNextPage}
                    onClick={() => notificationsQuery.fetchNextPage()}
                  >
                    {notificationsQuery.isFetchingNextPage ? (
                      <Loader2 className="size-4 animate-spin" />
                    ) : (
                      'Cargar más'
                    )}
                  </button>
                </div>
              )}
            </>
          )}
        </div>
      )}

      {activeTab === 'approvals' && (
        <div className="space-y-3">
          {approvalsQuery.isLoading ? (
            <div className="flex justify-center py-12">
              <Loader2 className="size-6 animate-spin text-muted-foreground" />
            </div>
          ) : approvals.length === 0 ? (
            <div className="flex flex-col items-center gap-2 py-24 text-center">
              <CheckSquare className="size-10 text-muted-foreground/40" />
              <p className="text-sm text-muted-foreground">No hay solicitudes pendientes</p>
            </div>
          ) : (
            <>
              {approvals.map((a) => (
                <ApprovalItem key={a.id} approval={a} orgMap={orgMap} projectMap={projectMap} />
              ))}
              {approvalsQuery.hasNextPage && (
                <div className="flex justify-center pt-2">
                  <button
                    type="button"
                    className="text-sm text-primary hover:underline disabled:opacity-50"
                    disabled={approvalsQuery.isFetchingNextPage}
                    onClick={() => approvalsQuery.fetchNextPage()}
                  >
                    {approvalsQuery.isFetchingNextPage ? (
                      <Loader2 className="size-4 animate-spin" />
                    ) : (
                      'Cargar más'
                    )}
                  </button>
                </div>
              )}
            </>
          )}
        </div>
      )}
    </div>
  )
}
