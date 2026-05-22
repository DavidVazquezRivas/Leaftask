import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type {
  GetUsersParams,
  GetUsersSuccessResponse,
} from '@/core/api/user/users'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

interface UseUsersInfiniteQueryParams {
  limit?: number
  search?: string
}

export const useUsersInfiniteQuery = (
  params: UseUsersInfiniteQueryParams = {},
  enabled = true
) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.user.users.list(params),
    queryFn: async (): Promise<GetUsersSuccessResponse> => {
      const users: GetUsersSuccessResponse['data'] = []
      let cursor: string | null = null
      let lastMeta: GetUsersSuccessResponse['meta']

      do {
        const requestParams: GetUsersParams = {
          limit: params.limit,
          cursor,
          search: params.search,
        }

        const response = await ApiGateway.user.users.getUsers(requestParams)

        users.push(...response.data)
        lastMeta = response.meta
        cursor = response.meta?.pagination?.nextCursor ?? null
      } while (cursor)

      return {
        data: users,
        meta: lastMeta,
      }
    },
    enabled: Boolean(accessToken && enabled),
  })
}
