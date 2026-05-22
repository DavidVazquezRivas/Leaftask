import { ApiRoutes } from '@/core/api/global/constants/routes'
import { ApiError } from '@/core/api/global/errors'
import { apiClient } from '@/core/api/global/httpClient'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  CreateCustomFieldApiResponse,
  CreateCustomFieldRequest,
  CreateCustomFieldSuccessResponse,
  DeleteCustomFieldApiResponse,
  DeleteCustomFieldSuccessResponse,
  GetCustomFieldsApiResponse,
  GetCustomFieldsSuccessResponse,
  GetFieldTypesApiResponse,
  GetFieldTypesSuccessResponse,
  PatchCustomFieldApiResponse,
  PatchCustomFieldRequest,
  PatchCustomFieldSuccessResponse,
} from './projectCustomFields.types'

export class ProjectCustomFieldsGateway {
  static async getFieldTypes(): Promise<GetFieldTypesSuccessResponse> {
    const response = await apiClient.get<GetFieldTypesApiResponse>(
      ApiRoutes.Project.CustomFields.FieldTypes
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
      throw new ApiError('ProjectCustomFields.FieldTypes.InvalidResponse', {
        message: 'Field types response is invalid',
      })
    }

    return payload
  }

  static async getFields(
    projectId: string
  ): Promise<GetCustomFieldsSuccessResponse> {
    const response = await apiClient.get<GetCustomFieldsApiResponse>(
      ApiRoutes.Project.CustomFields.List(projectId)
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
      throw new ApiError('ProjectCustomFields.List.InvalidResponse', {
        message: 'Custom fields list response is invalid',
      })
    }

    return payload
  }

  static async createField(
    projectId: string,
    payload: CreateCustomFieldRequest
  ): Promise<CreateCustomFieldSuccessResponse> {
    const response = await apiClient.post<CreateCustomFieldApiResponse>(
      ApiRoutes.Project.CustomFields.Create(projectId),
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
      throw new ApiError('ProjectCustomFields.Create.InvalidResponse', {
        message: 'Create custom field response is invalid',
      })
    }

    return responsePayload
  }

  static async patchField(
    projectId: string,
    fieldId: string,
    payload: PatchCustomFieldRequest
  ): Promise<PatchCustomFieldSuccessResponse> {
    const response = await apiClient.patch<PatchCustomFieldApiResponse>(
      ApiRoutes.Project.CustomFields.Update(projectId, fieldId),
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
      throw new ApiError('ProjectCustomFields.Update.InvalidResponse', {
        message: 'Patch custom field response is invalid',
      })
    }

    return responsePayload
  }

  static async deleteField(
    projectId: string,
    fieldId: string
  ): Promise<DeleteCustomFieldSuccessResponse> {
    const response = await apiClient.delete<DeleteCustomFieldApiResponse>(
      ApiRoutes.Project.CustomFields.Delete(projectId, fieldId)
    )

    const rawPayload = response.data as unknown

    if (
      response.status === 204 ||
      rawPayload === null ||
      rawPayload === undefined ||
      (typeof rawPayload === 'string' && rawPayload.trim().length === 0)
    ) {
      return { data: null }
    }

    if (isApiErrorResponse(rawPayload)) {
      throw new ApiError(rawPayload.error.code, {
        message: rawPayload.error.message,
        status: response.status,
        meta: rawPayload.meta,
      })
    }

    if (!isApiSuccessResponse<null>(rawPayload)) {
      throw new ApiError('ProjectCustomFields.Delete.InvalidResponse', {
        message: 'Delete custom field response is invalid',
      })
    }

    return rawPayload
  }
}
