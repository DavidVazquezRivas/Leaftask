import { SessionGateway } from '@/core/api/user/session'

export const ApiGateway = {
  user: {
    session: SessionGateway,
  },
} as const
