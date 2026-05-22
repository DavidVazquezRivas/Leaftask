import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type {
  GetOrganizationManagementParams,
  GetOrganizationManagementSuccessResponse,
  OrganizationManagementSortParam,
} from '@/core/api/organization/management'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export interface UseOrganizationManagementInfiniteQueryParams {
  limit?: number
  sort?: OrganizationManagementSortParam[]
}

export const useOrganizationManagementInfiniteQuery = (
  params: UseOrganizationManagementInfiniteQueryParams = {}
) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.organization.management.list(params),
    queryFn: async (): Promise<GetOrganizationManagementSuccessResponse> => {
      const organizations: GetOrganizationManagementSuccessResponse['data'] = []
      let cursor: string | null = null
      let lastMeta: GetOrganizationManagementSuccessResponse['meta']

      do {
        const requestParams: GetOrganizationManagementParams = {
          limit: params.limit,
          sort: params.sort,
          cursor,
        }

        const response =
          await ApiGateway.organization.management.getOrganizations(
            requestParams
          )

        organizations.push(...response.data)
        lastMeta = response.meta
        cursor = response.meta?.pagination?.nextCursor ?? null
      } while (cursor)

      return {
        data: organizations,
        meta: lastMeta,
      }
    },
    enabled: Boolean(accessToken),
  })
}
