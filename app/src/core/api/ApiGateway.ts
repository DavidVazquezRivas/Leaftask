import { SessionGateway } from '@/core/api/user/session'
import { UsersGateway } from '@/core/api/user/users'
import { OrganizationManagementGateway } from '@/core/api/organization/management'
import { OrganizationInvitationsGateway } from '@/core/api/organization/invitations'
import { OrganizationMembersGateway } from '@/core/api/organization/members'
import { OrganizationRolesGateway } from '@/core/api/organization/roles'
import { ProjectCustomFieldsGateway } from '@/core/api/project/customFields'
import { ProjectManagementGateway } from '@/core/api/project/management'
import { ProjectMembersGateway } from '@/core/api/project/members'
import { ProjectRolesGateway } from '@/core/api/project/roles'
import { WorkItemsGateway, WorkLogGateway, AttachmentGateway } from '@/core/api/workitems'

export const ApiGateway = {
  organization: {
    invitations: OrganizationInvitationsGateway,
    management: OrganizationManagementGateway,
    members: OrganizationMembersGateway,
    roles: OrganizationRolesGateway,
  },
  project: {
    customFields: ProjectCustomFieldsGateway,
    management: ProjectManagementGateway,
    members: ProjectMembersGateway,
    roles: ProjectRolesGateway,
  },
  workItem: {
    management: WorkItemsGateway,
    workLogs: WorkLogGateway,
    attachments: AttachmentGateway,
  },
  user: {
    session: SessionGateway,
    users: UsersGateway,
  },
} as const
