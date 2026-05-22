import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useAuthStore } from '@/core/zustand/auth/authStore'

export const useSessionMeQuery = () => {
  const accessToken = useAuthStore((state) => state.accessToken)

  return useQuery({
    queryKey: QueryKeys.user.session.me(),
    queryFn: async () => {
      return ApiGateway.user.session.getCurrentUser()
    },
    enabled: Boolean(accessToken),
    retry: false,
  })
}
