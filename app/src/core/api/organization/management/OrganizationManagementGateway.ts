import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  CreateOrganizationManagementApiResponse,
  CreateOrganizationManagementRequest,
  CreateOrganizationManagementSuccessResponse,
  DeleteOrganizationManagementApiResponse,
  DeleteOrganizationManagementSuccessResponse,
  GetOrganizationManagementDetailApiResponse,
  GetOrganizationManagementDetailSuccessResponse,
  GetOrganizationManagementPermissionsApiResponse,
  GetOrganizationManagementPermissionsSuccessResponse,
  GetOrganizationManagementApiResponse,
  GetOrganizationManagementParams,
  GetOrganizationManagementSuccessResponse,
  PatchOrganizationManagementApiResponse,
  PatchOrganizationManagementRequest,
  PatchOrganizationManagementSuccessResponse,
} from '@/core/api/organization/management/organizationManagement.types'

const serializeOrganizationManagementParams = (
  params: GetOrganizationManagementParams
): string => {
  const searchParams = new URLSearchParams()

  if (typeof params.limit === 'number') {
    searchParams.set('limit', String(params.limit))
  }

  if (params.cursor) {
    searchParams.set('cursor', params.cursor)
  }

  params.sort?.forEach((sortItem) => {
    searchParams.append('sort', sortItem)
  })

  return searchParams.toString()
}

export class OrganizationManagementGateway {
  static async getOrganizationPermissions(
    organizationId: string
  ): Promise<GetOrganizationManagementPermissionsSuccessResponse> {
    const response =
      await apiClient.get<GetOrganizationManagementPermissionsApiResponse>(
        ApiRoutes.Organization.Management.Permissions(organizationId)
      )

    const payloadResponse = response.data

    if (isApiErrorResponse(payloadResponse)) {
      throw new ApiError(payloadResponse.error.code, {
        message: payloadResponse.error.message,
        status: response.status,
        meta: payloadResponse.meta,
      })
    }

    if (!isApiSuccessResponse(payloadResponse)) {
      throw new ApiError('OrganizationManagement.Permissions.InvalidResponse', {
        message: 'Organization management permissions response is invalid',
      })
    }

    return payloadResponse
  }

  static async deleteOrganizationById(
    organizationId: string
  ): Promise<DeleteOrganizationManagementSuccessResponse> {
    const response =
      await apiClient.delete<DeleteOrganizationManagementApiResponse>(
        ApiRoutes.Organization.Management.Delete(organizationId)
      )

    const rawPayload = response.data as unknown

    if (
      response.status === 204 ||
      rawPayload === null ||
      rawPayload === undefined ||
      (typeof rawPayload === 'string' && rawPayload.trim().length === 0)
    ) {
      return {
        data: null,
      }
    }

    const payloadResponse = rawPayload

    if (isApiErrorResponse(payloadResponse)) {
      throw new ApiError(payloadResponse.error.code, {
        message: payloadResponse.error.message,
        status: response.status,
        meta: payloadResponse.meta,
      })
    }

    if (!isApiSuccessResponse<null>(payloadResponse)) {
      throw new ApiError('OrganizationManagement.Delete.InvalidResponse', {
        message: 'Organization management delete response is invalid',
      })
    }

    return payloadResponse
  }

  static async createOrganization(
    payload: CreateOrganizationManagementRequest
  ): Promise<CreateOrganizationManagementSuccessResponse> {
    const response =
      await apiClient.post<CreateOrganizationManagementApiResponse>(
        ApiRoutes.Organization.Management.Create,
        payload
      )

    const payloadResponse = response.data

    if (isApiErrorResponse(payloadResponse)) {
      throw new ApiError(payloadResponse.error.code, {
        message: payloadResponse.error.message,
        status: response.status,
        meta: payloadResponse.meta,
      })
    }

    if (!isApiSuccessResponse(payloadResponse)) {
      throw new ApiError('OrganizationManagement.Create.InvalidResponse', {
        message: 'Organization management create response is invalid',
      })
    }

    return payloadResponse
  }

  static async getOrganizationById(
    organizationId: string
  ): Promise<GetOrganizationManagementDetailSuccessResponse> {
    const response =
      await apiClient.get<GetOrganizationManagementDetailApiResponse>(
        ApiRoutes.Organization.Management.Detail(organizationId)
      )

    const payloadResponse = response.data

    if (isApiErrorResponse(payloadResponse)) {
      throw new ApiError(payloadResponse.error.code, {
        message: payloadResponse.error.message,
        status: response.status,
        meta: payloadResponse.meta,
      })
    }

    if (!isApiSuccessResponse(payloadResponse)) {
      throw new ApiError('OrganizationManagement.Detail.InvalidResponse', {
        message: 'Organization management detail response is invalid',
      })
    }

    return payloadResponse
  }

  static async getOrganizations(
    params: GetOrganizationManagementParams = {}
  ): Promise<GetOrganizationManagementSuccessResponse> {
    const response = await apiClient.get<GetOrganizationManagementApiResponse>(
      ApiRoutes.Organization.Management.List,
      {
        params,
        paramsSerializer: () => serializeOrganizationManagementParams(params),
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
      throw new ApiError('OrganizationManagement.List.InvalidResponse', {
        message: 'Organization management list response is invalid',
      })
    }

    return payload
  }

  static async updateOrganizationById(
    organizationId: string,
    payload: PatchOrganizationManagementRequest
  ): Promise<PatchOrganizationManagementSuccessResponse> {
    const response =
      await apiClient.patch<PatchOrganizationManagementApiResponse>(
        ApiRoutes.Organization.Management.Update(organizationId),
        payload
      )

    const payloadResponse = response.data

    if (isApiErrorResponse(payloadResponse)) {
      throw new ApiError(payloadResponse.error.code, {
        message: payloadResponse.error.message,
        status: response.status,
        meta: payloadResponse.meta,
      })
    }

    if (!isApiSuccessResponse(payloadResponse)) {
      throw new ApiError('OrganizationManagement.Update.InvalidResponse', {
        message: 'Organization management update response is invalid',
      })
    }

    return payloadResponse
  }
}
