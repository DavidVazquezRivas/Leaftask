import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useProjectDetailQuery = (projectId: string | null) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.project.management.detail(projectId ?? ''),
    queryFn: () =>
      ApiGateway.project.management.getProjectById(projectId as string),
    enabled: Boolean(accessToken && projectId),
  })
}
