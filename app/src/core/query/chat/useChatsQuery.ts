import { useQuery } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'

export const useChatsQuery = () =>
  useQuery({
    queryKey: QueryKeys.chat.list(),
    queryFn: () => ApiGateway.chat.list(),
  })
