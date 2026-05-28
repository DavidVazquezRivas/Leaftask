import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'

import { ApiGateway } from '@/core/api/ApiGateway'
import { AppPaths } from '@/core/router/paths'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useDeleteChatMutation = () => {
  const handleApiError = useApiErrorHandler()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (chatId: string) => ApiGateway.chat.delete(chatId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QueryKeys.chat.list() })
      navigate(AppPaths.chat())
    },
    onError: (error) => handleApiError(error),
  })
}
