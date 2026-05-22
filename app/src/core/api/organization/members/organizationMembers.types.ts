import type {
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export interface OrganizationMemberData {
  id: string
  name: string
  email: string
  role: string
}

export interface OrganizationMemberDistributionData {
  id: string
  memberCount: number
}

export interface GetOrganizationMembersParams {
  limit?: number
  cursor?: string | null
}

export interface PatchOrganizationMemberRoleRequest {
  roleId: string
}

export type GetOrganizationMembersSuccessResponse = ApiSuccessResponse<
  OrganizationMemberData[]
>

export type GetOrganizationMembersApiResponse = ApiResponse<
  OrganizationMemberData[]
>

export type GetOrganizationMembersDistributionSuccessResponse =
  ApiSuccessResponse<OrganizationMemberDistributionData[]>

export type GetOrganizationMembersDistributionApiResponse = ApiResponse<
  OrganizationMemberDistributionData[]
>

export type PatchOrganizationMemberRoleSuccessResponse =
  ApiSuccessResponse<OrganizationMemberData>

export type PatchOrganizationMemberRoleApiResponse =
  ApiResponse<OrganizationMemberData>

export type DeleteOrganizationMemberSuccessResponse = ApiSuccessResponse<null>

export type DeleteOrganizationMemberApiResponse = ApiResponse<null>
