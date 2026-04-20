import { Check, ChevronsUpDown, Plus, Trash2 } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { toast } from 'sonner'

import { useAppTranslation } from '@/core/i18n'
import { useCancelOrganizationInvitationMutation } from '@/core/query/organization/invitations'
import { useCreateOrganizationInvitationMutation } from '@/core/query/organization/invitations'
import { useOrganizationPendingInvitationsQuery } from '@/core/query/organization/invitations'
import { useOrganizationMembersDistributionQuery } from '@/core/query/organization/members'
import { useDeleteOrganizationMemberMutation } from '@/core/query/organization/members'
import { useOrganizationMembersInfiniteQuery } from '@/core/query/organization/members'
import { useUpdateOrganizationMemberRoleMutation } from '@/core/query/organization/members'
import { useOrganizationRolesQuery } from '@/core/query/organization/roles'
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
  getMemberInitials,
  getRoleAccentClassName,
} from '@/modules/organization/pages/settings/utils/organizationMembers.utils'
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

interface OrganizationSettingsMembersProps {
  organizationId: string
  canManageMemberRoles: boolean
  canInviteMembers: boolean
  canRemoveMembers: boolean
}

interface InviteUserSelection {
  id: string
  fullName: string
  email: string
}

interface PendingInvitationRow {
  id: string
  userLabel: string
  roleLabel: string
  invitedAt: string
}

