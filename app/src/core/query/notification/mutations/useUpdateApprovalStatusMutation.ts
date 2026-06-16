import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

export const useUpdateApprovalStatusMutation = () => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationFn: ({ approvalId, status }: { approvalId: string; status: 'approved' | 'rejected' }) =>
      ApiGateway.notification.updateApprovalStatus(approvalId, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QueryKeys.approval.list() })
    },
    onError: (error) => handleApiError(error),
  })
}
