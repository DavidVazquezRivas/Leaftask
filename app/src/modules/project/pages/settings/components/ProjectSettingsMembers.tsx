import { Bot, Check, ChevronDown, ChevronsUpDown, Trash2, UserPlus } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { toast } from 'sonner'

import { useAppTranslation } from '@/core/i18n'
import {
  useCancelProjectInvitationMutation,
  useCreateProjectInvitationMutation,
  useDeleteProjectMemberMutation,
  useProjectMembersQuery,
  useProjectPendingInvitationsQuery,
  useUpdateProjectMemberRoleMutation,
  useProjectRolesQuery,
} from '@/core/query/project'
import { useDeleteAgentMutation } from '@/core/query/agent'
import { useUsersInfiniteQuery } from '@/core/query/user/users'
import { Button } from '@/shared/components/ui/button'
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/shared/components/ui/command'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog'
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/shared/components/ui/popover'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'
import { cn } from '@/shared/lib/utils'

import {
  getMemberInitials,
  getRoleAccentClassName,
} from '@/modules/project/pages/settings/utils/projectMembers.utils'
import { CreateAgentDialog } from '@/modules/chat/components/CreateAgentDialog'

interface ProjectSettingsMembersProps {
  projectId: string
  canManageMembers: boolean
}

interface InviteUserSelection {
  id: string
  fullName: string
  email: string
}

