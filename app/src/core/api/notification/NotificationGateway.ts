import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import { isApiErrorResponse, isApiSuccessResponse, type ApiMeta } from '@/core/api/global/types/response'
import type { NotificationData, ApprovalData, ApprovalCommentData } from './notification.types'

interface PaginatedMeta {
  pagination?: { nextCursor?: string | null; hasMore?: boolean }
}

interface NotificationsApiResponse {
  data?: NotificationData[]
  error?: { code: string; message: string }
  meta?: ApiMeta & PaginatedMeta
}

interface ApprovalsApiResponse {
  data?: ApprovalData[]
  error?: { code: string; message: string }
  meta?: ApiMeta & PaginatedMeta
}

interface ApprovalApiResponse {
  data?: ApprovalData
  error?: { code: string; message: string }
  meta?: ApiMeta
}

interface ApprovalCommentApiResponse {
  data?: ApprovalCommentData
  error?: { code: string; message: string }
  meta?: ApiMeta
}

export class NotificationGateway {
  static async listNotifications(params: {
    limit?: number
    cursor?: string | null
    status?: 'all' | 'read' | 'unread'
  } = {}): Promise<{ data: NotificationData[]; nextCursor: string | null }> {
    const searchParams = new URLSearchParams()
    if (params.limit) searchParams.set('limit', String(params.limit))
    if (params.cursor) searchParams.set('cursor', params.cursor)
    if (params.status) searchParams.set('status', params.status)

    const response = await apiClient.get<NotificationsApiResponse>(
      ApiRoutes.Notification.List,
      { params: {}, paramsSerializer: () => searchParams.toString() }
    )
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload)) {
      throw new ApiError('Notification.List.InvalidResponse', { message: 'Notifications list response is invalid' })
    }

    return {
      data: Array.isArray(payload.data) ? payload.data : [],
      nextCursor: payload.meta?.pagination?.nextCursor ?? null,
    }
  }

  static async listApprovals(params: {
    limit?: number
    cursor?: string | null
  } = {}): Promise<{ data: ApprovalData[]; nextCursor: string | null }> {
    const searchParams = new URLSearchParams()
    if (params.limit) searchParams.set('limit', String(params.limit))
    if (params.cursor) searchParams.set('cursor', params.cursor)

    const response = await apiClient.get<ApprovalsApiResponse>(
      ApiRoutes.Approval.List,
      { params: {}, paramsSerializer: () => searchParams.toString() }
    )
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload)) {
      throw new ApiError('Approval.List.InvalidResponse', { message: 'Approvals list response is invalid' })
    }

    return {
      data: Array.isArray(payload.data) ? payload.data : [],
      nextCursor: payload.meta?.pagination?.nextCursor ?? null,
    }
  }

  static async markAsRead(notificationId: string): Promise<void> {
    const response = await apiClient.patch(ApiRoutes.Notification.MarkAsRead(notificationId))
    const payload = response.data

    if (payload && isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
      })
    }
  }

  static async markAllAsRead(): Promise<void> {
    const response = await apiClient.patch(ApiRoutes.Notification.MarkAllAsRead)
    const payload = response.data

    if (payload && isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
      })
    }
  }

  static async updateApprovalStatus(
    approvalId: string,
    status: 'approved' | 'rejected'
  ): Promise<ApprovalData> {
    const response = await apiClient.patch<ApprovalApiResponse>(
      ApiRoutes.Approval.Update(approvalId),
      { status }
    )
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data) {
      throw new ApiError('Approval.Update.InvalidResponse', { message: 'Update approval response is invalid' })
    }

    return payload.data
  }

  static async addApprovalComment(
    approvalId: string,
    content: string
  ): Promise<ApprovalCommentData> {
    const response = await apiClient.post<ApprovalCommentApiResponse>(
      ApiRoutes.Approval.AddComment(approvalId),
      { content }
    )
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data) {
      throw new ApiError('Approval.AddComment.InvalidResponse', { message: 'Add approval comment response is invalid' })
    }

    return payload.data
  }
}
