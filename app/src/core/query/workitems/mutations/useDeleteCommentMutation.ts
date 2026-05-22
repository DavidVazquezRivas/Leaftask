import { useMutation } from '@tanstack/react-query'
import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useDeleteCommentMutation = (projectId: string, itemId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [...QueryKeys.workItem.comments.all, 'delete', projectId, itemId],
    mutationFn: (commentId: string) =>
      ApiGateway.workItem.comments.delete(projectId, itemId, commentId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: QueryKeys.workItem.comments.list(projectId, itemId),
      })
    },
    onError: (error) => handleApiError(error),
  })
}
