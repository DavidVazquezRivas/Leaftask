import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import { isApiErrorResponse, isApiSuccessResponse } from '@/core/api/global/types/response'
import type { WorkItemCommentData } from './workitems.types'

interface CommentApiResponse {
  data?: WorkItemCommentData
  error?: { code: string; message: string }
  meta?: unknown
}

interface CommentsListApiResponse {
  data?: WorkItemCommentData[]
  error?: { code: string; message: string }
  meta?: { nextCursor?: string | null }
}

export interface AddCommentRequest {
  content: string
  attachmentIds?: string[]
}

export interface UpdateCommentRequest {
  content?: string
  attachmentIds?: string[]
}

export class CommentGateway {
  static async list(
    projectId: string,
    itemId: string,
    params: { limit?: number; cursor?: string | null } = {}
  ): Promise<{ data: WorkItemCommentData[]; nextCursor: string | null }> {
    const searchParams = new URLSearchParams()
    if (params.limit) searchParams.set('limit', String(params.limit))
    if (params.cursor) searchParams.set('cursor', params.cursor)
    searchParams.append('sort', 'createdAt:asc')

    const response = await apiClient.get<CommentsListApiResponse>(
      ApiRoutes.WorkItem.Comments.List(projectId, itemId),
      { params, paramsSerializer: () => searchParams.toString() }
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
      throw new ApiError('Comments.List.InvalidResponse', { message: 'Comments list response is invalid' })
    }

    return {
      data: payload.data ?? [],
      nextCursor: (payload.meta as { nextCursor?: string | null })?.nextCursor ?? null,
    }
  }

  static async create(
    projectId: string,
    itemId: string,
    body: AddCommentRequest
  ): Promise<WorkItemCommentData> {
    const response = await apiClient.post<CommentApiResponse>(
      ApiRoutes.WorkItem.Comments.Create(projectId, itemId),
      body
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
      throw new ApiError('Comments.Create.InvalidResponse', { message: 'Create comment response is invalid' })
    }

    return payload.data
  }

  static async update(
    projectId: string,
    itemId: string,
    commentId: string,
    body: UpdateCommentRequest
  ): Promise<WorkItemCommentData> {
    const response = await apiClient.patch<CommentApiResponse>(
      ApiRoutes.WorkItem.Comments.Update(projectId, itemId, commentId),
      body
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
      throw new ApiError('Comments.Update.InvalidResponse', { message: 'Update comment response is invalid' })
    }

    return payload.data
  }

  static async delete(
    projectId: string,
    itemId: string,
    commentId: string
  ): Promise<void> {
    const response = await apiClient.delete(
      ApiRoutes.WorkItem.Comments.Delete(projectId, itemId, commentId)
    )

    const payload = response.data

    if (payload && isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }
  }
}
