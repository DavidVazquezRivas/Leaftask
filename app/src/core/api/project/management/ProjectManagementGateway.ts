import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  CreateProjectApiResponse,
  CreateProjectRequest,
  CreateProjectSuccessResponse,
  DeleteProjectApiResponse,
  DeleteProjectSuccessResponse,
  GetProjectDetailApiResponse,
  GetProjectDetailSuccessResponse,
  GetProjectsApiResponse,
  GetProjectsParams,
  GetProjectsSuccessResponse,
  PatchProjectApiResponse,
  PatchProjectRequest,
  PatchProjectSuccessResponse,
} from '@/core/api/project/management/projectManagement.types'

const serializeProjectParams = (params: GetProjectsParams): string => {
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

export class ProjectManagementGateway {
  static async getMyProjects(
    params: GetProjectsParams = {}
  ): Promise<GetProjectsSuccessResponse> {
    const response = await apiClient.get<GetProjectsApiResponse>(
      ApiRoutes.Project.Management.Me,
      {
        params,
        paramsSerializer: () => serializeProjectParams(params),
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
      throw new ApiError('ProjectManagement.MyProjects.InvalidResponse', {
        message: 'My projects response is invalid',
      })
    }

    return payload
  }

  static async getOrganizationProjects(
    organizationId: string,
    params: GetProjectsParams = {}
  ): Promise<GetProjectsSuccessResponse> {
    const response = await apiClient.get<GetProjectsApiResponse>(
      ApiRoutes.Project.Management.Organization(organizationId),
      {
        params,
        paramsSerializer: () => serializeProjectParams(params),
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
      throw new ApiError('ProjectManagement.OrgProjects.InvalidResponse', {
        message: 'Organization projects response is invalid',
      })
    }

    return payload
  }

  static async getProjectById(
    projectId: string
  ): Promise<GetProjectDetailSuccessResponse> {
    const response = await apiClient.get<GetProjectDetailApiResponse>(
      ApiRoutes.Project.Management.Detail(projectId)
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
      throw new ApiError('ProjectManagement.Detail.InvalidResponse', {
        message: 'Project detail response is invalid',
      })
    }

    return payload
  }

  static async createProject(
    payload: CreateProjectRequest
  ): Promise<CreateProjectSuccessResponse> {
    const response = await apiClient.post<CreateProjectApiResponse>(
      ApiRoutes.Project.Management.Create,
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
      throw new ApiError('ProjectManagement.Create.InvalidResponse', {
        message: 'Create project response is invalid',
      })
    }

    return payloadResponse
  }

  static async patchProject(
    projectId: string,
    payload: PatchProjectRequest
  ): Promise<PatchProjectSuccessResponse> {
    const response = await apiClient.patch<PatchProjectApiResponse>(
      ApiRoutes.Project.Management.Update(projectId),
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
      throw new ApiError('ProjectManagement.Update.InvalidResponse', {
        message: 'Update project response is invalid',
      })
    }

    return payloadResponse
  }

  static async deleteProject(
    projectId: string
  ): Promise<DeleteProjectSuccessResponse> {
    const response = await apiClient.delete<DeleteProjectApiResponse>(
      ApiRoutes.Project.Management.Delete(projectId)
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
      throw new ApiError('ProjectManagement.Delete.InvalidResponse', {
        message: 'Delete project response is invalid',
      })
    }

    return rawPayload
  }
}
