import { useMutation } from '@tanstack/react-query'
import { ApiGateway } from '@/core/api/ApiGateway'
import type { UpdateCommentRequest } from '@/core/api/workitems'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useUpdateCommentMutation = (projectId: string, itemId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.workItem.comments.all, 'update', projectId, itemId],
    mutationFn: ({ commentId, ...body }: UpdateCommentRequest & { commentId: string }) =>
      ApiGateway.workItem.comments.update(projectId, itemId, commentId, body),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.comments.list(projectId, itemId),
      })
    },
    onError: (error) => handleApiError(error),
  })
}
