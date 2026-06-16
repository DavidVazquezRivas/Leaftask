import { ApiRoutes } from '@/core/api/global/constants/routes'
import { ApiError } from '@/core/api/global/errors'
import { apiClient } from '@/core/api/global/httpClient'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  CreateProjectInvitationApiResponse,
  CreateProjectInvitationRequest,
  CreateProjectInvitationSuccessResponse,
  DeleteProjectMemberApiResponse,
  DeleteProjectMemberSuccessResponse,
  GetProjectMembersApiResponse,
  GetProjectMembersParams,
  GetProjectMembersSuccessResponse,
  GetProjectPendingInvitationsApiResponse,
  GetProjectPendingInvitationsSuccessResponse,
  PatchProjectMemberRoleApiResponse,
  PatchProjectMemberRoleRequest,
  PatchProjectMemberRoleSuccessResponse,
  UpdateProjectInvitationStatusApiResponse,
  UpdateProjectInvitationStatusRequest,
  UpdateProjectInvitationStatusSuccessResponse,
} from './projectMembers.types'

const serializeParams = (params: GetProjectMembersParams): string => {
  const searchParams = new URLSearchParams()
  if (typeof params.limit === 'number')
    searchParams.set('limit', String(params.limit))
  if (params.cursor) searchParams.set('cursor', params.cursor)
  return searchParams.toString()
}

export class ProjectMembersGateway {
  static async getMembers(
    projectId: string,
    params: GetProjectMembersParams = {}
  ): Promise<GetProjectMembersSuccessResponse> {
    const response = await apiClient.get<GetProjectMembersApiResponse>(
      ApiRoutes.Project.Members.List(projectId),
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
      throw new ApiError('ProjectMembers.List.InvalidResponse', {
        message: 'Project members list response is invalid',
      })
    }

    return payload
  }

  static async getPendingInvitations(
    projectId: string
  ): Promise<GetProjectPendingInvitationsSuccessResponse> {
    const response =
      await apiClient.get<GetProjectPendingInvitationsApiResponse>(
        ApiRoutes.Project.Invitations.Pending(projectId)
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
      throw new ApiError('ProjectInvitations.Pending.InvalidResponse', {
        message: 'Project pending invitations response is invalid',
      })
    }

    return payload
  }

  static async updateMemberRole(
    projectId: string,
    memberId: string,
    payload: PatchProjectMemberRoleRequest
  ): Promise<PatchProjectMemberRoleSuccessResponse> {
    const response =
      await apiClient.patch<PatchProjectMemberRoleApiResponse>(
        ApiRoutes.Project.Members.Update(projectId, memberId),
        payload
      )

    if (response.status === 204) return

    const payloadResponse = response.data

    if (isApiErrorResponse(payloadResponse)) {
      throw new ApiError(payloadResponse.error.code, {
        message: payloadResponse.error.message,
        status: response.status,
        meta: payloadResponse.meta,
      })
    }
  }

  static async createInvitation(
    projectId: string,
    payload: CreateProjectInvitationRequest
  ): Promise<CreateProjectInvitationSuccessResponse> {
    const response =
      await apiClient.post<CreateProjectInvitationApiResponse>(
        ApiRoutes.Project.Invitations.Create(projectId),
        { userId: payload.userId, roleId: payload.roleId }
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
      throw new ApiError('ProjectInvitations.Create.InvalidResponse', {
        message: 'Project invitation create response is invalid',
      })
    }

    return rawPayload
  }

  static async updateInvitationStatus(
    projectId: string,
    invitationId: string,
    payload: UpdateProjectInvitationStatusRequest
  ): Promise<UpdateProjectInvitationStatusSuccessResponse> {
    const response =
      await apiClient.patch<UpdateProjectInvitationStatusApiResponse>(
        ApiRoutes.Project.Invitations.Update(projectId, invitationId),
        payload
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
      throw new ApiError('ProjectInvitations.UpdateStatus.InvalidResponse', {
        message: 'Project invitation update status response is invalid',
      })
    }

    return rawPayload
  }

  static async deleteMember(
    projectId: string,
    memberId: string
  ): Promise<DeleteProjectMemberSuccessResponse> {
    const response =
      await apiClient.delete<DeleteProjectMemberApiResponse>(
        ApiRoutes.Project.Members.Delete(projectId, memberId)
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
      throw new ApiError('ProjectMembers.Delete.InvalidResponse', {
        message: 'Project member delete response is invalid',
      })
    }

    return payloadResponse
  }
}
