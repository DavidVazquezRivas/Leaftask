import { ApiRoutes } from '@/core/api/global/constants/routes'
import { ApiError } from '@/core/api/global/errors'
import { apiClient } from '@/core/api/global/httpClient'
import { isAxiosError } from 'axios'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  CancelOrganizationInvitationApiResponse,
  CancelOrganizationInvitationRequest,
  CancelOrganizationInvitationSuccessResponse,
  CreateOrganizationInvitationApiResponse,
  CreateOrganizationInvitationRequest,
  CreateOrganizationInvitationSuccessResponse,
  GetPendingOrganizationInvitationsApiResponse,
  GetPendingOrganizationInvitationsSuccessResponse,
} from '@/core/api/organization/invitations/organizationInvitations.types'

export class OrganizationInvitationsGateway {
  static async cancelInvitation(
    organizationId: string,
    invitationId: string,
    payload: CancelOrganizationInvitationRequest
  ): Promise<CancelOrganizationInvitationSuccessResponse> {
    const response =
      await apiClient.patch<CancelOrganizationInvitationApiResponse>(
        ApiRoutes.Organization.Invitations.Update(organizationId, invitationId),
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
      throw new ApiError('OrganizationInvitations.Cancel.InvalidResponse', {
        message: 'Organization invitation cancel response is invalid',
      })
    }

    return payloadResponse
  }

  static async getPendingInvitations(
    organizationId: string
  ): Promise<GetPendingOrganizationInvitationsSuccessResponse> {
    const response =
      await apiClient.get<GetPendingOrganizationInvitationsApiResponse>(
        ApiRoutes.Organization.Invitations.Pending(organizationId)
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
      throw new ApiError('OrganizationInvitations.Pending.InvalidResponse', {
        message: 'Organization pending invitations response is invalid',
      })
    }

    return payloadResponse
  }

  static async respondToInvitation(
    organizationId: string,
    invitationId: string,
    status: 'accepted' | 'rejected'
  ): Promise<CancelOrganizationInvitationSuccessResponse> {
    const response = await apiClient.patch<CancelOrganizationInvitationApiResponse>(
      ApiRoutes.Organization.Invitations.Update(organizationId, invitationId),
      { status }
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
      throw new ApiError('OrganizationInvitations.Respond.InvalidResponse', {
        message: 'Organization invitation respond response is invalid',
      })
    }

    return payloadResponse
  }

  static async createInvitation(
    organizationId: string,
    payload: CreateOrganizationInvitationRequest
  ): Promise<CreateOrganizationInvitationSuccessResponse> {
    try {
      const response =
        await apiClient.post<CreateOrganizationInvitationApiResponse>(
          ApiRoutes.Organization.Invitations.Create(organizationId),
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
        throw new ApiError('OrganizationInvitations.Create.InvalidResponse', {
          message: 'Organization invitation create response is invalid',
        })
      }

      return payloadResponse
    } catch (error) {
      if (isAxiosError(error) && error.response) {
        const payloadResponse = error.response.data

        if (isApiErrorResponse(payloadResponse)) {
          throw new ApiError(payloadResponse.error.code, {
            message: payloadResponse.error.message,
            status: error.response.status,
            meta: payloadResponse.meta,
            cause: error,
          })
        }
      }

      throw error
    }
  }
}
