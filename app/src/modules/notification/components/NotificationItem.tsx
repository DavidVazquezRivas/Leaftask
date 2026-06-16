import { AtSign, Bell, Check, FolderOpen, Mail, X } from 'lucide-react'
import type { NotificationData } from '@/core/api/notification'
import { Button } from '@/shared/components/ui/button'
import {
  useRespondInvitationMutation,
  useRespondProjectInvitationMutation,
  useMarkNotificationAsReadMutation,
} from '@/core/query/notification'
import { useWorkItemDetailQuery } from '@/core/query/workitems'
import { cn } from '@/shared/lib/utils'

interface NotificationItemProps {
  notification: NotificationData
  orgMap: Record<string, string>
  projectMap: Record<string, string>
}

function getRelativeTime(timestamp: string): string {
  const now = new Date()
  const date = new Date(timestamp)
  const diffMs = now.getTime() - date.getTime()
  const diffMin = Math.floor(diffMs / 60_000)
  const diffHours = Math.floor(diffMin / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffMin < 1) return 'ahora'
  if (diffMin < 60) return `hace ${diffMin}m`
  if (diffHours < 24) return `hace ${diffHours}h`
  return `hace ${diffDays}d`
}

function NotificationIcon({ type }: { type: NotificationData['type'] }) {
  const base = 'flex size-8 shrink-0 items-center justify-center rounded-full'

  switch (type) {
    case 'Assignment':
      return <div className={cn(base, 'bg-blue-100 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400')}><Bell className="size-4" /></div>
    case 'Mention':
      return <div className={cn(base, 'bg-violet-100 text-violet-600 dark:bg-violet-900/30 dark:text-violet-400')}><AtSign className="size-4" /></div>
    case 'Invitation':
      return <div className={cn(base, 'bg-emerald-100 text-emerald-600 dark:bg-emerald-900/30 dark:text-emerald-400')}><Mail className="size-4" /></div>
    case 'ProjectInvitation':
      return <div className={cn(base, 'bg-cyan-100 text-cyan-600 dark:bg-cyan-900/30 dark:text-cyan-400')}><FolderOpen className="size-4" /></div>
    case 'Request':
      return <div className={cn(base, 'bg-amber-100 text-amber-600 dark:bg-amber-900/30 dark:text-amber-400')}><Bell className="size-4" /></div>
    default:
      return <div className={cn(base, 'bg-muted text-muted-foreground')}><Bell className="size-4" /></div>
  }
}

function getNotificationText(
  notification: NotificationData,
  orgMap: Record<string, string>,
  projectMap: Record<string, string>,
  workItemTitle: string | undefined
): string {
  switch (notification.type) {
    case 'Assignment': {
      const projectName = projectMap[notification.context.id] ?? '...'
      const itemName = workItemTitle ?? '...'
      return `Te han asignado a "${itemName}" en ${projectName}`
    }
    case 'Mention':
      return `${notification.actor?.name ?? 'Alguien'} te mencionó en un comentario`
    case 'Invitation': {
      const orgName = orgMap[notification.context.id] ?? '...'
      return `Te han invitado a unirte a la organización ${orgName}`
    }
    case 'ProjectInvitation': {
      const projectName = projectMap[notification.context.id] ?? '...'
      return `Te han invitado a unirte al proyecto ${projectName}`
    }
    case 'Request': {
      const orgName = orgMap[notification.context.id] ?? '...'
      return `Nueva solicitud en ${orgName}`
    }
    default:
      return 'Notificación'
  }
}

export function NotificationItem({ notification, orgMap, projectMap }: NotificationItemProps) {
  const orgRespondMutation = useRespondInvitationMutation()
  const projectRespondMutation = useRespondProjectInvitationMutation()
  const markAsReadMutation = useMarkNotificationAsReadMutation()

  const isOrgInvitation = notification.type === 'Invitation'
  const isProjectInvitation = notification.type === 'ProjectInvitation'
  const isInvitation = isOrgInvitation || isProjectInvitation
  const isAssignment = notification.type === 'Assignment'

  const workItemQuery = useWorkItemDetailQuery(
    isAssignment ? notification.context.id : null,
    isAssignment ? notification.target.id : null
  )

  const isPending = orgRespondMutation.isPending || projectRespondMutation.isPending

  const handleRespond = (e: React.MouseEvent, status: 'accepted' | 'rejected') => {
    e.stopPropagation()
    const onSuccess = () => markAsReadMutation.mutate(notification.id)
    if (isOrgInvitation) {
      orgRespondMutation.mutate(
        { organizationId: notification.context.id, invitationId: notification.target.id, status },
        { onSuccess }
      )
    } else {
      projectRespondMutation.mutate(
        { projectId: notification.context.id, invitationId: notification.target.id, status },
        { onSuccess }
      )
    }
  }

  const handleMarkAsRead = () => {
    if (!notification.read) {
      markAsReadMutation.mutate(notification.id)
    }
  }

  return (
    <div
      role="button"
      tabIndex={0}
      onClick={handleMarkAsRead}
      onKeyDown={(e) => e.key === 'Enter' && handleMarkAsRead()}
      className={cn(
        'flex cursor-pointer gap-3 rounded-lg border bg-card p-4 transition-colors',
        !notification.read
          ? 'border-primary/20 bg-primary/5 hover:bg-primary/10'
          : 'hover:bg-muted/50'
      )}
    >
      <NotificationIcon type={notification.type} />

      <div className="min-w-0 flex-1">
        <div className="flex items-start justify-between gap-2">
          <p className={cn('text-sm', !notification.read && 'font-medium')}>
            {getNotificationText(notification, orgMap, projectMap, workItemQuery.data?.data?.title)}
          </p>
          <div className="flex shrink-0 items-center gap-1.5">
            {!notification.read && (
              <span className="size-2 rounded-full bg-primary" />
            )}
            <span className="whitespace-nowrap text-xs text-muted-foreground">
              {getRelativeTime(notification.timestamp)}
            </span>
          </div>
        </div>

        {isInvitation && !notification.read && (
          <div className="mt-3 flex gap-2">
            <Button
              size="sm"
              variant="default"
              className="h-7 text-xs"
              onClick={(e) => handleRespond(e, 'accepted')}
              disabled={isPending}
            >
              <Check className="mr-1 size-3" />
              Aceptar
            </Button>
            <Button
              size="sm"
              variant="outline"
              className="h-7 text-xs"
              onClick={(e) => handleRespond(e, 'rejected')}
              disabled={isPending}
            >
              <X className="mr-1 size-3" />
              Rechazar
            </Button>
          </div>
        )}
      </div>
    </div>
  )
}
