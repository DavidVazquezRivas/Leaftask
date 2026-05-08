import type {
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export const ProjectPrivacy = {
  Public: 0,
  Restricted: 1,
  Private: 2,
} as const

export type ProjectPrivacy = (typeof ProjectPrivacy)[keyof typeof ProjectPrivacy]

export interface SimpleProjectData {
  id: string
  name: string
  abbreviation: string
}

export interface ProjectData {
  id: string
  name: string
  abbreviation: string
  privacyLevel: ProjectPrivacy
  organizationId: string | null
  createdAt: string
}

export interface GetProjectsParams {
  limit?: number
  cursor?: string | null
  sort?: string[]
}

export interface CreateProjectRequest {
  name: string
  abbreviation: string
  privacyLevel: ProjectPrivacy
  organizationId?: string | null
}

export interface PatchProjectRequest {
  name?: string | null
  abbreviation?: string | null
  privacyLevel?: ProjectPrivacy | null
}

export type GetProjectsSuccessResponse = ApiSuccessResponse<SimpleProjectData[]>
export type GetProjectsApiResponse = ApiResponse<SimpleProjectData[]>

export type GetProjectDetailSuccessResponse = ApiSuccessResponse<ProjectData>
export type GetProjectDetailApiResponse = ApiResponse<ProjectData>

export type CreateProjectSuccessResponse = ApiSuccessResponse<ProjectData>
export type CreateProjectApiResponse = ApiResponse<ProjectData>

export type PatchProjectSuccessResponse = ApiSuccessResponse<ProjectData>
export type PatchProjectApiResponse = ApiResponse<ProjectData>

export type DeleteProjectSuccessResponse = ApiSuccessResponse<null>
export type DeleteProjectApiResponse = ApiResponse<null>
