import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type {
  GetProjectsParams,
  GetProjectsSuccessResponse,
} from '@/core/api/project/management'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export interface UseMyProjectsQueryParams {
  limit?: number
  sort?: string[]
}

export const useMyProjectsQuery = (params: UseMyProjectsQueryParams = {}) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.project.management.myProjects(params),
    queryFn: async (): Promise<GetProjectsSuccessResponse> => {
      const projects: GetProjectsSuccessResponse['data'] = []
      let cursor: string | null = null
      let lastMeta: GetProjectsSuccessResponse['meta']

      do {
        const requestParams: GetProjectsParams = {
          limit: params.limit,
          sort: params.sort,
          cursor,
        }

        const response =
          await ApiGateway.project.management.getMyProjects(requestParams)

        projects.push(...response.data)
        lastMeta = response.meta
        cursor = response.meta?.pagination?.nextCursor ?? null
      } while (cursor)

      return { data: projects, meta: lastMeta }
    },
    enabled: Boolean(accessToken),
  })
}
