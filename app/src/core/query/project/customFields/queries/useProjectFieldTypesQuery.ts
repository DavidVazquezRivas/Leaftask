import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useProjectFieldTypesQuery = () => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.project.customFields.fieldTypes,
    queryFn: () => ApiGateway.project.customFields.getFieldTypes(),
    enabled: Boolean(accessToken),
    staleTime: 5 * 60 * 1000,
  })
}
