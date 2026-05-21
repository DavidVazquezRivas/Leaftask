import { useMutation } from '@tanstack/react-query'
import { ApiGateway } from '@/core/api/ApiGateway'
import type { AddCommentRequest } from '@/core/api/workitems'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useAddCommentMutation = (projectId: string, itemId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.workItem.comments.all, 'create', projectId, itemId],
    mutationFn: (payload: AddCommentRequest) =>
      ApiGateway.workItem.comments.create(projectId, itemId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.comments.list(projectId, itemId),
      })
    },
    onError: (error) => handleApiError(error),
  })
}
