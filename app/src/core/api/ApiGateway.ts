import { SessionGateway } from '@/core/api/user/session'
import { UsersGateway } from '@/core/api/user/users'
import { OrganizationManagementGateway } from '@/core/api/organization/management'
import { OrganizationInvitationsGateway } from '@/core/api/organization/invitations'
import { OrganizationMembersGateway } from '@/core/api/organization/members'
import { OrganizationRolesGateway } from '@/core/api/organization/roles'

export const ApiGateway = {
  organization: {
    invitations: OrganizationInvitationsGateway,
    management: OrganizationManagementGateway,
    members: OrganizationMembersGateway,
    roles: OrganizationRolesGateway,
  },
  user: {
    session: SessionGateway,
    users: UsersGateway,
  },
} as const
