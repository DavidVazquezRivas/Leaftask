import { isAxiosError } from 'axios'
import { useMutation } from '@tanstack/react-query'
import { toast } from 'sonner'

import { ApiGateway } from '@/core/api/ApiGateway'
import { ApiError } from '@/core/api/global/errors/ApiError'
import { i18n } from '@/core/i18n'
import { QueryKeys } from '@/core/query/QueryKeys'
import { useApiErrorHandler } from '@/core/query/hooks'
import { queryClient } from '@/core/query/queryClient'

const isForbiddenError = (error: unknown): boolean => {
  if (error instanceof ApiError) {
    return error.status === 403
  }

  return isAxiosError(error) && error.response?.status === 403
}

const isOwnerProtectionError = (error: unknown): boolean => {
  if (!(error instanceof ApiError)) {
    return false
  }

  const code = error.code.toLowerCase()
  return code.includes('owner')
}

export const useDeleteOrganizationMemberMutation = (organizationId: string) => {
  const handleApiError = useApiErrorHandler()

  return useMutation({
    mutationKey: [
      ...QueryKeys.organization.members.all,
      'delete',
      organizationId,
    ],
    mutationFn: async (memberId: string) => {
      return ApiGateway.organization.members.deleteMember(
        organizationId,
        memberId
      )
    },
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: QueryKeys.organization.members.all,
        }),
        queryClient.invalidateQueries({
          queryKey: QueryKeys.organization.members.distribution.all,
        }),
      ])

      toast.success(
        i18n.t('management.settings.members.feedback.memberDeleted', {
          ns: 'organizations',
          defaultValue: 'Member removed successfully.',
        })
      )
    },
    onError: (error) => {
      if (isOwnerProtectionError(error)) {
        toast.error(
          i18n.t('management.settings.members.feedback.ownerCannotBeRemoved', {
            ns: 'organizations',
            defaultValue: 'Owner members cannot be removed.',
          })
        )
        return
      }

      if (isForbiddenError(error)) {
        toast.info(
          i18n.t('management.settings.members.permissions.noRemove', {
            ns: 'organizations',
            defaultValue: "You don't have permission to remove members.",
          })
        )
        return
      }

      handleApiError(error)
    },
  })
}
