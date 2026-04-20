import type {
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export interface OrganizationInvitationData {
  id: string
  organizationId: string
  userId: string
  organizationRoleId: string
  status: string
  invitedAt: string
  respondedAt: string | null
  abandonedAt: string | null
}

export interface GetPendingOrganizationInvitationData {
  id: string
  organizationId: string
  userId: string
  organizationRoleId: string
  status: string
  invitedAt: string
  respondedAt: string | null
  abandonedAt: string | null
}

export interface CreateOrganizationInvitationRequest {
  userId: string
  roleId: string
}

export interface CancelOrganizationInvitationRequest {
  status: 'canceled'
}

export type CreateOrganizationInvitationSuccessResponse =
  ApiSuccessResponse<OrganizationInvitationData>

export type CreateOrganizationInvitationApiResponse =
  ApiResponse<OrganizationInvitationData>

export type GetPendingOrganizationInvitationsSuccessResponse =
  ApiSuccessResponse<GetPendingOrganizationInvitationData[]>

export type GetPendingOrganizationInvitationsApiResponse = ApiResponse<
  GetPendingOrganizationInvitationData[]
>

export type CancelOrganizationInvitationSuccessResponse =
  ApiSuccessResponse<OrganizationInvitationData>

export type CancelOrganizationInvitationApiResponse =
  ApiResponse<OrganizationInvitationData>
