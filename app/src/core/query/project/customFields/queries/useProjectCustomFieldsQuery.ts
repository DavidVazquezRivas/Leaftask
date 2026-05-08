import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useProjectCustomFieldsQuery = (projectId: string | null) => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.project.customFields.list(projectId ?? ''),
    queryFn: () =>
      ApiGateway.project.customFields.getFields(projectId as string),
    enabled: Boolean(accessToken && projectId),
  })
}
