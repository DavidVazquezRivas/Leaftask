import type {
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export type OrganizationRolePermissionLevel = 0 | 1 | 2

export interface OrganizationRolesPermissionData {
  id: string
  name: string
  description: string
}

export interface OrganizationRolePermissionAssignmentData {
  id: string
  level: OrganizationRolePermissionLevel
}

export interface OrganizationRoleData {
  id: string
  name: string
  totalMembers: number
  permissions: OrganizationRolePermissionAssignmentData[]
}

export interface CreateOrganizationRoleRequest {
  name: string
  permissions: OrganizationRolePermissionAssignmentData[]
}

export interface PatchOrganizationRoleRequest {
  name?: string | null
  permissions?: OrganizationRolePermissionAssignmentData[]
}

export type GetOrganizationRolesPermissionsSuccessResponse = ApiSuccessResponse<
  OrganizationRolesPermissionData[]
>

export type GetOrganizationRolesPermissionsApiResponse = ApiResponse<
  OrganizationRolesPermissionData[]
>

export type GetOrganizationRolesSuccessResponse = ApiSuccessResponse<
  OrganizationRoleData[]
>

export type GetOrganizationRolesApiResponse = ApiResponse<
  OrganizationRoleData[]
>

export type CreateOrganizationRoleSuccessResponse =
  ApiSuccessResponse<OrganizationRoleData>

export type CreateOrganizationRoleApiResponse =
  ApiResponse<OrganizationRoleData>

export type PatchOrganizationRoleSuccessResponse =
  ApiSuccessResponse<OrganizationRoleData>

export type PatchOrganizationRoleApiResponse = ApiResponse<OrganizationRoleData>

export type DeleteOrganizationRoleSuccessResponse = ApiSuccessResponse<null>

export type DeleteOrganizationRoleApiResponse = ApiResponse<null>
