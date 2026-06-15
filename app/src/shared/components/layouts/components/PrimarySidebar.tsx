import { Bell, Building2, MessageSquare, UserRound } from 'lucide-react'

import {
  CreateOrganizationButton,
  PrivateLanguageSwitcher,
  PrivateThemeToggle,
} from '@/shared/components/layouts/components'
import { Button } from '@/shared/components/ui/button'

interface PrimarySidebarProps {
  organizations: Array<{ id: string; name: string }>
  selectedOrganizationId: string | null
  onSelectPersonal: () => void
  onSelectOrganization: (organizationId: string) => void
  onPersonalLabel: string
  onNotificationsLabel: string
  onChatLabel: string
  onOrganizationsLabel: string
  isLoading: boolean
  isChatActive?: boolean
  isNotificationsActive?: boolean
  onChatClick?: () => void
  onNotificationsClick?: () => void
  unreadChatCount?: number
  unreadNotificationCount?: number
}

export function PrimarySidebar({
  organizations,
  selectedOrganizationId,
  onSelectPersonal,
  onSelectOrganization,
  onPersonalLabel,
  onNotificationsLabel,
  onChatLabel,
  onOrganizationsLabel,
  isLoading,
  isChatActive,
  isNotificationsActive,
  onChatClick,
  onNotificationsClick,
  unreadChatCount = 0,
  unreadNotificationCount = 0,
}: PrimarySidebarProps) {
  return (
    <aside className="flex h-full w-16 shrink-0 flex-col items-center border-r bg-card/60 py-4 backdrop-blur-sm">
      <Button
        size="icon-sm"
        variant={!selectedOrganizationId ? 'default' : 'outline'}
        aria-label={onPersonalLabel}
        title={onPersonalLabel}
        onClick={onSelectPersonal}
      >
        <UserRound />
      </Button>

      <div className="my-3 h-px w-8 bg-border/80" />

      <div
        className="flex min-h-0 flex-1 flex-col items-center gap-2 overflow-y-auto px-2"
        aria-label={onOrganizationsLabel}
      >
        {organizations.map((organization) => {
          const isSelected = organization.id === selectedOrganizationId

          return (
            <Button
              key={organization.id}
              size="icon-sm"
              variant={isSelected ? 'default' : 'ghost'}
              aria-label={organization.name}
              title={organization.name}
              onClick={() => {
                onSelectOrganization(organization.id)
              }}
            >
              <Building2 />
            </Button>
          )
        })}

        <CreateOrganizationButton />

        {isLoading ? (
          <div className="size-8 animate-pulse rounded-md bg-muted/70" />
        ) : null}
      </div>

      <div className="mt-auto flex flex-col items-center gap-2">
        <PrivateLanguageSwitcher />
        <PrivateThemeToggle />

        <div className="relative">
          <Button
            size="icon-sm"
            variant={isNotificationsActive ? 'default' : 'ghost'}
            aria-label={onNotificationsLabel}
            title={onNotificationsLabel}
            onClick={onNotificationsClick}
          >
            <Bell />
          </Button>
          {!isNotificationsActive && unreadNotificationCount > 0 && (
            <span className="pointer-events-none absolute -right-1 -top-1 flex h-4 min-w-4 items-center justify-center rounded-full bg-destructive px-1 text-[10px] font-semibold leading-none text-white">
              {unreadNotificationCount > 99 ? '99+' : unreadNotificationCount}
            </span>
          )}
        </div>

        <div className="relative">
          <Button
            size="icon-sm"
            variant={isChatActive ? 'default' : 'ghost'}
            aria-label={onChatLabel}
            title={onChatLabel}
            onClick={onChatClick}
          >
            <MessageSquare />
          </Button>
          {!isChatActive && unreadChatCount > 0 && (
            <span className="pointer-events-none absolute -right-1 -top-1 flex h-4 min-w-4 items-center justify-center rounded-full bg-destructive px-1 text-[10px] font-semibold leading-none text-white">
              {unreadChatCount > 99 ? '99+' : unreadChatCount}
            </span>
          )}
        </div>
      </div>
    </aside>
  )
}
