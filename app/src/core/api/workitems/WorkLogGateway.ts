import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  GetWorkLogsApiResponse,
  GetWorkLogsSuccessResponse,
  LogWorkApiResponse,
  LogWorkRequest,
  LogWorkSuccessResponse,
  UpdateWorkLogApiResponse,
  UpdateWorkLogRequest,
  UpdateWorkLogSuccessResponse,
} from './workitems.types'

export class WorkLogGateway {
  static async getWorkLogs(
    projectId: string,
    itemId: string,
    params: { limit?: number; cursor?: string | null } = {}
  ): Promise<GetWorkLogsSuccessResponse> {
    const searchParams = new URLSearchParams()
    if (params.limit) searchParams.set('limit', String(params.limit))
    if (params.cursor) searchParams.set('cursor', params.cursor)
    searchParams.append('sort', 'date:desc')

    const response = await apiClient.get<GetWorkLogsApiResponse>(
      ApiRoutes.WorkItem.WorkLogs.List(projectId, itemId),
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
      throw new ApiError('WorkLogs.List.InvalidResponse', {
        message: 'Work logs list response is invalid',
      })
    }

    return payload
  }

  static async logWork(
    projectId: string,
    itemId: string,
    payload: LogWorkRequest
  ): Promise<LogWorkSuccessResponse> {
    const response = await apiClient.post<LogWorkApiResponse>(
      ApiRoutes.WorkItem.WorkLogs.Create(projectId, itemId),
      payload
    )

    const responsePayload = response.data

    if (isApiErrorResponse(responsePayload)) {
      throw new ApiError(responsePayload.error.code, {
        message: responsePayload.error.message,
        status: response.status,
        meta: responsePayload.meta,
      })
    }

    if (!isApiSuccessResponse(responsePayload)) {
      throw new ApiError('WorkLogs.Create.InvalidResponse', {
        message: 'Log work response is invalid',
      })
    }

    return responsePayload
  }

  static async updateWorkLog(
    projectId: string,
    itemId: string,
    logId: string,
    payload: UpdateWorkLogRequest
  ): Promise<UpdateWorkLogSuccessResponse> {
    const response = await apiClient.patch<UpdateWorkLogApiResponse>(
      ApiRoutes.WorkItem.WorkLogs.Update(projectId, itemId, logId),
      payload
    )

    const responsePayload = response.data

    if (isApiErrorResponse(responsePayload)) {
      throw new ApiError(responsePayload.error.code, {
        message: responsePayload.error.message,
        status: response.status,
        meta: responsePayload.meta,
      })
    }

    if (!isApiSuccessResponse(responsePayload)) {
      throw new ApiError('WorkLogs.Update.InvalidResponse', {
        message: 'Update work log response is invalid',
      })
    }

    return responsePayload
  }

  static async deleteWorkLog(
    projectId: string,
    itemId: string,
    logId: string
  ): Promise<void> {
    const response = await apiClient.delete(
      ApiRoutes.WorkItem.WorkLogs.Delete(projectId, itemId, logId)
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
