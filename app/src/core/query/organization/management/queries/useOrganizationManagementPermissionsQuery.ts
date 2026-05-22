import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useOrganizationManagementPermissionsQuery = (
  organizationId: string | null
) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.organization.management.permissions.me(
      organizationId ?? ''
    ),
    queryFn: async () => {
      return ApiGateway.organization.management.getOrganizationPermissions(
        organizationId as string
      )
    },
    enabled: Boolean(accessToken && organizationId),
  })
}
