import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  CreateWorkItemApiResponse,
  CreateWorkItemRequest,
  CreateWorkItemSuccessResponse,
  GetWorkItemDetailApiResponse,
  GetWorkItemDetailSuccessResponse,
  GetWorkItemsApiResponse,
  GetWorkItemsParams,
  GetWorkItemsSuccessResponse,
  GetWorkItemStatusesApiResponse,
  GetWorkItemStatusesSuccessResponse,
  GetWorkItemTypesApiResponse,
  GetWorkItemTypesSuccessResponse,
  UpdateWorkItemApiResponse,
  UpdateWorkItemRequest,
  UpdateWorkItemSuccessResponse,
} from './workitems.types'

const serializeParams = (params: GetWorkItemsParams): string => {
  const searchParams = new URLSearchParams()

  if (typeof params.limit === 'number') {
    searchParams.set('limit', String(params.limit))
  }

  if (params.cursor) {
    searchParams.set('cursor', params.cursor)
  }

  params.sort?.forEach((s) => searchParams.append('sort', s))

  return searchParams.toString()
}

export class WorkItemsGateway {
  static async getProjectWorkItems(
    projectId: string,
    params: GetWorkItemsParams = {}
  ): Promise<GetWorkItemsSuccessResponse> {
    const response = await apiClient.get<GetWorkItemsApiResponse>(
      ApiRoutes.WorkItem.Management.List(projectId),
      {
        params,
        paramsSerializer: () => serializeParams(params),
      }
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
      throw new ApiError('WorkItems.List.InvalidResponse', {
        message: 'Work items list response is invalid',
      })
    }

    return payload
  }

  static async getWorkItemDetail(
    projectId: string,
    itemId: string
  ): Promise<GetWorkItemDetailSuccessResponse> {
    const response = await apiClient.get<GetWorkItemDetailApiResponse>(
      ApiRoutes.WorkItem.Management.Detail(projectId, itemId)
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
      throw new ApiError('WorkItems.Detail.InvalidResponse', {
        message: 'Work item detail response is invalid',
      })
    }

    return payload
  }

  static async updateWorkItem(
    projectId: string,
    itemId: string,
    payload: UpdateWorkItemRequest
  ): Promise<UpdateWorkItemSuccessResponse> {
    const response = await apiClient.patch<UpdateWorkItemApiResponse>(
      ApiRoutes.WorkItem.Management.Update(projectId, itemId),
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
      throw new ApiError('WorkItems.Update.InvalidResponse', {
        message: 'Update work item response is invalid',
      })
    }

    return responsePayload
  }

  static async getWorkItemTypes(): Promise<GetWorkItemTypesSuccessResponse> {
    const response = await apiClient.get<GetWorkItemTypesApiResponse>(
      ApiRoutes.WorkItem.Configuration.Types
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
      throw new ApiError('WorkItems.Types.InvalidResponse', {
        message: 'Work item types response is invalid',
      })
    }

    return payload
  }

  static async getWorkItemStatuses(): Promise<GetWorkItemStatusesSuccessResponse> {
    const response = await apiClient.get<GetWorkItemStatusesApiResponse>(
      ApiRoutes.WorkItem.Configuration.Statuses
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
      throw new ApiError('WorkItems.Statuses.InvalidResponse', {
        message: 'Work item statuses response is invalid',
      })
    }

    return payload
  }

  static async deleteWorkItem(
    projectId: string,
    itemId: string
  ): Promise<void> {
    const response = await apiClient.delete(
      ApiRoutes.WorkItem.Management.Delete(projectId, itemId)
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

  static async createWorkItem(
    projectId: string,
    payload: CreateWorkItemRequest
  ): Promise<CreateWorkItemSuccessResponse> {
    const response = await apiClient.post<CreateWorkItemApiResponse>(
      ApiRoutes.WorkItem.Management.Create(projectId),
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
      throw new ApiError('WorkItems.Create.InvalidResponse', {
        message: 'Create work item response is invalid',
      })
    }

    return responsePayload
  }
}