export function OrganizationSettingsMembers({
  organizationId,
  canManageMemberRoles,
  canInviteMembers,
  canRemoveMembers,
}: OrganizationSettingsMembersProps) {
  const { t } = useAppTranslation('organizations')
  const tr = (key: string, defaultValue: string) =>
    t(key, {
      defaultValue,
    })
  const [isInviteDialogOpen, setIsInviteDialogOpen] = useState(false)
  const [isInviteUserComboboxOpen, setIsInviteUserComboboxOpen] =
    useState(false)
  const [inviteSearch, setInviteSearch] = useState('')
  const [debouncedInviteSearch, setDebouncedInviteSearch] = useState('')
  const [inviteUserId, setInviteUserId] = useState('')
  const [inviteSelectedUser, setInviteSelectedUser] =
    useState<InviteUserSelection | null>(null)
  const [inviteRoleId, setInviteRoleId] = useState('')

  const membersQuery = useOrganizationMembersInfiniteQuery(organizationId, {
    limit: 50,
  })
  const createInvitationMutation =
    useCreateOrganizationInvitationMutation(organizationId)
  const cancelInvitationMutation =
    useCancelOrganizationInvitationMutation(organizationId)
  const pendingInvitationsQuery =
    useOrganizationPendingInvitationsQuery(organizationId)
  const distributionQuery =
    useOrganizationMembersDistributionQuery(organizationId)
  const rolesQuery = useOrganizationRolesQuery(organizationId)
  const usersQuery = useUsersInfiniteQuery(
    {
      limit: 20,
      search: debouncedInviteSearch,
    },
    isInviteDialogOpen || Boolean(pendingInvitationsQuery.data?.data.length)
  )
  const deleteMemberMutation =
    useDeleteOrganizationMemberMutation(organizationId)
  const updateMemberRoleMutation =
    useUpdateOrganizationMemberRoleMutation(organizationId)

  const members = useMemo(
    () => membersQuery.data?.data ?? [],
    [membersQuery.data?.data]
  )
  const roles = useMemo(
    () => rolesQuery.data?.data ?? [],
    [rolesQuery.data?.data]
  )
  const availableUsers = useMemo(
    () => usersQuery.data?.data ?? [],
    [usersQuery.data?.data]
  )
  const usersById = useMemo(() => {
    return new Map(availableUsers.map((user) => [user.id, user.fullName]))
  }, [availableUsers])

  useEffect(() => {
    if (!isInviteDialogOpen) {
      return
    }

    const timeoutId = window.setTimeout(() => {
      setDebouncedInviteSearch(inviteSearch.trim())
    }, 300)

    return () => {
      window.clearTimeout(timeoutId)
    }
  }, [inviteSearch, isInviteDialogOpen])

  const roleNameById = useMemo(() => {
    return new Map(roles.map((role) => [role.id, role.name]))
  }, [roles])

  const pendingInvitations = useMemo(() => {
    return pendingInvitationsQuery.data?.data ?? []
  }, [pendingInvitationsQuery.data?.data])

  const pendingInvitationRows = useMemo<PendingInvitationRow[]>(() => {
    return pendingInvitations.map((invitation) => {
      const userLabel = usersById.get(invitation.userId) ?? invitation.userId
      const roleLabel =
        roleNameById.get(invitation.organizationRoleId) ??
        t('management.settings.members.roleFallback')

      return {
        id: invitation.id,
        userLabel,
        roleLabel,
        invitedAt: invitation.invitedAt,
      }
    })
  }, [pendingInvitations, roleNameById, t, usersById])

  const ownerRoleId = useMemo(() => {
    return roles.find((role) => role.name.trim().toLowerCase() === 'owner')?.id
  }, [roles])

  const ownerMembersCount = useMemo(() => {
    if (!ownerRoleId) {
      return 0
    }

    return members.filter((member) => member.role === ownerRoleId).length
  }, [members, ownerRoleId])

  const roleAccentById = useMemo(() => {
    return new Map(
      roles.map((role, index) => [role.id, getRoleAccentClassName(index)])
    )
  }, [roles])

  const distributionRows = useMemo(() => {
    const distribution = distributionQuery.data?.data ?? []
    const distributionByRoleId = new Map(
      distribution.map((item) => [item.id, item.memberCount])
    )

    const rowsFromRoles = roles.map((role, index) => ({
      id: role.id,
      name: role.name,
      memberCount: distributionByRoleId.get(role.id) ?? 0,
      accentClassName: getRoleAccentClassName(index),
    }))

    const knownIds = new Set(rowsFromRoles.map((row) => row.id))
    const unknownRows = distribution
      .filter((item) => !knownIds.has(item.id))
      .map((item, index) => ({
        id: item.id,
        name: `${t('management.settings.members.roleFallback')} (${item.id.slice(0, 8)})`,
        memberCount: item.memberCount,
        accentClassName: getRoleAccentClassName(rowsFromRoles.length + index),
      }))

    return [...rowsFromRoles, ...unknownRows]
  }, [distributionQuery.data?.data, roles, t])

  const totalMembers =
    members.length > 0
      ? members.length
      : distributionRows.reduce((sum, row) => sum + row.memberCount, 0)

  const isLoading =
    membersQuery.isLoading ||
    rolesQuery.isLoading ||
    distributionQuery.isLoading ||
    pendingInvitationsQuery.isLoading

  const isError =
    membersQuery.isError ||
    rolesQuery.isError ||
    distributionQuery.isError ||
    pendingInvitationsQuery.isError

  const isInvitingMember = createInvitationMutation.isPending
  const isCancellingInvitation = cancelInvitationMutation.isPending
  const isLoadingUsers = usersQuery.isLoading
  const isUpdatingMemberRole = updateMemberRoleMutation.isPending
  const isDeletingMember = deleteMemberMutation.isPending

  const inviteDisabled =
    !canInviteMembers || roles.length === 0 || isInvitingMember

  const handleInviteSubmit = async () => {
    if (!canInviteMembers) {
      toast.info(t('management.settings.members.permissions.noInvite'))
      return
    }

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
    setDebouncedInviteSearch('')
    setIsInviteDialogOpen(false)
  }

  return (
    <section className="space-y-6">
      <header className="flex flex-wrap items-start justify-between gap-4 rounded-lg border bg-card p-6">
        <div className="space-y-1">
          <h2 className="text-lg font-semibold tracking-tight">
            {t('management.settings.members.title')}
          </h2>
          <p className="text-sm text-muted-foreground">
            {t('management.settings.members.subtitle')}
          </p>
        </div>

        <Button
          type="button"
          data-icon="inline-start"
          disabled={inviteDisabled}
          onClick={() => {
            if (!canInviteMembers) {
              toast.info(t('management.settings.members.permissions.noInvite'))
              return
            }

            setInviteSearch('')
            setDebouncedInviteSearch('')
            setInviteUserId('')
            setInviteSelectedUser(null)
            setInviteRoleId(roles[0]?.id ?? '')
            setIsInviteUserComboboxOpen(false)
            setIsInviteDialogOpen(true)
          }}
          title={
            canInviteMembers
              ? t('management.settings.members.actions.inviteMember')
              : t('management.settings.members.permissions.noInvite')
          }
        >
          <Plus />
          {t('management.settings.members.actions.inviteMember')}
        </Button>
      </header>

      {isLoading ? (
        <p className="text-sm text-muted-foreground">
          {t('management.settings.members.states.loading')}
        </p>
      ) : null}

      {isError ? (
        <p className="text-sm text-destructive">
          {t('management.settings.members.states.error')}
        </p>
      ) : null}

      {!isLoading && !isError && pendingInvitationRows.length > 0 ? (
        <section className="space-y-4 rounded-lg border bg-card p-6">
          <h3 className="text-sm font-semibold">
            {t('management.settings.members.pendingInvitations.title')}
          </h3>

          <div className="space-y-3">
            {pendingInvitationRows.map((invitation) => (
              <article
                key={invitation.id}
                className="flex flex-col gap-3 rounded-md border bg-muted/20 p-3 sm:flex-row sm:items-center sm:justify-between"
              >
                <div className="space-y-1">
                  <p className="text-sm font-semibold">
                    {invitation.userLabel}
                  </p>
                  <p className="text-xs text-muted-foreground">
                    {invitation.roleLabel}
                  </p>
                  <p className="text-xs text-muted-foreground">
                    {t(
                      'management.settings.members.pendingInvitations.invitedAt',
                      {
                        defaultValue: 'Invited at {{date}}',
                        date: new Date(invitation.invitedAt).toLocaleString(),
                      }
                    )}
                  </p>
                </div>

                <div className="flex items-center gap-2">
                  <span className="inline-flex w-fit rounded-full bg-amber-500/15 px-2 py-1 text-xs font-medium text-amber-600">
                    {t(
                      'management.settings.members.pendingInvitations.pending',
                      {
                        defaultValue: 'Pending',
                      }
                    )}
                  </span>

                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    disabled={isCancellingInvitation}
                    onClick={() => {
                      cancelInvitationMutation.mutate({
                        invitationId: invitation.id,
                        data: {
                          status: 'canceled',
                        },
                      })
                    }}
                    title={t(
                      'management.settings.members.pendingInvitations.cancel',
                      {
                        defaultValue: 'Cancel invitation',
                      }
                    )}
                  >
                    {t(
                      'management.settings.members.pendingInvitations.cancel',
                      {
                        defaultValue: 'Cancel invitation',
                      }
                    )}
                  </Button>
                </div>
              </article>
            ))}
          </div>
        </section>
      ) : null}

      {!isLoading && !isError && members.length === 0 ? (
        <p className="text-sm text-muted-foreground">
          {t('management.settings.members.states.emptyMembers')}
        </p>
      ) : null}

      {!isLoading && !isError && members.length > 0 ? (
        <div className="space-y-2">
          {members.map((member) => {
            const roleName =
              roleNameById.get(member.role) ??
              t('management.settings.members.roleFallback')
            const accentClassName =
              roleAccentById.get(member.role) ?? getRoleAccentClassName(0)
            const isLastOwner =
              Boolean(ownerRoleId) &&
              member.role === ownerRoleId &&
              ownerMembersCount <= 1
            const isOwnerMember =
              Boolean(ownerRoleId) && member.role === ownerRoleId

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
                    <p className="text-xs text-muted-foreground">
                      {member.email}
                    </p>
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
                      !canManageMemberRoles ||
                      isUpdatingMemberRole ||
                      isDeletingMember
                    }
                    onValueChange={(nextRoleId) => {
                      if (nextRoleId === member.role) {
                        return
                      }

                      if (isLastOwner && nextRoleId !== ownerRoleId) {
                        toast.error(
                          t(
                            'management.settings.members.feedback.atLeastOneOwner',
                            {
                              defaultValue:
                                'At least one owner is required in the organization.',
                            }
                          )
                        )
                        return
                      }

                      updateMemberRoleMutation.mutate({
                        memberId: member.id,
                        data: {
                          roleId: nextRoleId,
                        },
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
                      !canRemoveMembers || isOwnerMember || isDeletingMember
                    }
                    onClick={() => {
                      if (!canRemoveMembers) {
                        toast.info(
                          t('management.settings.members.permissions.noRemove')
                        )
                        return
                      }

                      if (isOwnerMember) {
                        toast.error(
                          t(
                            'management.settings.members.feedback.ownerCannotBeRemoved',
                            {
                              defaultValue: 'Owner members cannot be removed.',
                            }
                          )
                        )
                        return
                      }

                      deleteMemberMutation.mutate(member.id)
                    }}
                    title={
                      !canRemoveMembers
                        ? t('management.settings.members.permissions.noRemove')
                        : isOwnerMember
                          ? t(
                              'management.settings.members.feedback.ownerCannotBeRemoved',
                              {
                                defaultValue:
                                  'Owner members cannot be removed.',
                              }
                            )
                          : t(
                              'management.settings.members.actions.removeMember'
                            )
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
        <section className="space-y-4 rounded-lg border bg-card p-6">
          <h3 className="text-sm font-semibold">
            {t('management.settings.members.distribution.title')}
          </h3>

          {distributionRows.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              {t('management.settings.members.states.emptyDistribution')}
            </p>
          ) : (
            <ul className="space-y-3">
              {distributionRows.map((row) => {
                const percentage =
                  totalMembers > 0
                    ? Math.round((row.memberCount / totalMembers) * 100)
                    : 0

                return (
                  <li key={row.id} className="space-y-1">
                    <div className="flex items-center justify-between gap-2 text-sm">
                      <p className="flex items-center gap-2 text-muted-foreground">
                        <span
                          className={`size-2 rounded-full ${row.accentClassName}`}
                        />
                        {row.name}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        {row.memberCount} ({percentage}%)
                      </p>
                    </div>

                    <div className="h-2 rounded-full bg-muted">
                      <div
                        className={`h-full rounded-full ${row.accentClassName}`}
                        style={{ width: `${percentage}%` }}
                      />
                    </div>
                  </li>
                )
              })}
            </ul>
          )}
        </section>
      ) : null}

      <Dialog
        open={isInviteDialogOpen}
        onOpenChange={(open) => {
          if (!isInvitingMember) {
            if (!open) {
              setIsInviteUserComboboxOpen(false)
            }
            setIsInviteDialogOpen(open)
          }
        }}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {tr(
                'management.settings.members.inviteDialog.title',
                'Invite member'
              )}
            </DialogTitle>
            <DialogDescription>
              {tr(
                'management.settings.members.inviteDialog.description',
                'Invite a user and assign an organization role.'
              )}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            <div className="space-y-2">
              <p className="text-sm font-medium">
                {tr(
                  'management.settings.members.inviteDialog.userIdLabel',
                  'User'
                )}
              </p>
              <Popover
                open={isInviteUserComboboxOpen}
                onOpenChange={(open) => {
                  if (isInvitingMember) {
                    return
                  }

                  setIsInviteUserComboboxOpen(open)
                }}
              >
                <PopoverTrigger asChild>
                  <Button
                    type="button"
                    variant="outline"
                    role="combobox"
                    aria-expanded={isInviteUserComboboxOpen}
                    className="w-full justify-between"
                    disabled={isInvitingMember}
                  >
                    {inviteSelectedUser
                      ? inviteSelectedUser.fullName
                      : tr(
                          'management.settings.members.inviteDialog.userPlaceholder',
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
                        'management.settings.members.inviteDialog.searchPlaceholder',
                        'Search by name or email'
                      )}
                    />
                    <CommandList>
                      {isLoadingUsers ? (
                        <CommandEmpty>
                          {tr(
                            'management.settings.members.inviteDialog.loadingUsers',
                            'Loading users...'
                          )}
                        </CommandEmpty>
                      ) : null}

                      {!isLoadingUsers && availableUsers.length === 0 ? (
                        <CommandEmpty>
                          {tr(
                            'management.settings.members.inviteDialog.emptyUsers',
                            'No users found.'
                          )}
                        </CommandEmpty>
                      ) : null}

                      {!isLoadingUsers && availableUsers.length > 0 ? (
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
                                setIsInviteUserComboboxOpen(false)
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
                              {user.fullName}
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
                {tr(
                  'management.settings.members.inviteDialog.roleLabel',
                  'Role'
                )}
              </p>
              <Select
                value={inviteRoleId}
                onValueChange={setInviteRoleId}
                disabled={isInvitingMember || roles.length === 0}
              >
                <SelectTrigger>
                  <SelectValue
                    placeholder={tr(
                      'management.settings.members.inviteDialog.rolePlaceholder',
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
              onClick={() => {
                setIsInviteDialogOpen(false)
              }}
              disabled={isInvitingMember}
            >
              {tr('management.settings.members.inviteDialog.cancel', 'Cancel')}
            </Button>

            <Button
              type="button"
              onClick={() => {
                void handleInviteSubmit()
              }}
              disabled={
                isInvitingMember ||
                inviteUserId.trim().length === 0 ||
                inviteRoleId.trim().length === 0
              }
            >
              {isInvitingMember
                ? tr(
                    'management.settings.members.inviteDialog.submitting',
                    'Sending...'
                  )
                : tr(
                    'management.settings.members.inviteDialog.submit',
                    'Send invitation'
                  )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </section>
  )
}
