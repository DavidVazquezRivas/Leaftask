import { ApiRoutes } from '@/core/api/global/constants/routes'
import { ApiError } from '@/core/api/global/errors'
import { apiClient } from '@/core/api/global/httpClient'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  DeleteOrganizationMemberApiResponse,
  DeleteOrganizationMemberSuccessResponse,
  GetOrganizationMembersApiResponse,
  GetOrganizationMembersDistributionApiResponse,
  GetOrganizationMembersDistributionSuccessResponse,
  GetOrganizationMembersParams,
  GetOrganizationMembersSuccessResponse,
  PatchOrganizationMemberRoleApiResponse,
  PatchOrganizationMemberRoleRequest,
  PatchOrganizationMemberRoleSuccessResponse,
} from '@/core/api/organization/members/organizationMembers.types'

const serializeOrganizationMembersParams = (
  params: GetOrganizationMembersParams
): string => {
  const searchParams = new URLSearchParams()

  if (typeof params.limit === 'number') {
    searchParams.set('limit', String(params.limit))
  }

  if (params.cursor) {
    searchParams.set('cursor', params.cursor)
  }

  return searchParams.toString()
}

export class OrganizationMembersGateway {
  static async getMembers(
    organizationId: string,
    params: GetOrganizationMembersParams = {}
  ): Promise<GetOrganizationMembersSuccessResponse> {
    const response = await apiClient.get<GetOrganizationMembersApiResponse>(
      ApiRoutes.Organization.Members.List(organizationId),
      {
        params,
        paramsSerializer: () => serializeOrganizationMembersParams(params),
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
      throw new ApiError('OrganizationMembers.List.InvalidResponse', {
        message: 'Organization members list response is invalid',
      })
    }

    return payload
  }

  static async getDistribution(
    organizationId: string
  ): Promise<GetOrganizationMembersDistributionSuccessResponse> {
    const response =
      await apiClient.get<GetOrganizationMembersDistributionApiResponse>(
        ApiRoutes.Organization.Members.Distribution(organizationId)
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
      throw new ApiError('OrganizationMembers.Distribution.InvalidResponse', {
        message: 'Organization members distribution response is invalid',
      })
    }

    return payload
  }

  static async updateMemberRole(
    organizationId: string,
    memberId: string,
    payload: PatchOrganizationMemberRoleRequest
  ): Promise<PatchOrganizationMemberRoleSuccessResponse> {
    const response =
      await apiClient.patch<PatchOrganizationMemberRoleApiResponse>(
        ApiRoutes.Organization.Members.Update(organizationId, memberId),
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
      throw new ApiError('OrganizationMembers.Update.InvalidResponse', {
        message: 'Organization member update response is invalid',
      })
    }

    return payloadResponse
  }

  static async deleteMember(
    organizationId: string,
    memberId: string
  ): Promise<DeleteOrganizationMemberSuccessResponse> {
    const response =
      await apiClient.delete<DeleteOrganizationMemberApiResponse>(
        ApiRoutes.Organization.Members.Delete(organizationId, memberId)
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
      throw new ApiError('OrganizationMembers.Delete.InvalidResponse', {
        message: 'Organization member delete response is invalid',
      })
    }

    return payloadResponse
  }
}
