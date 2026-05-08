import type {
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export interface ProjectMemberData {
  id: string
  name: string
  email: string | null
  role: string
}

export interface ProjectPendingInvitationData {
  id: string
  user: {
    id: string
    name: string
    email: string | null
  }
  role: {
    id: string
    name: string
  }
}

export interface GetProjectMembersParams {
  limit?: number
  cursor?: string | null
}

export interface PatchProjectMemberRoleRequest {
  roleId: string
}

export type GetProjectMembersSuccessResponse =
  ApiSuccessResponse<ProjectMemberData[]>

export type GetProjectMembersApiResponse = ApiResponse<ProjectMemberData[]>

export type GetProjectPendingInvitationsSuccessResponse =
  ApiSuccessResponse<ProjectPendingInvitationData[]>

export type GetProjectPendingInvitationsApiResponse =
  ApiResponse<ProjectPendingInvitationData[]>

export type PatchProjectMemberRoleSuccessResponse =
  ApiSuccessResponse<ProjectMemberData>

export type PatchProjectMemberRoleApiResponse =
  ApiResponse<ProjectMemberData>

export type DeleteProjectMemberSuccessResponse = ApiSuccessResponse<null>

export type DeleteProjectMemberApiResponse = ApiResponse<null>

export interface CreateProjectInvitationRequest {
  userId: string
  roleId: string
}

export interface UpdateProjectInvitationStatusRequest {
  status: string
}

export type CreateProjectInvitationSuccessResponse = ApiSuccessResponse<null>

export type CreateProjectInvitationApiResponse = ApiResponse<null>

export type UpdateProjectInvitationStatusSuccessResponse =
  ApiSuccessResponse<null>

export type UpdateProjectInvitationStatusApiResponse = ApiResponse<null>
