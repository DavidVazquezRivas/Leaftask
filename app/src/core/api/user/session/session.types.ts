import type {
  ApiErrorResponse,
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export interface RefreshSessionData {
  accessToken: string
}

export interface GoogleOAuthRequest {
  token: string
}

export interface GoogleOAuthSessionData {
  accessToken: string
}

export interface SessionMeData {
  id?: string
  name?: string
  firstName?: string
  lastName?: string
  email?: string
  avatarUrl?: string | null
}

export type RefreshSessionSuccessResponse =
  ApiSuccessResponse<RefreshSessionData>
export type RefreshSessionApiResponse = ApiResponse<RefreshSessionData>
export type GoogleOAuthSessionSuccessResponse =
  ApiSuccessResponse<GoogleOAuthSessionData>
export type GoogleOAuthSessionApiResponse = ApiResponse<GoogleOAuthSessionData>
export type SessionMeSuccessResponse = ApiSuccessResponse<SessionMeData>
export type SessionMeApiResponse = ApiResponse<SessionMeData>
export type SessionLogoutApiResponse = ApiResponse<unknown>

export interface SessionApiError extends ApiErrorResponse {
  error: {
    code: string
    message: string
  }
}
