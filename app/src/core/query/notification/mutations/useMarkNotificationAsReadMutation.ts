import { useMutation } from '@tanstack/react-query'

import { ApiGateway } from '@/core/api/ApiGateway'
import { QueryKeys } from '@/core/query/QueryKeys'
import { queryClient } from '@/core/query/queryClient'

export const useMarkNotificationAsReadMutation = () =>
  useMutation({
    mutationFn: (notificationId: string) =>
      ApiGateway.notification.markAsRead(notificationId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QueryKeys.notification.all })
    },
  })
