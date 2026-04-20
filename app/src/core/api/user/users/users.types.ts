import type {
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export interface UserListItemData {
  id: string
  fullName: string
  email: string
}

export interface GetUsersParams {
  limit?: number
  cursor?: string | null
  search?: string
}

export type GetUsersSuccessResponse = ApiSuccessResponse<UserListItemData[]>

export type GetUsersApiResponse = ApiResponse<UserListItemData[]>
