import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useOrganizationRolesPermissionsQuery = () => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.organization.roles.permissions.list(),
    queryFn: async () => {
      return ApiGateway.organization.roles.getPermissions()
    },
    enabled: Boolean(accessToken),
  })
}
