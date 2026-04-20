import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useOrganizationRolesQuery = (organizationId: string | null) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.organization.roles.list(organizationId ?? ''),
    queryFn: async () => {
      return ApiGateway.organization.roles.getRoles(organizationId as string)
    },
    enabled: Boolean(accessToken && organizationId),
  })
}
