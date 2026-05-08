import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import { apiClient } from '@/core/api/global/httpClient'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  CreateProjectRoleApiResponse,
  CreateProjectRoleRequest,
  CreateProjectRoleSuccessResponse,
  DeleteProjectRoleApiResponse,
  DeleteProjectRoleSuccessResponse,
  GetProjectRolesApiResponse,
  GetProjectRolesPermissionsApiResponse,
  GetProjectRolesPermissionsSuccessResponse,
  GetProjectRolesSuccessResponse,
  PatchProjectRoleApiResponse,
  PatchProjectRoleRequest,
  PatchProjectRoleSuccessResponse,
} from './projectRoles.types'

export class ProjectRolesGateway {
  static async getRoles(
    projectId: string
  ): Promise<GetProjectRolesSuccessResponse> {
    const response = await apiClient.get<GetProjectRolesApiResponse>(
      ApiRoutes.Project.Roles.List(projectId)
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
      throw new ApiError('ProjectRoles.List.InvalidResponse', {
        message: 'Project roles response is invalid',
      })
    }

    return payloadResponse
  }

  static async getPermissions(
    projectId: string
  ): Promise<GetProjectRolesPermissionsSuccessResponse> {
    const response =
      await apiClient.get<GetProjectRolesPermissionsApiResponse>(
        ApiRoutes.Project.Roles.PermissionsList(projectId)
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
      throw new ApiError('ProjectRoles.Permissions.InvalidResponse', {
        message: 'Project roles permissions response is invalid',
      })
    }

    return payloadResponse
  }

  static async createRole(
    projectId: string,
    payload: CreateProjectRoleRequest
  ): Promise<CreateProjectRoleSuccessResponse> {
    const body = {
      name: payload.name,
      permissions: payload.permissions.map((p) => ({
        id: p.permissionId,
        level: p.level,
      })),
    }

    const response = await apiClient.post<CreateProjectRoleApiResponse>(
      ApiRoutes.Project.Roles.Create(projectId),
      body
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
      throw new ApiError('ProjectRoles.Create.InvalidResponse', {
        message: 'Project role create response is invalid',
      })
    }

    return payloadResponse
  }

  static async updateRole(
    projectId: string,
    roleId: string,
    payload: PatchProjectRoleRequest
  ): Promise<PatchProjectRoleSuccessResponse> {
    const body = {
      name: payload.name,
      permissions: payload.permissions?.map((p) => ({
        id: p.permissionId,
        level: p.level,
      })),
    }

    const response = await apiClient.patch<PatchProjectRoleApiResponse>(
      ApiRoutes.Project.Roles.Update(projectId, roleId),
      body
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
      throw new ApiError('ProjectRoles.Update.InvalidResponse', {
        message: 'Project role update response is invalid',
      })
    }

    return payloadResponse
  }

  static async deleteRole(
    projectId: string,
    roleId: string
  ): Promise<DeleteProjectRoleSuccessResponse> {
    const response = await apiClient.delete<DeleteProjectRoleApiResponse>(
      ApiRoutes.Project.Roles.Delete(projectId, roleId)
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

    const payloadResponse = rawPayload

    if (isApiErrorResponse(payloadResponse)) {
      throw new ApiError(payloadResponse.error.code, {
        message: payloadResponse.error.message,
        status: response.status,
        meta: payloadResponse.meta,
      })
    }

    if (!isApiSuccessResponse<null>(payloadResponse)) {
      throw new ApiError('ProjectRoles.Delete.InvalidResponse', {
        message: 'Project role delete response is invalid',
      })
    }

    return payloadResponse
  }
}
