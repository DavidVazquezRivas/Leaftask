import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type {
  GetProjectMembersParams,
  GetProjectMembersSuccessResponse,
} from '@/core/api/project/members'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

interface UseProjectMembersQueryParams {
  limit?: number
}

export const useProjectMembersQuery = (
  projectId: string | null,
  params: UseProjectMembersQueryParams = {}
) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.project.members.list(projectId ?? '', params),
    queryFn: async (): Promise<GetProjectMembersSuccessResponse> => {
      const members: GetProjectMembersSuccessResponse['data'] = []
      let cursor: string | null = null
      let lastMeta: GetProjectMembersSuccessResponse['meta']

      do {
        const requestParams: GetProjectMembersParams = {
          limit: params.limit,
          cursor,
        }

        const response = await ApiGateway.project.members.getMembers(
          projectId as string,
          requestParams
        )

        members.push(...response.data)
        lastMeta = response.meta
        cursor = response.meta?.pagination?.nextCursor ?? null
      } while (cursor)

      return { data: members, meta: lastMeta }
    },
    enabled: Boolean(accessToken && projectId),
  })
}
