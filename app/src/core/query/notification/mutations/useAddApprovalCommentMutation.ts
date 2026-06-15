import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useAddApprovalCommentMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationFn: ({ approvalId, content }: { approvalId: string; content: string }) =>
      ApiGateway.notification.addApprovalComment(approvalId, content),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QueryKeys.approval.list() })
    },
    onError: (error) => handleApiError(error),
  })
}
