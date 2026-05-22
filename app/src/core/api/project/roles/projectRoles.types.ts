import type {
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export type ProjectRolePermissionLevel = 0 | 1 | 2

export interface ProjectRolesPermissionData {
  id: string
  name: string
  description: string
  permissionType: string
}

export interface ProjectRolePermissionAssignmentData {
  permissionId: string
  level: ProjectRolePermissionLevel
}

export interface ProjectRoleData {
  id: string
  name: string
  totalMembers: number
  permissions: ProjectRolePermissionAssignmentData[]
}

export interface CreateProjectRoleRequest {
  name: string
  permissions: ProjectRolePermissionAssignmentData[]
}

export interface PatchProjectRoleRequest {
  name?: string | null
  permissions?: ProjectRolePermissionAssignmentData[]
}

export type GetProjectRolesPermissionsSuccessResponse = ApiSuccessResponse<
  ProjectRolesPermissionData[]
>

export type GetProjectRolesPermissionsApiResponse = ApiResponse<
  ProjectRolesPermissionData[]
>

export type GetProjectRolesSuccessResponse =
  ApiSuccessResponse<ProjectRoleData[]>

export type GetProjectRolesApiResponse = ApiResponse<ProjectRoleData[]>

export type CreateProjectRoleSuccessResponse = ApiSuccessResponse<ProjectRoleData>

export type CreateProjectRoleApiResponse = ApiResponse<ProjectRoleData>

export type PatchProjectRoleSuccessResponse = ApiSuccessResponse<ProjectRoleData>

export type PatchProjectRoleApiResponse = ApiResponse<ProjectRoleData>

export type DeleteProjectRoleSuccessResponse = ApiSuccessResponse<null>

export type DeleteProjectRoleApiResponse = ApiResponse<null>
