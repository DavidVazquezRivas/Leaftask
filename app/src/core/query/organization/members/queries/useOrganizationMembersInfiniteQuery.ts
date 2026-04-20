import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type {
  GetOrganizationMembersParams,
  GetOrganizationMembersSuccessResponse,
} from '@/core/api/organization/members'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

interface UseOrganizationMembersInfiniteQueryParams {
  limit?: number
}

export const useOrganizationMembersInfiniteQuery = (
  organizationId: string | null,
  params: UseOrganizationMembersInfiniteQueryParams = {}
) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.organization.members.list(organizationId ?? '', params),
    queryFn: async (): Promise<GetOrganizationMembersSuccessResponse> => {
      const members: GetOrganizationMembersSuccessResponse['data'] = []
      let cursor: string | null = null
      let lastMeta: GetOrganizationMembersSuccessResponse['meta']

      do {
        const requestParams: GetOrganizationMembersParams = {
          limit: params.limit,
          cursor,
        }

        const response = await ApiGateway.organization.members.getMembers(
          organizationId as string,
          requestParams
        )

        members.push(...response.data)
        lastMeta = response.meta
        cursor = response.meta?.pagination?.nextCursor ?? null
      } while (cursor)

      return {
        data: members,
        meta: lastMeta,
      }
    },
    enabled: Boolean(accessToken && organizationId),
  })
}
