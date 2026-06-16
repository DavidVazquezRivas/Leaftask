import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import type { CreateChatRequest } from '@/core/api/chat'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useCreateChatMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationFn: (payload: CreateChatRequest) => ApiGateway.chat.create(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QueryKeys.chat.list() })
    },
    onError: (error) => handleApiError(error),
  })
}
