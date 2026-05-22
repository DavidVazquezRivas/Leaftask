import type {
  ApiResponse,
  ApiSuccessResponse,
  SortDirection,
} from '@/core/api/global/types/response'

export interface OrganizationManagementSummaryData {
  id: string
  name: string
}

export interface OrganizationManagementPermissionData {
  id: string
  name: string
  level: 0 | 1 | 2
}

export interface OrganizationManagementDetailData {
  id: string
  name: string
  description: string
  website?: string | null
  totalMembers: number
  activeProjects: number
  customRoles: number
  createdAt: string
}

export type OrganizationManagementSortParam = `${string}:${SortDirection}`

export interface GetOrganizationManagementParams {
  limit?: number
  cursor?: string | null
  sort?: OrganizationManagementSortParam[]
}

export interface CreateOrganizationManagementRequest {
  name: string
  description: string
  website: string
}

export interface PatchOrganizationManagementRequest {
  name?: string | null
  description?: string | null
  website?: string | null
}

export type GetOrganizationManagementSuccessResponse = ApiSuccessResponse<
  OrganizationManagementSummaryData[]
>
export type GetOrganizationManagementApiResponse = ApiResponse<
  OrganizationManagementSummaryData[]
>

export type CreateOrganizationManagementSuccessResponse =
  ApiSuccessResponse<OrganizationManagementSummaryData>

export type CreateOrganizationManagementApiResponse =
  ApiResponse<OrganizationManagementSummaryData>

export type GetOrganizationManagementDetailSuccessResponse =
  ApiSuccessResponse<OrganizationManagementDetailData>

export type GetOrganizationManagementDetailApiResponse =
  ApiResponse<OrganizationManagementDetailData>

export type PatchOrganizationManagementSuccessResponse =
  ApiSuccessResponse<OrganizationManagementDetailData>

export type PatchOrganizationManagementApiResponse =
  ApiResponse<OrganizationManagementDetailData>

export type DeleteOrganizationManagementSuccessResponse =
  ApiSuccessResponse<null>

export type DeleteOrganizationManagementApiResponse = ApiResponse<null>

export type GetOrganizationManagementPermissionsSuccessResponse =
  ApiSuccessResponse<OrganizationManagementPermissionData[]>

export type GetOrganizationManagementPermissionsApiResponse = ApiResponse<
  OrganizationManagementPermissionData[]
>
