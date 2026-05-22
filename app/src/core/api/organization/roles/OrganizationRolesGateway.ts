import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import { apiClient } from '@/core/api/global/httpClient'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  CreateOrganizationRoleApiResponse,
  CreateOrganizationRoleRequest,
  CreateOrganizationRoleSuccessResponse,
  DeleteOrganizationRoleApiResponse,
  DeleteOrganizationRoleSuccessResponse,
  GetOrganizationRolesApiResponse,
  GetOrganizationRolesPermissionsApiResponse,
  GetOrganizationRolesPermissionsSuccessResponse,
  GetOrganizationRolesSuccessResponse,
  PatchOrganizationRoleApiResponse,
  PatchOrganizationRoleRequest,
  PatchOrganizationRoleSuccessResponse,
} from '@/core/api/organization/roles/organizationRoles.types'

export class OrganizationRolesGateway {
  static async getRoles(
    organizationId: string
  ): Promise<GetOrganizationRolesSuccessResponse> {
    const response = await apiClient.get<GetOrganizationRolesApiResponse>(
      ApiRoutes.Organization.Roles.List(organizationId)
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
      throw new ApiError('OrganizationRoles.List.InvalidResponse', {
        message: 'Organization roles response is invalid',
      })
    }

    return payloadResponse
  }

  static async getPermissions(): Promise<GetOrganizationRolesPermissionsSuccessResponse> {
    const response =
      await apiClient.get<GetOrganizationRolesPermissionsApiResponse>(
        ApiRoutes.Organization.Roles.PermissionsList
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
      throw new ApiError('OrganizationRoles.Permissions.InvalidResponse', {
        message: 'Organization roles permissions response is invalid',
      })
    }

    return payloadResponse
  }

  static async createRole(
    organizationId: string,
    payload: CreateOrganizationRoleRequest
  ): Promise<CreateOrganizationRoleSuccessResponse> {
    const response = await apiClient.post<CreateOrganizationRoleApiResponse>(
      ApiRoutes.Organization.Roles.Create(organizationId),
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
      throw new ApiError('OrganizationRoles.Create.InvalidResponse', {
        message: 'Organization role create response is invalid',
      })
    }

    return payloadResponse
  }

  static async updateRole(
    organizationId: string,
    roleId: string,
    payload: PatchOrganizationRoleRequest
  ): Promise<PatchOrganizationRoleSuccessResponse> {
    const response = await apiClient.patch<PatchOrganizationRoleApiResponse>(
      ApiRoutes.Organization.Roles.Update(organizationId, roleId),
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
      throw new ApiError('OrganizationRoles.Update.InvalidResponse', {
        message: 'Organization role update response is invalid',
      })
    }

    return payloadResponse
  }

  static async deleteRole(
    organizationId: string,
    roleId: string
  ): Promise<DeleteOrganizationRoleSuccessResponse> {
    const response = await apiClient.delete<DeleteOrganizationRoleApiResponse>(
      ApiRoutes.Organization.Roles.Delete(organizationId, roleId)
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
      throw new ApiError('OrganizationRoles.Delete.InvalidResponse', {
        message: 'Organization role delete response is invalid',
      })
    }

    return payloadResponse
  }
}
