import { SessionGateway } from '@/core/api/user/session'
import { OrganizationManagementGateway } from '@/core/api/organization/management'

export const ApiGateway = {
  organization: {
    management: OrganizationManagementGateway,
  },
  user: {
    session: SessionGateway,
  },
} as const