export function ProjectSettingsMembers({
  projectId,
  canManageMembers,
}: ProjectSettingsMembersProps) {
  const { t } = useAppTranslation('projects')
  const tr = (key: string, defaultValue: string) => t(key, { defaultValue })

  const [isPendingCollapsed, setIsPendingCollapsed] = useState(false)
  const [isInviteDialogOpen, setIsInviteDialogOpen] = useState(false)
  const [isUserComboboxOpen, setIsUserComboboxOpen] = useState(false)
  const [inviteSearch, setInviteSearch] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const [inviteUserId, setInviteUserId] = useState('')
  const [inviteSelectedUser, setInviteSelectedUser] =
    useState<InviteUserSelection | null>(null)
  const [inviteRoleId, setInviteRoleId] = useState('')

  const membersQuery = useProjectMembersQuery(projectId, { limit: 50 })
  const pendingInvitationsQuery = useProjectPendingInvitationsQuery(projectId)
  const rolesQuery = useProjectRolesQuery(projectId)
  const usersQuery = useUsersInfiniteQuery(
    { limit: 20, search: debouncedSearch },
    isInviteDialogOpen
  )
  const updateRoleMutation = useUpdateProjectMemberRoleMutation(projectId)
  const deleteMemberMutation = useDeleteProjectMemberMutation(projectId)
  const deleteAgentMutation = useDeleteAgentMutation(projectId)
  const createInvitationMutation = useCreateProjectInvitationMutation(projectId)
  const cancelInvitationMutation = useCancelProjectInvitationMutation(projectId)

  const members = useMemo(
    () => membersQuery.data?.data ?? [],
    [membersQuery.data?.data]
  )
  const roles = useMemo(
    () => rolesQuery.data?.data ?? [],
    [rolesQuery.data?.data]
  )
  const pendingInvitations = useMemo(
    () => pendingInvitationsQuery.data?.data ?? [],
    [pendingInvitationsQuery.data?.data]
  )
  const availableUsers = useMemo(
    () => usersQuery.data?.data ?? [],
    [usersQuery.data?.data]
  )

  useEffect(() => {
    if (!isInviteDialogOpen) return

    const id = window.setTimeout(() => {
      setDebouncedSearch(inviteSearch.trim())
    }, 300)

    return () => {
      window.clearTimeout(id)
    }
  }, [inviteSearch, isInviteDialogOpen])

  const ownerRoleId = useMemo(
    () => roles.find((r) => r.name.trim().toLowerCase() === 'owner')?.id,
    [roles]
  )

  const ownerMembersCount = useMemo(() => {
    if (!ownerRoleId) return 0
    return members.filter((m) => m.role === ownerRoleId && m.type === 'person').length
  }, [members, ownerRoleId])

  const roleNameById = useMemo(
    () => new Map(roles.map((r) => [r.id, r.name])),
    [roles]
  )

  const roleAccentById = useMemo(
    () => new Map(roles.map((r, i) => [r.id, getRoleAccentClassName(i)])),
    [roles]
  )

  const isLoading =
    membersQuery.isLoading ||
    rolesQuery.isLoading ||
    pendingInvitationsQuery.isLoading

  const isError =
    membersQuery.isError ||
    rolesQuery.isError ||
    pendingInvitationsQuery.isError

  const isUpdatingRole = updateRoleMutation.isPending
  const isDeletingMember = deleteMemberMutation.isPending || deleteAgentMutation.isPending
  const isInviting = createInvitationMutation.isPending
  const isCancelling = cancelInvitationMutation.isPending

  const agentsCount = members.filter((m) => m.type === 'agent').length
  const peopleCount = members.length - agentsCount
  const totalCount = members.length

  const openInviteDialog = () => {
    setInviteSearch('')
    setDebouncedSearch('')
    setInviteUserId('')
    setInviteSelectedUser(null)
    setInviteRoleId(roles[0]?.id ?? '')
    setIsUserComboboxOpen(false)
    setIsInviteDialogOpen(true)
  }

  const handleInviteSubmit = async () => {
    if (inviteUserId.trim().length === 0 || inviteRoleId.trim().length === 0) {
      toast.error(t('management.create.validation.required'))
      return
    }

    await createInvitationMutation.mutateAsync({
      userId: inviteUserId.trim(),
      roleId: inviteRoleId,
    })

    setInviteUserId('')
    setInviteSelectedUser(null)
    setInviteRoleId('')
    setInviteSearch('')
    setDebouncedSearch('')
    setIsInviteDialogOpen(false)
  }

  return (
    <section className="space-y-6">
      <header className="flex flex-wrap items-start justify-between gap-4 rounded-lg border bg-card p-6">
        <div className="space-y-1">
          <h2 className="text-lg font-semibold tracking-tight">
            {t('management.members.title')}
          </h2>
          <p className="text-sm text-muted-foreground">
            {t('management.members.subtitle')}
          </p>
        </div>

        <div className="flex shrink-0 items-center gap-2">
          <CreateAgentDialog
            projectId={projectId}
            onCreated={() => toast.success(t('management.members.actions.agentCreated', { defaultValue: 'Agente creado correctamente' }))}
            trigger={
              <Button
                type="button"
                variant="outline"
                data-icon="inline-start"
                disabled={!canManageMembers}
                title={!canManageMembers ? t('management.members.permissions.noInvite') : undefined}
              >
                <Bot />
                {t('management.members.actions.createAgent')}
              </Button>
            }
          />

          <Button
            type="button"
            data-icon="inline-start"
            disabled={!canManageMembers || roles.length === 0 || isInviting}
            onClick={openInviteDialog}
            title={
              !canManageMembers
                ? t('management.members.permissions.noInvite')
                : undefined
            }
          >
            <UserPlus />
            {t('management.members.actions.inviteMember')}
          </Button>
        </div>
      </header>

      {isLoading ? (
        <p className="text-sm text-muted-foreground">
          {t('management.members.states.loading')}
        </p>
      ) : null}

      {isError ? (
        <p className="text-sm text-destructive">
          {t('management.members.states.error')}
        </p>
      ) : null}

      {!isLoading && !isError && pendingInvitations.length > 0 ? (
        <section className="space-y-4 rounded-lg border bg-card p-6">
          <div className="flex items-center justify-between">
            <h3 className="text-sm font-semibold">
              {t('management.members.pendingInvitations.title')}
            </h3>
            <Button
              type="button"
              variant="ghost"
              size="sm"
              onClick={() => setIsPendingCollapsed(!isPendingCollapsed)}
              title={
                isPendingCollapsed
                  ? tr('management.members.pendingInvitations.expand', 'Expand')
                  : tr(
                      'management.members.pendingInvitations.collapse',
                      'Collapse'
                    )
              }
            >
              <ChevronDown
                className={cn(
                  'size-4 transition-transform',
                  isPendingCollapsed && 'rotate-180'
                )}
              />
            </Button>
          </div>

          {!isPendingCollapsed && (
            <div className="space-y-3">
              {pendingInvitations.map((invitation) => (
                <article
                  key={invitation.id}
                  className="flex flex-col gap-3 rounded-md border bg-muted/20 p-3 sm:flex-row sm:items-center sm:justify-between"
                >
                  <div className="space-y-0.5">
                    <p className="text-sm font-semibold">
                      {invitation.user.name}
                    </p>
                    {invitation.user.email ? (
                      <p className="text-xs text-muted-foreground">
                        {invitation.user.email}
                      </p>
                    ) : null}
                    <p className="text-xs text-muted-foreground">
                      {invitation.role.name}
                    </p>
                  </div>

                  <div className="flex items-center gap-2">
                    <span className="inline-flex w-fit rounded-full bg-amber-500/15 px-2 py-1 text-xs font-medium text-amber-600">
                      {tr(
                        'management.members.pendingInvitations.pending',
                        'Pending'
                      )}
                    </span>

                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      disabled={!canManageMembers || isCancelling}
                      onClick={() => {
                        cancelInvitationMutation.mutate(invitation.id)
                      }}
                      title={tr(
                        'management.members.pendingInvitations.cancel',
                        'Cancel invitation'
                      )}
                    >
                      {tr(
                        'management.members.pendingInvitations.cancel',
                        'Cancel invitation'
                      )}
                    </Button>
                  </div>
                </article>
              ))}
            </div>
          )}
        </section>
      ) : null}

      {!isLoading && !isError && members.length === 0 ? (
        <p className="text-sm text-muted-foreground">
          {t('management.members.states.emptyMembers')}
        </p>
      ) : null}

      {!isLoading && !isError && members.length > 0 ? (
        <div className="space-y-2">
          {members.map((member) => {
            const roleName =
              roleNameById.get(member.role) ??
              t('management.members.roleFallback')
            const accentClassName =
              roleAccentById.get(member.role) ?? getRoleAccentClassName(0)
            const isOwnerMember =
              member.type === 'person' && Boolean(ownerRoleId) && member.role === ownerRoleId
            const isLastOwner = isOwnerMember && ownerMembersCount <= 1

            return (
              <article
                key={member.id}
                className="flex flex-col gap-3 rounded-lg border bg-card p-3 sm:flex-row sm:items-center sm:justify-between"
              >
                <div className="flex items-center gap-3">
                  <div className="flex size-10 items-center justify-center rounded-md bg-muted text-sm font-semibold text-muted-foreground">
                    {getMemberInitials(member.name)}
                  </div>

                  <div className="space-y-0.5">
                    <p className="text-sm font-semibold">{member.name}</p>
                    {member.email ? (
                      <p className="text-xs text-muted-foreground">
                        {member.email}
                      </p>
                    ) : null}
                    <p className="flex items-center gap-1 text-xs text-muted-foreground">
                      <span
                        className={`size-1.5 rounded-full ${accentClassName}`}
                      />
                      {roleName}
                    </p>
                  </div>
                </div>

                <div className="flex items-center gap-2 sm:w-auto">
                  <Select
                    value={member.role}
                    disabled={
                      !canManageMembers || isUpdatingRole || isDeletingMember || member.type === 'agent'
                    }
                    onValueChange={(nextRoleId) => {
                      if (nextRoleId === member.role) return

                      if (isLastOwner && nextRoleId !== ownerRoleId) {
                        toast.error(
                          t('management.members.feedback.atLeastOneOwner')
                        )
                        return
                      }

                      updateRoleMutation.mutate({
                        memberId: member.id,
                        data: { roleId: nextRoleId },
                      })
                    }}
                  >
                    <SelectTrigger className="w-full sm:w-44">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {roles.length > 0 ? (
                        roles.map((role) => (
                          <SelectItem
                            key={role.id}
                            value={role.id}
                            disabled={Boolean(
                              isLastOwner && role.id !== ownerRoleId
                            )}
                          >
                            {role.name}
                          </SelectItem>
                        ))
                      ) : (
                        <SelectItem key={member.role} value={member.role}>
                          {roleName}
                        </SelectItem>
                      )}
                    </SelectContent>
                  </Select>

                  <Button
                    type="button"
                    size="icon-sm"
                    variant="ghost"
                    disabled={
                      !canManageMembers || isOwnerMember || isDeletingMember
                    }
                    onClick={() => {
                      if (!canManageMembers) {
                        toast.info(t('management.members.permissions.noRemove'))
                        return
                      }

                      if (isOwnerMember) {
                        toast.error(
                          t('management.members.feedback.ownerCannotBeRemoved')
                        )
                        return
                      }

                      if (member.type === 'agent') {
                        deleteAgentMutation.mutate(member.id)
                      } else {
                        deleteMemberMutation.mutate(member.id)
                      }
                    }}
                    title={
                      !canManageMembers
                        ? t('management.members.permissions.noRemove')
                        : isOwnerMember
                          ? t(
                              'management.members.feedback.ownerCannotBeRemoved'
                            )
                          : t('management.members.actions.removeMember')
                    }
                  >
                    <Trash2 className="size-4" />
                  </Button>
                </div>
              </article>
            )
          })}
        </div>
      ) : null}

      {!isLoading && !isError ? (
        <div className="grid grid-cols-3 gap-4">
          <div className="rounded-lg border bg-card p-4">
            <p className="text-2xl font-bold">{peopleCount}</p>
            <p className="text-sm text-muted-foreground">
              {t('management.members.stats.people')}
            </p>
          </div>

          <div className="rounded-lg border bg-card p-4">
            <p className="text-2xl font-bold">{agentsCount}</p>
            <p className="text-sm text-muted-foreground">
              {t('management.members.stats.agents')}
            </p>
          </div>

          <div className="rounded-lg border bg-card p-4">
            <p className="text-2xl font-bold">{totalCount}</p>
            <p className="text-sm text-muted-foreground">
              {t('management.members.stats.total')}
            </p>
          </div>
        </div>
      ) : null}

      {/* Invite member dialog */}
      <Dialog
        open={isInviteDialogOpen}
        onOpenChange={(open) => {
          if (!isInviting) {
            if (!open) setIsUserComboboxOpen(false)
            setIsInviteDialogOpen(open)
          }
        }}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {tr('management.members.inviteDialog.title', 'Invite member')}
            </DialogTitle>
            <DialogDescription>
              {tr(
                'management.members.inviteDialog.description',
                'Invite a user and assign a project role.'
              )}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            <div className="space-y-2">
              <p className="text-sm font-medium">
                {tr('management.members.inviteDialog.userLabel', 'User')}
              </p>

              <Popover
                open={isUserComboboxOpen}
                onOpenChange={(open) => {
                  if (!isInviting) setIsUserComboboxOpen(open)
                }}
              >
                <PopoverTrigger asChild>
                  <Button
                    type="button"
                    variant="outline"
                    role="combobox"
                    aria-expanded={isUserComboboxOpen}
                    className="w-full justify-between"
                    disabled={isInviting}
                  >
                    {inviteSelectedUser
                      ? inviteSelectedUser.fullName
                      : tr(
                          'management.members.inviteDialog.userPlaceholder',
                          'Search and select a user'
                        )}
                    <ChevronsUpDown className="ml-2 size-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>

                <PopoverContent className="w-(--radix-popover-trigger-width) p-0">
                  <Command shouldFilter={false}>
                    <CommandInput
                      value={inviteSearch}
                      onValueChange={setInviteSearch}
                      placeholder={tr(
                        'management.members.inviteDialog.searchPlaceholder',
                        'Search by name or email...'
                      )}
                    />
                    <CommandList>
                      {usersQuery.isLoading ? (
                        <CommandEmpty>
                          {tr(
                            'management.members.inviteDialog.loadingUsers',
                            'Loading users...'
                          )}
                        </CommandEmpty>
                      ) : null}

                      {!usersQuery.isLoading && availableUsers.length === 0 ? (
                        <CommandEmpty>
                          {tr(
                            'management.members.inviteDialog.emptyUsers',
                            'No users found.'
                          )}
                        </CommandEmpty>
                      ) : null}

                      {!usersQuery.isLoading && availableUsers.length > 0 ? (
                        <CommandGroup>
                          {availableUsers.map((user) => (
                            <CommandItem
                              key={user.id}
                              value={user.id}
                              onSelect={() => {
                                setInviteUserId(user.id)
                                setInviteSelectedUser({
                                  id: user.id,
                                  fullName: user.fullName,
                                  email: user.email,
                                })
                                setIsUserComboboxOpen(false)
                              }}
                            >
                              <Check
                                className={cn(
                                  'size-4',
                                  inviteUserId === user.id
                                    ? 'opacity-100'
                                    : 'opacity-0'
                                )}
                              />
                              <span>{user.fullName}</span>
                              <span className="ml-auto text-xs text-muted-foreground">
                                {user.email}
                              </span>
                            </CommandItem>
                          ))}
                        </CommandGroup>
                      ) : null}
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            <div className="space-y-2">
              <p className="text-sm font-medium">
                {tr('management.members.inviteDialog.roleLabel', 'Role')}
              </p>

              <Select
                value={inviteRoleId}
                onValueChange={setInviteRoleId}
                disabled={isInviting || roles.length === 0}
              >
                <SelectTrigger>
                  <SelectValue
                    placeholder={tr(
                      'management.members.inviteDialog.rolePlaceholder',
                      'Select a role'
                    )}
                  />
                </SelectTrigger>
                <SelectContent>
                  {roles.map((role) => (
                    <SelectItem key={role.id} value={role.id}>
                      {role.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="ghost"
              disabled={isInviting}
              onClick={() => {
                setIsInviteDialogOpen(false)
              }}
            >
              {tr('management.members.inviteDialog.cancel', 'Cancel')}
            </Button>

            <Button
              type="button"
              disabled={
                isInviting ||
                inviteUserId.trim().length === 0 ||
                inviteRoleId.trim().length === 0
              }
              onClick={() => {
                void handleInviteSubmit()
              }}
            >
              {isInviting
                ? tr('management.members.inviteDialog.submitting', 'Sending...')
                : tr(
                    'management.members.inviteDialog.submit',
                    'Send invitation'
                  )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </section>
  )
}
